using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Foolproof;
using Umbraco.Core.Models;

namespace TaxisBeat.Models
{
    public class RegisterFacebookModel : BaseRegisterModel
    {
        public string AccessToken { get; set; }
        public string FacebookUserId { get; set; }

        public RegisterFacebookModel() 
            : base()
        {
        }

        public RegisterFacebookModel(IPublishedContent termsPage, int cpid) 
            : base(termsPage, cpid)
        {

        }
    }
}