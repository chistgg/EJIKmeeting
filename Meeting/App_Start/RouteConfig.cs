using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Meeting
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Users", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ChatRoom",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "MainChat", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Upload",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "MainChat", action = "Upload", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "Download",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "MainChat", action = "Download", id = UrlParameter.Optional }
           );

            routes.MapRoute(
            name: "PublicDownload",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Base", action = "Download", id = UrlParameter.Optional }
        );
        }
    }
}
