using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Domain;

namespace WebPage
{
    public class MvcApplication : Spring.Web.Mvc.SpringMvcApplication//System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            //var db = new MyConfig().db;
            //Database.SetInitializer(new UserInitializer());
            //db.Database.Initialize(true);
        }
    }
}
