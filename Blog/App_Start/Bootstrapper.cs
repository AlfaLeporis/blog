using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Blog.Services;
using WebMatrix.WebData;
using System.Web.Security;
using Blog.DAL;
using System.IO;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using System.Data.Entity.Migrations;
using System.Net;

namespace Blog.App_Start
{
    public class Bootstrapper
    {
        public void Init()
        {
            var unityContainer = RegisterTypes();

            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters, unityContainer);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            System.Net.ServicePointManager.Expect100Continue = false;

            InitDatabase();
            InitFiles();
            InitCustomProviders(unityContainer);
        }

        private IUnityContainer RegisterTypes()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<ISecurityService, SecurityService>();
            unityContainer.RegisterType<ILogService, LogService>();
            unityContainer.RegisterType<ICategoriesService, CategoriesService>();
            unityContainer.RegisterType<IArticlesService, ArticlesService>();
            unityContainer.RegisterType<ITagsService, TagsService>();
            unityContainer.RegisterType<ICommentsService, CommentsService>();
            unityContainer.RegisterType<ISitesService, SitesService>();
            unityContainer.RegisterType<ISearchService, SearchService>();
            unityContainer.RegisterType<IFeedsService, FeedsService>();
            unityContainer.RegisterType<IBackupsService, BackupsService>();
            unityContainer.RegisterType<ISiteMapsService, SiteMapsService>();
            unityContainer.RegisterType<ICaptchaService, CaptchaService>();
            unityContainer.RegisterInstance<ISettingsService>(new SettingsService());
            unityContainer.RegisterInstance<DbContext>(new DatabaseContext());

            return unityContainer;
        }

        private void InitDatabase()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, Migrations.Configuration>());

            var dbContext = new DAL.DatabaseContext();
            dbContext.Dispose();

            WebSecurity.InitializeDatabaseConnection("MainDBConnection", "Users", "ID", "Name", true);

            var additionalInfo = new Dictionary<String, object>();
            additionalInfo.Add("EMail", "alfaleporis@gmail.com");
            additionalInfo.Add("WebSite", "http://www.google.pl");
            additionalInfo.Add("LastVisit", DateTime.Now);

            if (!Roles.RoleExists("Administrator"))
                Roles.CreateRole("Administrator");
            if (!WebSecurity.UserExists("AlfaLeporis"))
                WebSecurity.CreateUserAndAccount("AlfaLeporis", "password", additionalInfo);
            if(!Roles.IsUserInRole("AlfaLeporis", "Administrator"))
                Roles.AddUsersToRole(new[] { "AlfaLeporis" }, "Administrator");
        }

        private void InitFiles()
        {
            CreateDirIfNotExist("Uploaded",
                                "Backups",
                                "Backups\\tmp");
        }

        private void InitCustomProviders(IUnityContainer container)
        {
            ControllerBuilder.Current.SetControllerFactory(new CustomControllerFactory(container));

            IFilterProvider filterProvider = FilterProviders.Providers.Single(p => p is FilterAttributeFilterProvider);
            FilterProviders.Providers.Remove(filterProvider);

            var provider = new CustomFilterProvider(container);
            FilterProviders.Providers.Add(provider);
        }

        private void CreateDirIfNotExist(params String[] dirName)
        {
            for(int i=0; i<dirName.Length; i++)
            {
                var uploadedPath = HttpContext.Current.Server.MapPath(dirName.ElementAt(i));
                if (!Directory.Exists(uploadedPath))
                    Directory.CreateDirectory(uploadedPath);
            }
        }
    }
}