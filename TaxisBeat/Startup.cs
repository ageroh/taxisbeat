using Microsoft.Owin;
using Owin;
using Umbraco.Web;
using TaxisBeat.Code;
using UmbracoCmsTicketMania.Code;
using System.Web.Optimization;

[assembly: OwinStartup(typeof(TaxisBeat.Startup))]
namespace TaxisBeat
{
    public class Startup : UmbracoDefaultOwinStartup
    {
        public override void Configuration(IAppBuilder app)
        {
            base.Configuration(app);
            
            // Bundle configuration
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Telemetry Configuration
            //TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["AiInstrumentationKey"];
        }
    }
}