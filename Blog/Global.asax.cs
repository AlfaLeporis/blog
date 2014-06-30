using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Blog.App_Start;
using Blog.Services;
using Blog.Filters;
using Microsoft.Practices.Unity;
using WebMatrix.WebData;
using System.Web.Security;
using NLog;

namespace Blog
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Init();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            var logger = LogManager.GetLogger("MainLogger");
            String log = String.Empty;
            log += "-------314159------------------Critical error handled------------------------------\r\n";
            log += "Message: " + exception.Message + Environment.NewLine;
            log += "Stacktrace: " + exception.StackTrace + Environment.NewLine;
            log += "----------------------------------------------------------------------------------\r\n";

            logger.Error(log);
        }
    }
}