using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Filters
{
    public class ReturnUrlAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(filterContext.RouteData.Values.Any(p => p.Key == "returnUrl"))
            {
                filterContext.Controller.ViewBag.ReturnUrl = filterContext.RouteData.Values["returnUrl"];
            }
            else
            {
                if (filterContext.HttpContext.Request.UrlReferrer == null)
                    filterContext.Controller.ViewBag.ReturnUrl =  filterContext.HttpContext.Request.Url;
                else
                    filterContext.Controller.ViewBag.ReturnUrl = filterContext.HttpContext.Request.UrlReferrer;

                filterContext.RouteData.Values.Add("returnUrl", filterContext.Controller.ViewBag.ReturnUrl);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}