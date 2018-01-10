using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaxisBeat.Code
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class VATNumberAttribute : ValidationAttribute, IClientValidatable
    {
        public string ErrorMessageDictionaryKey { get; set; }
        public string DefaultTranslation { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var afm = Convert.ToString(value);
            if (!string.IsNullOrEmpty(afm) && !CheckAFM(afm))
            {
                //Get the error message to return
                var error = FormatErrorMessage(validationContext.DisplayName);

                //Return error
                return new ValidationResult(error);
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var error = FormatErrorMessage(metadata.DisplayName);
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = error,
                ValidationType = "afmvalidation"
            };

            yield return rule;

        }
        private bool CheckAFM(string afm)
        {
            if (afm.Length != 9)
                return false;
            var digits = afm.ToCharArray();
            int checkDigit = digits[8] - 48;
            long sum = ((digits[7] - 48) << 1) +
                ((digits[6] - 48) << 2) +
                ((digits[5] - 48) << 3) +
                ((digits[4] - 48) << 4) +
                ((digits[3] - 48) << 5) +
                ((digits[2] - 48) << 6) +
                ((digits[1] - 48) << 7) +
                ((digits[0] - 48) << 8);
            long mod = sum % 11;
            if (mod == 10)
                mod = 0;
            return (mod == checkDigit);
        }
    }
}