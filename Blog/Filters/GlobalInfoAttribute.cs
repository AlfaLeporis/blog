using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using Blog.Services;
using Microsoft.Practices.Unity;

namespace Blog.Filters
{
    public class GlobalInfoAttribute : ActionFilterAttribute
    {
        private ISettingsService _settingsService { get; set; }

        public GlobalInfoAttribute(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var mainAssembly = Assembly.GetAssembly(this.GetType());
            var attributes = mainAssembly.GetCustomAttributesData();

            filterContext.Controller.ViewBag.CMSVersion = mainAssembly.GetName().Version.ToString(3);

            var name = (AssemblyTitleAttribute)mainAssembly.GetCustomAttribute(typeof(AssemblyTitleAttribute));
            filterContext.Controller.ViewBag.CMSTitle = name.Title;

            var copyright = (AssemblyCopyrightAttribute)mainAssembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute));
            filterContext.Controller.ViewBag.Copyright = copyright.Copyright;

            var settings = _settingsService.GetSettings();
            filterContext.Controller.ViewBag.SiteName = settings.Title;;
            filterContext.Controller.ViewBag.SiteDescription = settings.Description;;
            filterContext.Controller.ViewBag.SiteAuthor = settings.Author;
            filterContext.Controller.ViewBag.SiteTags = settings.Tags;

            base.OnActionExecuting(filterContext);
        }
    }
}