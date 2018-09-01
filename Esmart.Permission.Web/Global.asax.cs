using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;
using Esmart.Framework.Utility;
using Esmart.Framework;
using Esmart.Framework.Config;

namespace Esmart.Permission.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutoMapperConfig.Register();
            Esmart.Permission.Application.Startup.Configuration();
            FileWatchHelper.StartWatching(updateProcess, CachedConfigContext.Current.ConfigService.GetFileFolder(), Constants.SystemConfig); //监控CONFIGXMl
            log4net.Config.XmlConfigurator.Configure();//来源于自身WEBCONFIG section来扩展config
        }
        private void updateProcess(object o)
        {
        }
    }
}
