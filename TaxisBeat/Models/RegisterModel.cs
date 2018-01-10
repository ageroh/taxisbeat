using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Constants = TaxisBeat.Code.Constants;
using Foolproof;

namespace TaxisBeat.Models
{
    public class RegisterModel
    {
        [DisplayName("Όνομα")]
        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(Constants.NameLatinGreekRegex, ErrorMessage = "Check your name, it must consist only of latin/greek characters and dashes.")]
        public string Name { get; set; }

        [DisplayName("Επώνυμο")]
        [Required(ErrorMessage = "Το Επώνυμο είναι υποχρεωτικό")]
        [RegularExpression(Constants.SurnameLatinGreekRegex, ErrorMessage = "Check your Surname, it must consist only of latin/greek characters and dashes.")]
        public string Surname { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(Constants.EmailRegex, ErrorMessage = "Email is invalid.")]
        [Remote("CheckEmailIsUsed", "AuthSurface", ErrorMessage = "Η διευθυνσή email ήδη χρησιμοποιείται.")] 
        public string Email { get; set; }
        
        [DisplayName("Username")]
        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression(Constants.UsernameRegex, ErrorMessage = "Your username is invalid")]
        [Remote("CheckUsernamelIsUsed", "AuthSurface", ErrorMessage = "Το username ήδη χρησιμοποιείται.")] 
        public string Username { get; set; }

        [UIHint("Password")]
        [DisplayName("Κωδικός")]
        [Required(ErrorMessage = "O Κωδικός είναι υποχρεωτικός")]
        [StringLength(20, ErrorMessage = "Password must be at least 8 characters", MinimumLength = 8)]
        public string Password { get; set; }

        [UIHint("Password")]
        [DisplayName("Επιβεβαίωση Κωδικού")]
        [Required(ErrorMessage = "H επιβεβαίωση κωδικού είναι υποχρεωτική.")]
        [EqualTo("Password", ErrorMessage = "Pasword and Confirm Password must be equal")]
        [StringLength(20, ErrorMessage = "Password must be at least 8 characters", MinimumLength = 8)]
        public string ConfirmPassword { get; set; }
        
        public string Cpid { get; set; }
        public bool MemberCreated { get; set; }
        public AccountType AccountType { get; set; }
        // releated to Competitions properties here

        public RegisterModel()
        {
        }

        public RegisterModel(int cpid)
        {
            MemberCreated = false;
            Cpid = cpid.ToString();
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