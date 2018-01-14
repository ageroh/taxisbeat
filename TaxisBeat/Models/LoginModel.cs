using System.ComponentModel.DataAnnotations;
using Umbraco.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace TaxisBeat.Models
{
    public class LoginModel : PostRedirectModel
    {
        [Required(ErrorMessage = "Το Email ειναι υποχρεωτικό")]
        [DisplayName("Email / Username")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ο Κωδικός ειναι απαραίτητος για να κάνετε Login!")]
        [DisplayName("Password")]
        public string Password { get; set; }

        public string AccessToken { get; set; }
        
        public string CurrentPageId { get; set; }
    }

    public class FacebookLoginDetail
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get { return $"{Name.ToLowerInvariant()}.{Surname.ToLowerInvariant()}"; } }
        public bool IsFacebookVerified { get; set; }
        public string AccessToken { get; set; }

        public FacebookLoginDetail(JObject obj, string accessToken)
        {
            if (obj == null) return;
            UserId = obj.Value<string>("id");
            AccessToken = accessToken;
            Email = obj.Value<string>("email");
            Name = obj.Value<string>("first_name");
            Surname = obj.Value<string>("last_name");
            IsFacebookVerified = obj.Value<bool>("verified");
        }
    }

    //Forgotten Password View Model
    public class ForgottenPasswordViewModel
    {
        [DisplayName("Email")]
        [Required(ErrorMessage = "Συμπληρώστε το email σας για να σας στείλουμε οδηγίες για τον νέο σας κωδικό.")]
        [EmailAddress(ErrorMessage = "Το email που δώσατε δεν αντιστοιχεί σε σωστό email format.")]
        public string EmailAddress { get; set; }
    }

    //Reset Password View Model
    public class ResetPasswordViewModel
    {
        [DisplayName("Email")]
        [Required(ErrorMessage = "Το Email είναι απαραίτητο για να ανακτήσετε τον κωδικό σας.")]
        [EmailAddress(ErrorMessage = "Το email που δώσατε δεν αντιστοιχεί σε σωστό email format.")]
        public string EmailAddress { get; set; }

        [UIHint("Password")]
        [DisplayName("Κωδικός")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [UIHint("Password")]
        [DisplayName("Επιβεβαίωση Κωδικού")]
        [Required(ErrorMessage = "Το πεδίο Επιβεβαίωση Κωδικού είναι υποχρεωτικό!")]
        //[Equals("Password", "Forms.ResetPassword.Equals", "Confirm Password and Password must be equal!")]
        public string ConfirmPassword { get; set; }

        public bool PasswordChangedOk { get; set; }
        public string ResetGUID { get; set; }
        public int Cpid { get; set; }
        
        public ResetPasswordViewModel()
        {

        }

        public ResetPasswordViewModel(int cpid)
        {
            PasswordChangedOk = false;
            Cpid = cpid;
        }
    }
}