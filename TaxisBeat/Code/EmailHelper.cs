using System.Linq;
using System.Web;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;
using Task = System.Threading.Tasks.Task;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using System.Text;
using Umbraco.Core;
using Umbraco.Web.PublishedContentModels;
using System.Collections.Generic;

namespace TaxisBeat.Code
{
    public class EmailHelper
    {
        private const string DefaultLang = "el-GR";
        private static readonly string DateFormat = "dd/MM/yyyy HH:mm:ss";
        private const string EmailFromDefault = "lal@gmail.com";
        private const string GuidReplacementString = "##GUID##";
        private const string MemberReplaceString = "{MEMBER}";
        private const string ProfileUrlReplaceString = "##PROFILEPAGE_URL##";
        private const string CompetitionWinnerNeedsIdentity = "{NEEDSIDENTITYMESSAGE}";

        // Email Tempalte Contact Us.
        private const string ContactUsName      = "{Name}";
        private const string ContactUsPhone     = "{Phone}";
        private const string ContactUsSubject   = "{Subject}";
        private const string ContactUsEmail     = "{Email}";
        private const string ContactUsMessage   = "{Message}";
        private const string ContactUsService   = "{Service}";
        private const string ContactUsWebsite   = "{Website}";
        private const string ContactUsLang      = "{Lang}";

        private static readonly IContent Website = ApplicationContext.Current.Services.ContentService.GetById(Constants.HomePageId);
        private static readonly IEnumerable<IContent> RootPages = ApplicationContext.Current.Services.ContentService.GetRootContent();
        private static readonly IContent EmailTemplateRepo = RootPages.FirstOrDefault(z => z.ContentType.Alias == Constants.EmailTemplateRepo);

        public static async Task<Response> SendGridMail(SendGridMessage message, string destinationEmail = null, string destinationName = null)
        {
            var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = ConfigurationManager.AppSettings.Get("SENDGRID_APIKEY");
            }
            var client = new SendGridClient(apiKey);
#if DEBUG
            message.AddTo(new EmailAddress("argigero@gmail.com", "Anarguros"));
#else
            if (destinationEmail != null)
            {
                message.AddTo(new EmailAddress(destinationEmail, destinationName));
            }
#endif


            return
                await client.SendEmailAsync(message);
        }

        public static Task PasswordChangedSuccessfully(IPublishedContent member)
        {
            var emailTemplate = GetEmailTeplate(Constants.PasswordChangedSuccessfullyTemplate);

            var body = emailTemplate?.EmailTemplateEmailBody?.ToString()
                        .Replace(MemberReplaceString, member.UserName());
            return
                SendGridMailForTemplate(emailTemplate, body, member.Email());
        }

        public static Task SendResetPasswordEmail(UmbracoHelper umbracoHelper, IMember member, string resetGUID, IPublishedContent currentPage)
        {
            var resetURL = $"{HttpContext.Current.Request.Url.Host}?resetGUID={resetGUID}";
            if (!string.IsNullOrEmpty(resetGUID))
            {
                var baseURL = UmbracoContext.Current.UrlProvider.GetUrl(Constants.ResetPasswordPageId, true);
                resetURL = $"{baseURL}?resetGUID={resetGUID}";
            }

            var emailTemplate = GetEmailTeplate(Constants.ResetPasswordEmailTemplate);

            var body = emailTemplate.EmailTemplateEmailBody?.ToString()
                .Replace(GuidReplacementString, resetURL)
                .Replace(MemberReplaceString, member.Username);

            return SendGridMailForTemplate(emailTemplate, body, member.Email);
        }

        internal static Task SendMailContactUs(UmbracoHelper umbracoHelper, IContent newContactRequest, IPublishedContent website, string lang)
        {
            var emailTemplate = GetEmailTeplate(Constants.ContactUsRequestTemplate);
            var body = new StringBuilder();
            var service = umbracoHelper.GetDictionaryValue(umbracoHelper.GetPreValueAsString(newContactRequest.GetValue<int>("contactUsServices")));
            body.AppendLine(emailTemplate.EmailTemplateEmailBody.ToString());
            body.Replace(ContactUsName, newContactRequest.GetValue<string>("contactUsName"))
                .Replace(ContactUsPhone, newContactRequest.GetValue<string>("contactUsPhone"))
                .Replace(ContactUsSubject, newContactRequest.GetValue<string>("contactUsSubject"))
                .Replace(ContactUsEmail, newContactRequest.GetValue<string>("contactUsEmail"))
                .Replace(ContactUsMessage, newContactRequest.GetValue<string>("contactUsMessage"))
                .Replace(ContactUsService, service)
                .Replace(ContactUsWebsite, newContactRequest.GetValue<string>("contactUsWebsite"))
                .Replace(ContactUsLang, newContactRequest.GetValue<string>("contactUsLanguage"));

            return SendGridMailForTemplate(emailTemplate, body.ToString(),
                subject: emailTemplate.EmailTemplateSubject.Replace(ContactUsService, service));
        }

        public static Task SendVerifyEmail(UmbracoHelper umbracoHelper, IMember member, string verifyGUID)
        {
            var emailTemplate = GetEmailTeplate(Constants.VerifyEmailNewMemberTemplate);
            var body = emailTemplate?.EmailTemplateEmailBody?.ToString()
                        .Replace(GuidReplacementString, umbracoHelper.NiceUrlWithDomain(Constants.HomePageId)
                            + "umbraco/Surface/AuthSurface/verifyEmail?verifyGUID=" + verifyGUID + "&wid=" + Constants.HomePageId)
                        .Replace(MemberReplaceString, member.Username);

            return SendGridMailForTemplate(emailTemplate, body, member.Email);
        }

        // TODO: when member requests for a new service.
        //public static Task SendNewServiceRequest(IPublishedContent member, ServiceRequest request, IPublishedContent website)
        //{
        //    var emailTemplate = GetEmailTeplate(Constants.CompetitionParticipationTemplate, website, lang);
        //    var body = emailTemplate?.EmailTemplateEmailBody?.ToString()
        //        .Replace(MemberReplaceString, member?.UserName())
        //        .Replace(CompetitionReplaceString, competition?.Name(lang));

        //    var subject = emailTemplate?.EmailTemplateSubject
        //        .Replace(CompetitionReplaceString, competition?.Name(lang));

        //    return SendGridMailForTemplate(emailTemplate, body, member?.Email(), member.Name, subject);
        //}


        public static EmailTemplate GetEmailTeplate(string emailTemplateName)
        {
            var emailTemplate = EmailTemplateRepo?.Children()?.FirstOrDefault(z => z.Name == emailTemplateName);
            if (emailTemplate == null)
            {
                // then try get it through Email Type.
                emailTemplate = EmailTemplateRepo?.Children()?.FirstOrDefault(z => (string)z.Properties["EmailTemplateCode"].Value == emailTemplateName);
                if (emailTemplate == null)
                {
                    LogHelper.Warn(typeof(EmailHelper), $"The Templated with Name: {emailTemplateName} was not found in Content tree of Email Templates!");
                    return default(EmailTemplate);
                }
            }
            return emailTemplate as EmailTemplate;
        }

        private static async Task<Response> SendGridMailForTemplate(EmailTemplate emailTemplate, string body = null, string destinationEmail = null, string destinationName = null, string subject = null)
        {
            var bd = body != null ? body : emailTemplate.EmailTemplateEmailBody.ToString();

            return
                await
                    SendGridMail(
                    new SendGridMessage()
                    {
#if DEBUG
                        From = new EmailAddress("argigero@gmail.com", "fake argigero"),
#else
                        From = new EmailAddress(emailTemplate.EmailTemplateSenderEmail, emailTemplate.EmailTemplateSenderName),
#endif

                        Subject = subject ?? emailTemplate.EmailTemplateSubject,
                        PlainTextContent = StripHTML(bd),
                        HtmlContent = string.IsNullOrEmpty(bd) ? string.Empty : bd,
                    }, destinationEmail ?? emailTemplate.EmailTemplateRecipientEmail
                    , destinationName);
        }


        private static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

    }
}