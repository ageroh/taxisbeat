﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TaxisBeat.Code
{
    public class MustBeTrueAttribute : ValidationAttribute, IClientValidatable
    {   
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Convert the value to a bool & check it's true
            if ((bool)value)
            {
                //All good :)
                return ValidationResult.Success;
            }

            //Get the error message to return
            var error = FormatErrorMessage(validationContext.DisplayName);

            //Return error
            return new ValidationResult(error);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var error   = FormatErrorMessage(metadata.DisplayName);
            var rule    = new ModelClientValidationRule
            {
                ErrorMessage    = error,
                ValidationType  = "mustbetrue"
            };

            yield return rule;
        }

    }
}
