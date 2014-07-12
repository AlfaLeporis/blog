using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Filters
{
    public class BreadcrumbsFilter : ActionFilterAttribute
    {
        public String BreadcrumbsSection { get; set; }
        public String BreadcrumbsItem { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if(BreadcrumbsSection != null)
                filterContext.Controller.ViewBag.BreadcrumbsSection = BreadcrumbsSection;

            if(BreadcrumbsItem != null)
                filterContext.Controller.ViewBag.BreadcrumbsItem = BreadcrumbsItem;

            base.OnActionExecuted(filterContext);
        }
    }
}