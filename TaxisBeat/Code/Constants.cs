using System;

namespace TaxisBeat.Code
{
    public class Constants
    {
        public const string HomeTemplateAlias = "Home";
        public const string ProfileTemplateAlias = "Profile";
        public const string MemberAlias = "Member";

        //Email Templates
        public const string PasswordChangedSuccessfullyTemplate = "PasswordChangedSuccessfully";
        public const string ResetPasswordEmailTemplate = "ResetPasswordEmail";
        public const string VerifyEmailNewMemberTemplate = "VerifyEmailTemplate";
        public const string ContactUsRequestTemplate = "ContactUsRequest";
        
        // Alias
        public const string EmailTemplateRepo = "emailTemplateRepository";
        public const string DoyDropDownAlias = "memberDoy";

        // Regex
        public const string MobileNumber = "^(?:\\+\\d{8,15}|00\\d{8,15}|69\\d{8})$";
        public const string UsernameRegex = "^[a-zA-Z\\d._-]+$";
        public const string NameLatinGreekRegex = "^([a-zA-Z0-9Ά-ωΑ-ώ-]+(-[a-zA-Z0-9Ά-ωΑ-ώ]+){0,1}){1,60}$";
        public const string SurnameLatinGreekRegex = "^([a-zA-Z0-9Ά-ωΑ-ώ-]+){1,60}$";
        public const string GreekIdentityCard = "^[Α-Ω]{1,2}[0-9]{6}$";
        public const string EmailRegex = @"(?:[a-z0-9!#$%&'*+=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])";
        public const string AmkaNumber = "(^$)|(^\\d{10,11}$)";
        public const string AmkaNumberRequired = "^\\d{10,11}$";

        // Member Groups
        public const string VerifiedMemberGroup = "Verified";
        
        // Pages
        public const int HomePageId = 1094;
        public const int EmailVerifiedPageId = 1151;
        public const int ResetPasswordPageId = 1152;
        public const int TermAndConditionPage = 1154;

        // Cache Keys
        public const string CacheKey_GetDOYs = "Cache_GetDoys_";

    }
}