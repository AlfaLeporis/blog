using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;

namespace Blog.Filters
{
    public class ErrorFilterAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            var logger = LogManager.GetLogger("MainLogger");

            String log = String.Empty;
            log += "Message: " + filterContext.Exception.Message + Environment.NewLine;
            log += "Stacktrace: " + filterContext.Exception.StackTrace + Environment.NewLine;

            logger.Error(log);
            
            base.OnException(filterContext);
        }
    }
}