using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Constants = TaxisBeat.Code.Constants;

namespace TaxisBeat.Models
{
    public class RegisterFacebookModel
    {

        [DisplayName("Name")]
        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(Constants.NameLatinGreekRegex, ErrorMessage = "Πρέπει να απότελείται μόνο από λατινικούς/ελληνικούς χαρακτήρες και παύλες.")]
        public string Name { get; set; }

        [DisplayName("Surname")]
        [Required(ErrorMessage = "Το Επώνυμο είναι υποχρεωτικό")]
        [RegularExpression(Constants.SurnameLatinGreekRegex, ErrorMessage = "Πρέπει να απότελείται μόνο από λατινικούς/ελληνικούς χαρακτήρες και παύλες.")]
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

        [DisplayName("Κινητό")]
        [RegularExpression(Constants.MobileNumber, ErrorMessage = "Το κινητό τηλέφωνο δεν είναι σωστό.")]
        public string MobilePhone { get; set; }

        public bool MemberCreated { get; set; }
        public AccountType AccountType { get; set; }

        // get a token just to verify, user is the same with the Cookie.
        public string AccessToken { get; set; }
        public string FacebookUserId { get; set; }
        public string Cpid { get; set; }
        
        public RegisterFacebookModel()
        {
        }

        public RegisterFacebookModel(int cpid)
        {
            MemberCreated = false;
            Cpid = cpid.ToString();
        }


    }
    
}