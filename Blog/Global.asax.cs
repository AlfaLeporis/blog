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

namespace Blog
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<ISecurityService, SecurityService>();
            unityContainer.RegisterType<ILogService, LogService>();
            unityContainer.RegisterType<ISettingsService, SettingsService>();
            unityContainer.RegisterType<ICategoriesService, CategoriesService>();
            unityContainer.RegisterType<IArticlesService, ArticlesService>();
            unityContainer.RegisterType<ITagsService, TagsService>();

            ControllerBuilder.Current.SetControllerFactory(new CustomControllerFactory(unityContainer));

            var dbContext = new DAL.DatabaseContext();
            dbContext.Dispose();

            WebSecurity.InitializeDatabaseConnection("MainDBConnection", "Users", "ID", "Name", true);
            if(!Roles.RoleExists("Administrator"))
                Roles.CreateRole("Administrator");
            if(!WebSecurity.UserExists("AlfaLeporis"))
                WebSecurity.CreateUserAndAccount("AlfaLeporis", "password");
        }
    }
}