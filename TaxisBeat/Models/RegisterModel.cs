using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Constants = TaxisBeat.Code.Constants;
using Foolproof;
using Umbraco.Core.Models;
using TaxisBeat.Code;

namespace TaxisBeat.Models
{
    public class RegisterModel
    {
        [DisplayName("Όνομα")]
        [Required(ErrorMessage = "Το Όνομα είναι υποχρεωτικό.")]
        [RegularExpression(Constants.NameLatinGreekRegex, ErrorMessage = "Πρέπει να απότελείται μόνο από λατινικούς/ελληνικούς χαρακτήρες και παύλες.")]
        public string Name { get; set; }

        [DisplayName("Επώνυμο")]
        [Required(ErrorMessage = "Το Επώνυμο είναι υποχρεωτικό")]
        [RegularExpression(Constants.SurnameLatinGreekRegex, ErrorMessage = " Πρέπει να απότελείται μόνο από λατινικούς/ελληνικούς χαρακτήρες και παύλες")]
        public string Surname { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Το Email είναι υποχρεωτικό")]
        [RegularExpression(Constants.EmailRegex, ErrorMessage = "Το Email δεν είναι σωστό")]
        [Remote("CheckEmailIsUsed", "AuthSurface", ErrorMessage = "Η διευθυνσή email ήδη χρησιμοποιείται.")] 
        public string Email { get; set; }
        
        [DisplayName("Username")]
        [Required(ErrorMessage = "Το Username είναι υποχρεωτικό")]
        [RegularExpression(Constants.UsernameRegex, ErrorMessage = "Το username δεν είναι σωστό")]
        [Remote("CheckUsernamelIsUsed", "AuthSurface", ErrorMessage = "Το username ήδη χρησιμοποιείται.")] 
        public string Username { get; set; }

        [UIHint("Password")]
        [DisplayName("Κωδικός")]
        [Required(ErrorMessage = "O Κωδικός είναι υποχρεωτικός")]
        [StringLength(20, ErrorMessage = "Ο κωδικός πρέπει να είναι τουλάχιστον 8 χαρακτήρες", MinimumLength = 8)]
        public string Password { get; set; }

        [UIHint("Password")]
        [DisplayName("Επιβεβαίωση Κωδικού")]
        [Required(ErrorMessage = "H επιβεβαίωση κωδικού είναι υποχρεωτική.")]
        [EqualTo("Password", ErrorMessage = "Ο Κωδικός και η Επιβεβαίωση Κωδικού πρέπει να ταιριάζουν!")]
        [StringLength(20, ErrorMessage = "Ο κωδικός πρέπει να είναι τουλάχιστον 8 χαρακτήρες", MinimumLength = 8)]
        public string ConfirmPassword { get; set; }

        [MustBeTrue(ErrorMessage = "Πρέπει να αποδεχθείτε τους όρους & προυποθέσεις για να κάνετε εγγραφή στο site.")]
        public bool AcceptedTerms { get; set; }

        public string Cpid { get; set; }
        public bool MemberCreated { get; set; }

        public string TermsAndConditionsUrl { get; set; }

        public AccountType AccountType { get; set; }
        // releated to Competitions properties here

        public RegisterModel()
        {
        }

        public RegisterModel(IPublishedContent termsPage, int cpid)
        {
            MemberCreated = false;
            Cpid = cpid.ToString();
            TermsAndConditionsUrl = termsPage?.Url;
        }
    }

    public enum AccountType
    {
        Normal = 0,
        Vip,
        Facebook,
        TempRegistration
    }
}