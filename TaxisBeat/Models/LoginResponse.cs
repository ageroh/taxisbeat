using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace TaxisBeat.Models
{
    public class LoginResponse
    {
        public string RedirectUrl { get; set; }
        public bool IsValidMember { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public LoggedInData Data { get; set; }

        public LoginResponse()
        {
            Success = false;
            IsValidMember = false;
            ErrorMessage = "No Error";
        }
    }

    public class LoggedInData
    {
        public string MemberName { get; set; }
        public int? TotalApplications { get; set; }
        public string MemberProfileUrl { get; set; }
    }
}