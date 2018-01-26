using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Foolproof;
using Umbraco.Core.Models;

namespace TaxisBeat.Models
{
    public class RegisterModel : BaseRegisterModel
    {
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
        
        public RegisterModel() 
            : base()
        {
        }

        public RegisterModel(IPublishedContent termsPage, int cpid) 
            : base(termsPage, cpid)
        {

        }
    }
}