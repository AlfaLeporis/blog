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
            routes.IgnoreRoute("favicon.ico");

            var namesDic = new List<String[]>();
            namesDic.Add(new[]{"Articles", "Article", "Artykuł"});
            namesDic.Add(new[]{"Sites", "Site", "Strona"});
            namesDic.Add(new[]{"Categories", "Category", "Kategoria"});
            namesDic.Add(new[]{"Tags", "Tag", "Tag"});
            namesDic.Add(new[]{"Archive", "Archive", "Archiwum"});
            namesDic.Add(new[]{"Security", "Register", "Rejestracja" });
            namesDic.Add(new[]{"Modules", "Search", "Szukaj"});

            for (int i = 0; i < namesDic.Count; i++)
            {
                routes.MapRoute(
                    name: namesDic[i][0],
                    url: namesDic[i][2] + "/{id}",
                    defaults: new { controller = namesDic[i][0], action = namesDic[i][1], id = UrlParameter.Optional },
                    namespaces: new[] { "Blog.Controllers" }
                );
            }

            routes.MapRoute(
                name: "XMLMap",
                url: "sitemap.xml",
                defaults: new { controller = "Home", action = "GetSiteMap", id = UrlParameter.Optional },
                namespaces: new[] { "Blog.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Blog.Controllers" }
            );
        }
    }
}