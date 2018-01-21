using System;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Core.Models;
using TaxisBeat.Models;
using System.Linq;
using Member = Umbraco.Web.PublishedContentModels.Member;

namespace TaxisBeat.Controllers
{
    public class ProfileSurfaceController : SurfaceController
    {

        [HttpGet]
        [Authorize]
        public new ActionResult Profile(string cpid)
        {
            var logedInMember = Members.GetCurrentMember() as Member;
            var currentPageId = -1;
            int.TryParse(cpid, out currentPageId);
            if (logedInMember == null)
            {
                var currentPage = Umbraco.TypedContent(currentPageId);
                return Redirect(UmbracoContext.Current.UrlProvider.GetUrl(currentPage.Parent.Id));
            }
            return PartialView("Profile/Index", new ProfileViewModel(logedInMember));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Update(ProfileViewModel model)
        {

            var currentMember = Members.GetCurrentMember();
            if (currentMember == null)
            {
                return Redirect(UmbracoContext.Current.UrlProvider.GetUrl(CurrentPage.Parent.Id));
            }

            var member = Services.MemberService.GetById(currentMember.Id);
            PersistProfileModel(model, member);

            if (!ModelState.IsValid)
            {
                return PartialView("Profile/Index", model);
            }

            // check if Amka is taken by other member.
            if (!string.IsNullOrEmpty(model.AmkaNumber) 
                && Services.MemberService.GetMembersByPropertyValue("memberAmkaNumber", model.AmkaNumber?.Trim()).Any(z => z.Id != member.Id))
            {
                ModelState.AddModelError("FormGenericError", "Check if your AMKA is valid. Another member is already using it.");
                Logger.Error(typeof(ProfileSurfaceController), 
                    $"Member {member.Name}, {member.Id} try to use others AMKA number while update his profile.", new InvalidOperationException("Fraud"));
                return PartialView("Profile/Index", model);
            }

            // check if Amka is taken by other member.
            if (!string.IsNullOrEmpty(model.IdentityNumber) 
                && Services.MemberService.GetMembersByPropertyValue("memberIdentityNumber", model.IdentityNumber?.Trim()).Any(z => z.Id != member.Id))
            {
                ModelState.AddModelError("FormGenericError", "Check if your Identity is valid. Another member is already using it!");
                Logger.Error(typeof(ProfileSurfaceController), 
                    $"Member {member.Name}, {member.Id} try to use others IDENTITY number while update his profile.", new InvalidOperationException("Fraud"));
                return PartialView("Profile/Index", model);
            }

            // maybe if all data are the do, not update information of member.
            member.Name = $"{model.Surname} {model.Name}";
            member.SetValue("memberMobilePhone", model.MobilePhone);
            member.SetValue("memberDoy", model.DoyModel?.Doy);
            member.SetValue("memberAmkaNumber", model.AmkaNumber);
            member.SetValue("memberIdentityNumber", model.IdentityNumber);
            member.SetValue("memberDateOfBirth", model.DateOfBirth.Value);
            member.SetValue("memberMobilePhone", model.MobilePhone);
            member.SetValue("memberPostalCode", model.PostalCode);
            member.SetValue("memberVatNumber", model.VatNumber);
            member.SetValue("memberAddress", model.Address);

            Services.MemberService.Save(member);

            //// check if new password is set.
            //if (!string.IsNullOrWhiteSpace(model.Password) && !string.IsNullOrWhiteSpace(model.ConfirmPassword))
            //{
            //    if (string.Compare(model.Password.Trim(), model.ConfirmPassword.Trim()) == 0 
            //        && model.Password.Trim().Length >= 8 && model.ConfirmPassword.Trim().Length >= 8)
            //    {
            //        // good passwords, save them.
            //        Services.MemberService.SavePassword(member, model.ConfirmPassword.Trim());
            //        model.HasPasswordChanged = true;
            //        var lang = CurrentPage.GetCulture()?.Name;
            //        var website = CurrentPage?.Ancestors<Website>(1)?.First();
            //        await EmailHelper.PasswordChangedSuccessfully(currentMember, website, lang);
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("FormGenericError", u.s("Forms.Profile.PasswordChangingRules", "Your password must be the same as confirm password, and at least 10 characters long!"));
            //        return PartialView("Profile/Index", model);
            //    }
            //}
            //else if ((string.IsNullOrWhiteSpace(model.Password) && !string.IsNullOrWhiteSpace(model.ConfirmPassword))
            //    || (!string.IsNullOrWhiteSpace(model.Password) && string.IsNullOrWhiteSpace(model.ConfirmPassword)))
            //{
            //    ModelState.AddModelError("FormGenericError", u.s("Forms.Profile.PasswordChangingRules", "Your password must be the same as confirm password, and at least 10 characters long!"));
            //    return PartialView("Profile/Index", model);
            //}

            model.Success = true;
            return PartialView("Profile/Index", model);
        }
        


        //REMOTE Validation
        [Authorize]
        public JsonResult CheckEmailIsUsed(string email)
        {
            if(string.IsNullOrEmpty(email))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            var currentMember = Members.GetCurrentMember();
            var checkEmail = Members.GetByEmail(email);

            if (checkEmail != null && checkEmail.Id != currentMember.Id)
            {
                return Json($"The email {email} is already in use", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private static void PersistProfileModel(ProfileViewModel model, IMember member)
        {
            if(!string.IsNullOrEmpty(member.GetValue<string>("memberAmkaNumber")))
            {
                model.AmkaNumber = member.GetValue<string>("memberAmkaNumber");
            }
            if (!string.IsNullOrEmpty(member.GetValue<string>("memberIdentityNumber")))
            {
                model.IdentityNumber = member.GetValue<string>("memberIdentityNumber");
            }
            model.Email = member.Email;
            model.Username = member.Username;
            //model.DateOfBirthStr = member.GetValue<DateTime>("memberDateOfBirth").ToString("dd/MM/yyyy");
        }
    }
}