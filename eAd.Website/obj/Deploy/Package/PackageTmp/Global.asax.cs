﻿using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using eAd.Website.Controllers;

namespace eAd.Website
{

    public class RootRouteConstraint<T> : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var rootMethodNames = typeof(T).GetMethods().Select(x => x.Name.ToLower());
            return rootMethodNames.Contains(values["action"].ToString().ToLower());
        }
    }
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }



        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        //    routes.MapRoute(
        //    "Init", // Route name
        //    "{controller}/{action}/{id}",        // URL with parameters
        //    new { controller = "Home", action = "Init", id = UrlParameter.Optional }  // Parameter defaults
        //    );

        //    routes.MapRoute(
        //    "Stop", // Route name
        //    "{controller}/{action}/{id}",        // URL with parameters
        //    new { controller = "Home", action = "Stop", id = UrlParameter.Optional }  // Parameter defaults
        //);

            //routes.MapRoute(
            //    "Default", // Route name
            //    "{controller}/{action}/{id}", // URL with parameters
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            //);

            routes.MapRoute("Root", "{action}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new { isMethodInHomeController = new RootRouteConstraint<HomeController>() });
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
                );          
       

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}