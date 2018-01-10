using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace TaxisBeat.Code
{

    public class DateOfBirthAttribute : ValidationAttribute, IClientValidatable
    {
        public const string DateFormat = "dd/MM/yyyy";

        public override bool IsValid(object value)
        {
            DateTime birthdate;
            if (!DateTime.TryParseExact(Convert.ToString(value), DateFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out birthdate))
            {
                return false;
            }
            var limits = GetLimits();
            return (birthdate >= limits.MinDateOfBirth) && (birthdate <= limits.MaxDateOfBirth);
        }

        private string _errorMessageResourceName;
        public override string FormatErrorMessage(string name)
        {
            _errorMessageResourceName = _errorMessageResourceName ?? ErrorMessageResourceName;
            ErrorMessage = "Μη αποδεκτή ημερομηνία γέννησης.";
            ErrorMessageResourceName = null;
            return base.FormatErrorMessage(name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var limits = GetLimits();

            yield return
                new DateOfBirthClientValidationRule(FormatErrorMessage(metadata.DisplayName),
                    limits.MinDateOfBirth.ToJsMillis(), limits.MaxDateOfBirth.ToJsMillis());
        }


        public Limits GetLimits()
        {
            return new Limits()
            {
                MinDateOfBirth = DateTime.UtcNow.AddYears(-100),
                MaxDateOfBirth = DateTime.UtcNow.AddYears(-18)
            };
        }


        public class Limits
        {
            public DateTime MinDateOfBirth;
            public DateTime MaxDateOfBirth;
        }

    }



    public class DateOfBirthClientValidationRule : ModelClientValidationRule
    {
        public DateOfBirthClientValidationRule(string errorMessage, long min, long max)
        {
            ErrorMessage = errorMessage;
            ValidationType = "dateofbirth";
            ValidationParameters.Add("min", min.ToString());
            ValidationParameters.Add("max", max.ToString());
        }
    }
}