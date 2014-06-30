﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Filters
{
    public class ValidationModelState : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                filterContext.Result = new ViewResult()
                {
                    ViewData = filterContext.Controller.ViewData,
                    ViewName = filterContext.ActionDescriptor.ActionName,
                };        
            }

            base.OnActionExecuting(filterContext);
        }
    }
}