using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DarkServer;

namespace DarkServerWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DarkServerManager.ApplicationStart(System.Configuration.ConfigurationManager.ConnectionStrings["DarkServerConnection"].ConnectionString,
                LoginApp.Browser);
        }

        protected void Session_Start()
        {
            Session.Add("DarkLoginSession", new DarkLoginSession(DetectDevice()));
        }

        protected void Session_End()
        {
            DarkLoginSession s = Session["DarkLoginSession"] as DarkLoginSession;
            s.Logoff();
        }

        private LoginDevice DetectDevice()
        {
            if (Request.Browser.IsMobileDevice)
            {
                return LoginDevice.Mobile;
            }
            else return LoginDevice.PC;
        }
    }
}
