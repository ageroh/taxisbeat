using System.Linq;
using System.Web.Optimization;
using Umbraco.Core.Logging;

namespace UmbracoCmsTicketMania.Code
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            
            bundles.Add(new StyleBundle("~/bundles/css")
                .Include(
                    "~/css/bootstrap.css",
                    "~/css/style.css",
                     "~/css/dark.css",
                     "~/css/colors.css",
                     "~/css/animate.css",
                     "~/css/magnific-popup.css",
                     "~/css/responsive.css",
                     "~/css/taxisbeat.css")
                     .Include("~/css/font-icons.css", new CssRewriteUrlTransform()));
   
            bundles.Add(new ScriptBundle("~/bundles/libs").Include(
                    "~/scripts/libs/jquery.js",
                    //"~/scripts/libs/bs-filestyle.js",
                    "~/scripts/libs/timepicker.js",
                    "~/scripts/libs/datepicker.js",
                    "~/scripts/libs/daterangepicker.js",
                    //"~/scripts/libs/jquery.cropit.js",
                    //"~/scripts/libs/jQuery.fakeScroll.js",
                    "~/scripts/libs/plugins.js",
                    "~/scripts/libs/functions.js",
                    "~/scripts/common.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/libs/moment")
                .IncludeDirectory("~/scripts/libs/moment", "*.js", true));
            
            bundles.Add(new ScriptBundle("~/bundles/validation").Include(
                    "~/scripts/libs/jquery.validate.js",
                    "~/scripts/libs/jquery.validate.unobtrusive.js"
                    //"~/scripts/libs/jquery.unobtrusive-ajax.js"
                ));
            
            bundles.Add(new ScriptBundle("~/bundles/taxisbeat").Include(
                    "~/scripts/login.js",
                    "~/scripts/register.js",
                    "~/scripts/profile.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/loginregister").Include(
                    "~/scripts/loginRegisterHandle.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/profileIndex").Include(
                    "~/scripts/profileIndex.js"
                ));

            LogHelper.Info<string>("All Bundles Loaded");
        }
    }
}