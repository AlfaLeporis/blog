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

namespace Blog.App_Start
{
    public class Bootstrapper
    {
        public void Init()
        {
            RegisterTypes();
            InitDatabase();
            InitFiles();
        }

        public void RegisterTypes()
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
            unityContainer.RegisterType<IPaginationService, PaginationService>();
            unityContainer.RegisterInstance<ISettingsService>(new SettingsService());
            unityContainer.RegisterInstance<DbContext>(new DatabaseContext());

            ControllerBuilder.Current.SetControllerFactory(new CustomControllerFactory(unityContainer));
        }

        public void InitDatabase()
        {
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

        public void InitFiles()
        {
            var uploadedPath = HttpContext.Current.Server.MapPath("Uploaded");
            if (!Directory.Exists(uploadedPath))
                Directory.CreateDirectory(uploadedPath);
        }
    }
}