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

namespace Blog.App_Start
{
    public class Bootstrapper
    {
        public void Init()
        {
            RegisterTypes();
            InitDatabase();
        }

        public void RegisterTypes()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<ISecurityService, SecurityService>();
            unityContainer.RegisterType<ILogService, LogService>();
            unityContainer.RegisterType<ISettingsService, SettingsService>();
            unityContainer.RegisterType<ICategoriesService, CategoriesService>();
            unityContainer.RegisterType<IArticlesService, ArticlesService>();
            unityContainer.RegisterType<ITagsService, TagsService>();
            unityContainer.RegisterType<ICommentsService, CommentsService>();
            unityContainer.RegisterType<ISitesService, SitesService>();
            unityContainer.RegisterType<ISearchService, SearchService>();

            ControllerBuilder.Current.SetControllerFactory(new CustomControllerFactory(unityContainer));
        }

        public void InitDatabase()
        {
            var dbContext = new DAL.DatabaseContext();
            dbContext.Dispose();

            WebSecurity.InitializeDatabaseConnection("MainDBConnection", "Users", "ID", "Name", true);
            if (!Roles.RoleExists("Administrator"))
                Roles.CreateRole("Administrator");
            if (!WebSecurity.UserExists("AlfaLeporis"))
                WebSecurity.CreateUserAndAccount("AlfaLeporis", "password");
        }
    }
}