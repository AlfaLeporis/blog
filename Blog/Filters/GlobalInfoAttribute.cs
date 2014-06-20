using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;

namespace Blog.Filters
{
    public class GlobalInfoAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var mainAssembly = Assembly.GetAssembly(this.GetType());
            var attributes = mainAssembly.GetCustomAttributesData();

            filterContext.Controller.ViewBag.CMSVersion = mainAssembly.GetName().Version.ToString(3);

            var name = (AssemblyTitleAttribute)mainAssembly.GetCustomAttribute(typeof(AssemblyTitleAttribute));
            filterContext.Controller.ViewBag.CMSTitle = name.Title;

            var copyright = (AssemblyCopyrightAttribute)mainAssembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute));
            filterContext.Controller.ViewBag.Copyright = copyright.Copyright;

            base.OnActionExecuting(filterContext);
        }
    }
}