using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Core.Logging;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedContentModels;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Web.PublishedCache;
using System.Configuration;
using TaxisBeat.Models;
using TaxisBeat.Code;

namespace TaxisBeat.Controllers
{
    public class AuthSurfaceController : SurfaceController
    {
        public static readonly int ExpirePasswordMinutes = int.Parse(ConfigurationManager.AppSettings.Get("ExpirePasswordMinutes"));

        [AllowAnonymous]
        public ActionResult LoginIndex()
        {
            return PartialView("Profile/Login", new LoginModel());
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult ValidateFacebook(LoginModel model)
        {
            // get all data.
            var accessToken = model != null && !string.IsNullOrEmpty(model.AccessToken) ? model.AccessToken : null;
            var client = new Facebook.FacebookClient(accessToken);
            if (client == null)
            {
                return Json(new
                {
                    ErrorMessage = "You should provide correct credentials to login with facebook.",
                    Success = false
                });
            }

            var userDetail = client.Get("/me", new { fields = "last_name,id,email,name,middle_name,first_name,locale,verified" });
            if (userDetail == null)
            {
                return Json(new
                {
                    ErrorMessage = "You should provide correct credentials to login with facebook.",
                    Success = false
                });
            }

            var loginDetail = new FacebookLoginDetail(JObject.FromObject(userDetail), accessToken);
            if (loginDetail == null
#if !DEBUG             
                || !loginDetail.IsFacebookVerified
#endif                
            )
            {
                return Json(new
                {
                    ErrorMessage = "You have to verify you facebook acount!",
                    Success = false
                });
            }

            // ok, check if exists as Member with userid, if so, log him in. and return state
            if (!string.IsNullOrEmpty(loginDetail.UserId))
            {
                var member = Services.MemberService.GetMembersByPropertyValue("facebookUserId", loginDetail.UserId).FirstOrDefault();
                if (member != null)
                {
                    var tryLogin = Members.Login(member.Username, loginDetail.UserId);
                    if (!tryLogin)
                    {
                        return Json(new
                        {
                            ErrorMessage = "Your password is invalid",
                            Success = false,
                            RedirectUrl = string.Empty,
                            Data = loginDetail
                        });
                    }
                    return GetLoggedInMember(member.Name, member.Id, model.CurrentPageId, Umbraco);
                }
            }

            // Redirect him to register, with predifined fields
            return Json(new
            {
                ErrorMessage = string.Empty,
                Success = false,
                IsValidMember = false,
                NeedsRegister = true,
                Data = loginDetail
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult Validate(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    ErrorMessage = "Please check your fields entered to login.",
                    Success = false
                });
            }

            // get member with email
            var member = Members.GetByEmail(model.Email);
            if (member == null)
            {
                // try get it through username.
                member = Members.GetByUsername(model.Email);
            }

            if (member == null)
            {
                return Json(new
                {
                    ErrorMessage = "This member does not exists.", 
                    Success = false
                });
            }


            if (member.HasProperty("hasVerifiedEmail") && member.GetPropertyValue<bool>("hasVerifiedEmail") == false)
            {
                return Json(new
                {
                    ErrorMessage = "You have to verify tou email first.",
                    Success = false
                });
            }

            // if member is a FB member, don't log-in him this way.
            if (member.HasProperty("facebookUserId") && !string.IsNullOrEmpty(member.GetPropertyValue<string>("facebookUserId")))
            {
                LogHelper.Warn(typeof(AuthSurfaceController),
                    $"A user tried log in with a FB member by using his userId and email. User: {Request.UserHostAddress}, Member: {member.Name}");

                return Json(new
                {
                    ErrorMessage = "This incident will be reported. If you have a FB account, then try login by clicking on FB icon.",
                    Success = false
                });
            }

            // if username/email is enabled then must check the workflow
            if (!Members.Login(member.UserName(), model.Password))
            {
                return Json(new
                {
                    ErrorMessage = "Your password is invalid", 
                    Success = false,
                    RedirectUrl = string.Empty
                });
            }
            return GetLoggedInMember(member.Name, member.Id, model.CurrentPageId, Umbraco);
        }

        [HttpPost]
        [Authorize]
        public ActionResult AntiFg()
        {
            return PartialView("Common/Forgery");
        }

        [AllowAnonymous]
        public ActionResult Logout(int pageId)
        {
            if (Members.IsLoggedIn())
            {
                Members.Logout();
            }

            // handle correct redirect to site.
            var curPage = Umbraco.TypedContent(pageId);
            switch (curPage?.GetTemplateAlias())
            {
                case Constants.HomeTemplateAlias:
                //case Constants.ProfileTemplateAlias:
                    return RedirectToUmbracoPage(curPage?.Parent?.Id ?? pageId);
                default:
                    return RedirectToUmbracoPage(pageId);
            }
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return PartialView("Various/ForgottenPassword", new ForgottenPasswordViewModel());
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> HandleForgottenPassword(ForgottenPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Success = false,
                    ErrorMessage = "Please check your details entered again.",
                });
            }

            // the process should take care of Facebook user id.

            // get member with email
            var member = Members.GetByEmail(model.EmailAddress);
            if (member == null)
            {
                // try get it through username.
                member = Members.GetByUsername(model.EmailAddress);
            }

            if (member != null)
            {
                // Get the details of the user currently logged in
                var currentMember = Members.GetCurrentMember();
                if (currentMember != null
                    && currentMember.Email().Trim().ToLowerInvariant() != model.EmailAddress.Trim().ToLowerInvariant())
                {
                    LogHelper.Error(typeof(AuthSurfaceController),
                        $"Possible fraud, logged in member: {currentMember.Email()}, username:{currentMember.UserName()}, try reset password of other Member: {model.EmailAddress}", new InvalidOperationException("Fraud"));
                    return Json(new { Success = false });
                }

                // if its a facebook user cannot reset password!
                var fbUserId = member.GetPropertyValue<string>("facebookUserId");
                if (!string.IsNullOrEmpty(fbUserId))
                {
                    return Json(new
                    {
                        ErrorMessage = "Facebook members cannot change password!",
                        Success = false,
                    });
                }

                var expiryTime = DateTime.UtcNow.AddMinutes(ExpirePasswordMinutes);
                var hash = $"{HashHelper.GetSHA256Hash(member.Email())}__{expiryTime.Ticks}";

                // update password hash
                var resetMember = Services.MemberService.GetById(member.Id);
                resetMember.SetValue("passwordResetHash", hash);
                Services.MemberService.Save(resetMember);

                await EmailHelper.SendResetPasswordEmail(Umbraco, resetMember, hash, CurrentPage);
                return Json(new
                {
                    Success = true,
                    ErrorMessage = "A reset password email had been sent to your email account. Please check.",
                });
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    ErrorMessage = "No member found with this credentials.",
                });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HandleResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Various/ResetPassword", model);
            }

            //Get member from email
            var curMember = Members.GetByEmail(model.EmailAddress);

            //Ensure we have that member
            if (curMember != null)
            {
                // if its a facebook user cannot reset password!
                var fbUserId = curMember.GetPropertyValue<string>("facebookUserId");
                if (!string.IsNullOrEmpty(fbUserId))
                {
                    ModelState.AddModelError("FormGenericError", "Facebook members cannot change password!");
                    return PartialView("Various/ResetPassword", model);
                }

                //Get the querystring GUID
                var resetQS = Request.QueryString["resetGUID"];

                //Ensure we have a vlaue in QS //See if the QS matches the value on the member property
                if (!string.IsNullOrEmpty(resetQS) && curMember.GetPropertyValue<string>("passwordResetHash") == resetQS)
                {
                    long millis;
                    var parsedExpiryTime = long.TryParse(resetQS.Substring(resetQS.IndexOf("__") + 2), out millis);
                    var expiryTime = new DateTime(millis);
                    var currentTime = DateTime.UtcNow;

                    //Check if date has NOT expired (been and gone)
                    if (parsedExpiryTime && currentTime.CompareTo(expiryTime) < 0)
                    {
                        var resetMember = Services.MemberService.GetById(curMember.Id);
                        resetMember.SetValue("passwordResetHash", string.Empty);
                        resetMember.IsLockedOut = false;
                        resetMember.FailedPasswordAttempts = 0;
                        Services.MemberService.Save(resetMember);
                        Services.MemberService.SavePassword(resetMember, model.Password);

                        if (!string.IsNullOrEmpty(resetMember.Email))
                        {
                            var currentPage = Umbraco.TypedContent(model.Cpid);
                            if (currentPage != null)
                            {
                                model.PasswordChangedOk = true;
                                var lang = currentPage.GetCulture()?.Name;
                                var website = currentPage?.Ancestors<Home>(1)?.FirstOrDefault();
                                await EmailHelper.PasswordChangedSuccessfully(curMember);
                            }
                        }

                        return PartialView("Various/ResetPassword", model);
                    }
                    else
                    {
                        ModelState.AddModelError("FormGenericError", "Reset password request has expired, try again!");
                        return PartialView("Various/ResetPassword", model);
                    }
                }
                else
                {
                    ModelState.AddModelError("FormGenericError", "Try request for a reset password from Login modal first.");
                    return PartialView("Various/ResetPassword", model);
                }
            }

            ModelState.AddModelError("FormGenericError", "While requesting a password reset, member does not exists.");
            return PartialView("Various/ResetPassword", model);
        }


        //REMOTE Validation
        [AllowAnonymous]
        public JsonResult CheckEmailIsUsed(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            var checkEmail = Members.GetByEmail(email);
            if (checkEmail != null)
            {
                return Json(string.Format("The email {0} is already in use", email), JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //REMOTE Validation
        [AllowAnonymous]
        public JsonResult CheckUsernamelIsUsed(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            var checkEmail = Members.GetByUsername(username);
            if (checkEmail != null)
            {
                return Json(string.Format("The username:{0} is already in use!", username), JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult RegisterIndex(int? pid)
        {
            // get terms and termsandconditionpage
            var curPage = pid != null ? Umbraco.TypedContent(pid) : CurrentPage;
            return PartialView("Profile/Register", new RegisterModel(Umbraco.TypedContent(Constants.TermAndConditionPage), curPage.Id));
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult RegisterFacebookIndex(int? pid)
        {
            var curPage = pid != null ? Umbraco.TypedContent(pid) : CurrentPage;
            return PartialView("Profile/RegisterFacebook", new RegisterFacebookModel(curPage.Id));
        }

        [HttpPost]
        public ActionResult RegisterFacebook(RegisterFacebookModel model)
        {
            if (!ModelState.IsValid || Members.IsLoggedIn() || string.IsNullOrEmpty(model.Email))
            {
                return PartialView("Profile/RegisterFacebook", model);
            }

            var memberService = Services.MemberService;
            var checkMemberEmail = Services.MemberService.GetByEmail(model.Email);
            if (checkMemberEmail != null)
            {
                ModelState.AddModelError("FormGenericError", "The email already in use!");
                return PartialView("Profile/RegisterFacebook", model);
            }
            
            // check if username unique
            if (Services.MemberService.GetByUsername(model.Username.Trim().ToLower()) != null)
            {
                ModelState.AddModelError("FormGenericError", "Το username χρησιμοποιείται ήδη από άλλο μέλος, επιλέξτε ένα άλλο.");
                return PartialView("Profile/RegisterFacebook", model);
            }

            // last check FB
            var client = new Facebook.FacebookClient(model.AccessToken);
            if (client != null)
            {
                var userDetail = client.Get("/me", new { fields = "last_name,id,email,name,birthday,age_range,middle_name,first_name,locale,verified" });
                if (userDetail != null)
                {
                    var loginDetail = new FacebookLoginDetail(JObject.FromObject(userDetail), model.AccessToken);
                    if (loginDetail == null || loginDetail.UserId != model.FacebookUserId)
                    {
                        ModelState.AddModelError("FormGenericError", "Κάντε Login στο Facebook με τον δικό σας λογαριασμό και ξαναδοκιμάστε!");
                        return PartialView("Profile/RegisterFacebook", model);
                    }

                    if (loginDetail == null
#if !DEBUG
                        || !loginDetail.IsFacebookVerified
#endif
                    )
                    {
                        ModelState.AddModelError("FormGenericError", "Πρέπει να έχετε επιβεβαιώση τον λογαριασμό σας στο Facebook ώστε να κάνετε την εγγραφή σας.");
                        return PartialView("Profile/RegisterFacebook", model);
                    }

                    if (string.Compare(loginDetail.Username, model.Username, true) != 0
                        && string.Compare(loginDetail.Email, model.Email, true) != 0)
                    {
                        ModelState.AddModelError("FormGenericError",
                            "Μπορείτε να αλλάξετε μόνο είτε το email σας, είτε το username σας όταν κάνετε εγγραφή μέσω Facebook.");
                        return PartialView("Profile/RegisterFacebook", model);
                    }

                    // assign correct userid.
                    model.FacebookUserId = loginDetail.UserId;
                }
            }
            else
            {
                // could not logon server side
                ModelState.AddModelError("FormGenericError", "Παρακάλω κάντε login στο Facebook πρώτα ώστε να προχωρήσετε με την εγγραφή σας στο site.");
                return PartialView("Profile/RegisterFacebook", model);
            }

            // check if user exists as a facebook user already.
            if (!string.IsNullOrEmpty(model.FacebookUserId))
            {
                var existingMember = Services.MemberService.GetMembersByPropertyValue("facebookUserId", model.FacebookUserId).FirstOrDefault();
                if (existingMember != null && existingMember.IsApproved)
                {
                    // log him in.
                    Members.Login(existingMember.Username, model.FacebookUserId);
                    return RedirectToCurrentUmbracoPage();
                }
            }

            // all good!, create the member
            try
            {
                var curPage = Umbraco.TypedContent(model.Cpid);
                var culture = curPage.GetCulture();
                var newMember = memberService.CreateMember(model.Username.Trim(), model.Email.Trim(), $"{model.Surname.Trim()} {model.Name.Trim()}", Constants.MemberAlias);
                if (newMember == null)
                {
                    ModelState.AddModelError("FormGenericError", "Κάποιο σοβαρό σφάλμα προεκυψε. Προσπαθήστε ξανά!");
                    LogHelper.Warn(typeof(AuthSurfaceController), $"Cannot create new Facebook member: {model.Email}");
                    return PartialView("Profile/RegisterFacebook", model);
                }
                // Membership properties
                newMember.SetValue("emailVerifyHash", "ok");
                newMember.SetValue("hasVerifiedEmail", true);
                newMember.IsApproved = true;
                newMember.SetValue("memberMobilePhone", model.MobilePhone);
                newMember.SetValue("facebookUserId", model.FacebookUserId);
                newMember.SetValue("memberisOver18", true);
                Services.MemberService.Save(newMember);
                Services.MemberService.SavePassword(newMember, model.FacebookUserId);
                
                // assign member to group in order to see profile page
                var memberRoles = Services.MemberService.GetAllRoles(newMember.Id);
                if (!memberRoles.Any(z => string.Compare(z, Constants.VerifiedMemberGroup, true) == 0))
                {
                    Services.MemberService.AssignRole(newMember.Id, Constants.VerifiedMemberGroup);
                }

                //Services.MemberService.Save(newMember);
                if (!Members.Login(newMember.Username, model.FacebookUserId))
                {
                    ModelState.AddModelError("FormGenericError", "Συνέβη κάποιο σοβαρό σφάλμα. Παρακαλώ προσπαθήστε ξάνα.");
                    LogHelper.Warn(typeof(AuthSurfaceController), $"Cannot Login Facebook new member: {model.Email}");
                    return PartialView("Profile/Register", model);
                }
                return GetLoggedInMember(newMember.Name, newMember.Id, curPage.Id.ToString(), Umbraco);
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(AuthSurfaceController), $"Error while registering Facebook member {model.Email}", ex);
                ModelState.AddModelError("FormGenericError", "Συνέβη κάποιο σοβαρό σφάλμα. Παρακαλώ προσπαθήστε ξάνα. Αλλιώς επικοινωνήστε με το support μας.");
                return PartialView("Profile/Register", model);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid || Members.IsLoggedIn())
            {
                return PartialView("Profile/Register", model);
            }

            var memberService = Services.MemberService;
            var checkMemberEmail = Services.MemberService.GetByEmail(model.Email);
            if (checkMemberEmail != null)
            {
                ModelState.AddModelError("FormGenericError", "Το email χρησιμοποιείται ήδη!");
                return PartialView("Profile/Register", model);
            }

            // check if username unique
            if (Services.MemberService.GetByUsername(model.Username.Trim().ToLower()) != null)
            {
                ModelState.AddModelError("FormGenericError", "Το username που επιλέξατε χρησιμοποιείται ήδη από άλλο μελος, παρακαλώ επιλέξτε ένα άλλο.");
                return PartialView("Profile/Register", model);
            }

            try
            {
                var tempGUID = Guid.NewGuid();

                var curPage = Umbraco.TypedContent(model.Cpid);
                var culture = curPage.GetCulture();
                var newMember = memberService.CreateMember(model.Username.Trim(), model.Email.Trim(), $"{model.Surname.Trim()} {model.Name.Trim()}", Constants.MemberAlias);
                if (newMember == null)
                {
                    ModelState.AddModelError("FormGenericError", "Κάποιο σοβαρό σφάλμα προέκυψε. Παρακαλώ προσπαθήστε ξανά!");
                    LogHelper.Warn(typeof(AuthSurfaceController), $"Cannot create new member: {model.Email}");
                    return PartialView("Profile/Register", model);
                }
                // Membership properties
                newMember.SetValue("hasVerifiedEmail", false);
                newMember.SetValue("emailVerifyHash", tempGUID.ToString());
                newMember.IsApproved = false;
                newMember.SetValue("memberisOver18", true);

                // Assign all properies
                newMember.SetValue("memberLanguage", culture.Name);

                Services.MemberService.Save(newMember);
                Services.MemberService.SavePassword(newMember, model.Password);
                model.MemberCreated = true;

                await EmailHelper.SendVerifyEmail(Umbraco, newMember, tempGUID.ToString());
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(AuthSurfaceController), $"Error while registering member {model.Email}", ex);
                ModelState.AddModelError("FormGenericError", "Some critical error occurred, try again.");
                return PartialView("Profile/Register", model);
            }

            return PartialView("Profile/Register", model);
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult VerifyEmail(string verifyGUID, int? wid)
        {
            if (string.IsNullOrEmpty(verifyGUID))
            {
                return Redirect("/");
            }

            var findMember = Services.MemberService.GetMembersByPropertyValue("emailVerifyHash", verifyGUID)?.FirstOrDefault();
            //Ensure we find a member with the verifyGUID
            if (findMember != null)
            {
                //We got the member, so let's update the verify email checkbox
                findMember.SetValue("emailVerifyHash", "ok");
                findMember.SetValue("hasVerifiedEmail", true);
                findMember.IsApproved = true;

                // assign member to group in order to see profile page
                var memberRoles = Services.MemberService.GetAllRoles(findMember.Id);
                if (!memberRoles.Any(z => string.Compare(z, Constants.VerifiedMemberGroup, true) == 0))
                {
                    Services.MemberService.AssignRole(findMember.Id, Constants.VerifiedMemberGroup);
                }

                Services.MemberService.Save(findMember);

                var home = Umbraco.TypedContentAtRoot().FirstOrDefault(z => z.Id == wid);
                if (home != null)
                {
                    var emailVerifiedPage = Umbraco.TypedContent(Constants.EmailVerifiedPageId);
                    return RedirectToUmbracoPage(emailVerifiedPage);
                }
            }
            return Redirect("/");
        }


        [HttpPost]
        public ActionResult IsLoggedIn()
        {
            return Json(Members.IsLoggedIn());
        }

        [HttpGet]
        public ActionResult HomeLoggedIn(string cpid)
        {
            var member = Members.GetCurrentMember();

            return member != null
                ? GetLoggedInMember(member.Name, member.Id, cpid, Umbraco)
                : null;
        }

        private JsonResult GetLoggedInMember(string memberName, int memberId, string cpid, UmbracoHelper umbracoHelper = null)
        {
            if (string.IsNullOrEmpty(memberName) || cpid == null || umbracoHelper == null)
            {
                return null;
            }
            var currentPage = Umbraco.TypedContent(cpid);
            //var wonParticipation = (await ParticipationHelper.Default.GetWonParticipationsRecent(memberId, umbracoHelper))?.FirstOrDefault();
            var profilePage = Umbraco.TypedContent(Constants.ProfilePage);
            return Json(new
            {
                Success = true,
                RedirectUrl = currentPage.Url,
                IsValidMember = true,
                Data = new LoggedInData
                {
                    MemberName = memberName,
                    //HasRecentWonCompetition = wonParticipation != null,
                    MemberProfileUrl = UmbracoContext.Current.UrlProvider.GetUrl(profilePage.Id)
                }
            }, JsonRequestBehavior.AllowGet);
        }
    }
}