using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TaxisBeat.Code;
using Umbraco.Core.Models;
using Member = Umbraco.Web.PublishedContentModels.Member;

namespace TaxisBeat.Models
{
    public class ProfileViewModel
    {

        [DisplayName("Όνομα")]
        [RegularExpression(Constants.NameLatinGreekRegex, ErrorMessage = "Check your name, it must consist only of latin/greek characters and dashes.")]
        public string Name { get; set; }

        [DisplayName("Επώνυμο")]
        [RegularExpression(Constants.SurnameLatinGreekRegex, ErrorMessage = "Check your Surname, it must consist only of latin/greek characters and dashes.")]
        public string Surname { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Username")]
        public string Username { get; set; }

        [DisplayName("Κινητό")]
        [RegularExpression(Constants.MobileNumber, ErrorMessage = "Check your mobile phone, is invalid.")]
        public string MobilePhone { get; set; }
        
        [DisplayName("A.M.K.A")]
        [RegularExpression(Constants.AmkaNumber, ErrorMessage = "Το ΑΜΚΑ αποτελείται από 11 ψηφία (0 εως 9)")]
        public string AmkaNumber { get; set; }

        [DisplayName("Α.Δ.Τ")]
        [RegularExpression(Constants.GreekIdentityCard, ErrorMessage = "Ο Α.Δ.Τ αποτελείται από 1 με 2 ελληνικούς χαρακτήρες και 6 ψηφία.")]
        public string IdentityNumber { get; set; }

        // dieut, DOY, AFM
        [DisplayName("Διεύθυνση")]
        public string Address { get; set; }

        [DisplayName("Τ.Κ")]
        public int PostalCode { get; set; }

        [DisplayName("Α.Φ.Μ.")]
        [VATNumber(ErrorMessage = "Πρέπει να εισάγετε το έγκυρο ΑΦΜ σας.")]
        public string VatNumber { get; set; }

        public DoyModel DoyModel { get; set; }

        [DisplayName("Ημ/νια Γέννησης")]
        [DateOfBirth]
        public string DateOfBirthStr { get; set; }


        public DateTime? DateOfBirth
        {
            get
            {
                DateTime birthdate;
                if (DateTime.TryParseExact(Convert.ToString(DateOfBirthStr), DateOfBirthAttribute.DateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out birthdate))
                {
                    return birthdate;
                }
                return null;
            }
        }

        public IEnumerable<SelectListItem> Days { get { return Enumerable.Range(1, 31).Select(d => new SelectListItem() { Text = d.ToString("00"), Value = d.ToString("00") }); } }

        public IEnumerable<SelectListItem> Months { get { return Enumerable.Range(1, 12).Select(d => new SelectListItem() { Text = d.ToString("00"), Value = d.ToString("00") }); } }

        public IEnumerable<SelectListItem> Years
        {
            get
            {
                var limits = new DateOfBirthAttribute().GetLimits();
                var max = limits.MaxDateOfBirth.Year;
                var min = limits.MinDateOfBirth.Year;
                var diff = max - min + 1;
                return Enumerable.Range(min, diff).Select(d => new SelectListItem() { Text = d.ToString("0000"), Value = d.ToString("0000") }).Reverse();
            }
        }

        public bool HasPasswordChanged { get; set; }
        private IPublishedContent logedInMember;
        public bool Success { get; set; }
        

        public ProfileViewModel()
        {
            HasPasswordChanged = false;
            Success = false;
        }

        public ProfileViewModel(Member member)
        {
            this.logedInMember = member;
            var tpl = member.ExtractNameSurname();
            Surname = tpl.Surname;
            Name = tpl.Name; 
            Email = member.Email();
            MobilePhone = member.MemberMobilePhone;
            AmkaNumber = member.MemberAmkaNumber;
            IdentityNumber = member.MemberIdentityNumber;
            PostalCode = member.MemberPostalCode;
            VatNumber = member.MemberVatNumber;
            Address = member.MemberAddress;
            DateOfBirthStr = member.MemberDateOfBirth.ToString("dd/MM/yyyy");
            DoyModel = new DoyModel(member.MemberDoy);
            Username = member.UserName();
            HasPasswordChanged = false;
            Success = false;
        }

    }
}