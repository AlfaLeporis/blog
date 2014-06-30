using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Blog
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var namesDic = new Dictionary<String, String>();
            namesDic.Add("Articles", "Article");
            namesDic.Add("Sites", "Site");
            namesDic.Add("Categories", "Category");
            namesDic.Add("Tags", "Tag");
            namesDic.Add("Archive", "Archive");
            namesDic.Add("Modules", "Search");

            for (int i = 0; i < namesDic.Count; i++)
            {
                routes.MapRoute(
                    name: namesDic.ElementAt(i).Key,
                    url: namesDic.ElementAt(i).Value + "/{id}",
                    defaults: new { controller = namesDic.ElementAt(i).Key, action = namesDic.ElementAt(i).Value, id = UrlParameter.Optional },
                    namespaces: new[] { "Blog.Controllers" }
                );
            }

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Blog.Controllers" }
            );
        }
    }
}