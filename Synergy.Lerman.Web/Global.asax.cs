using Synergy.Lerman.Controllers;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Synergy.Lerman.Realm.Books;

namespace Synergy.Lerman
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var filesPath = this.Server.MapPath("~\\App_Data");
            BookStore.Read(filesPath);
        }
    }
}
