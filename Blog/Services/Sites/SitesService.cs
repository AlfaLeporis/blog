using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.DAL;
using Blog.ViewModels;
using Blog.Models;
using Omu.ValueInjecter;
using Blog.App_Start;
using Blog.Services;
using System.Data.Entity;
using Blog.Infrastructure;

namespace Blog.Services
{
    public class SitesService : ISitesService
    {
        private ICommentsService _commentsService = null;
        private ISettingsService _settingsService = null;
        private DbContext _db = null;

        public SitesService(ICommentsService commentsService,
                            ISettingsService settingsService,
                            DbContext db)
        {
            _commentsService = commentsService;
            _settingsService = settingsService;
            _db = db;
        }

        public bool Add(SiteViewModel viewModel)
        {
            return Add(viewModel, 1, null);
        }

        public bool Add(SiteViewModel viewModel, int version, int? parent)
        {
            if (parent == null && _db.Set<SiteModel>().Any(p => p.Alias == viewModel.Alias && !p.IsRemoved))
                return false;

            var model = new SiteModel();
            model.InjectFrom<CustomInjection>(viewModel);
            model.LastUpdateDate = DateTime.Now;

            model.Version = version;
            model.Parent = parent;

            _db.Set<SiteModel>().Add(model);
            _db.SaveChanges();

            viewModel.ID = model.ID;

            return true;
        }

        public bool Edit(SiteViewModel viewModel)
        {
            var model = _db.Set<SiteModel>().FirstOrDefault(p => (p.ID == viewModel.ID || p.ID == viewModel.Parent)
                                                                                          && p.Parent == null);
            var lastVersion = _db.Set<SiteModel>().Where(p => p.Parent == model.ID)
                                                     .OrderByDescending(p => p.Version)
                                                     .FirstOrDefault();
            if (model == null)
                return false;

            int version = 0;
            if (lastVersion != null)
                version = lastVersion.Version + 1;
            else
                version = 1;

            var parentViewModel = ConvertModel(model);
            Add(parentViewModel, version, model.ID);

            var maxVersions = _settingsService.GetSettings().VersionsCount;
            var versionsList = _db.Set<SiteModel>().Where(p => p.Parent == model.ID).OrderBy(p => p.Version);
            var versionsCount = versionsList.Count();

            if (versionsCount > maxVersions)
            {
                var versionsToRemove = versionsList.Take(versionsCount - maxVersions).ToList();
                for (int i = 0; i < versionsToRemove.Count(); i++)
                {
                    versionsToRemove[i].Content = "";
                    versionsToRemove[i].IsRemoved = true;
                }
            }

            model.InjectFrom(new IgnoreProperties("ID", "Version", "Parent"), viewModel);
            model.LastUpdateDate = DateTime.Now;        

            _db.SaveChanges();

            return true;
        }

        public bool Remove(int id)
        {
            var element = _db.Set<SiteModel>().FirstOrDefault(p => p.ID == id);

            if (element == null)
                return false;

            element.IsRemoved = true;

            _db.SaveChanges();

            return true;
        }

        public SiteViewModel Get(int id, bool shortVersion)
        {
            var element = _db.Set<SiteModel>().FirstOrDefault(p => p.ID == id);

            if (element == null)
                return null;

            var viewModel = ConvertModel(element);

            if(shortVersion)
            {
                int contentLength = viewModel.Content.Length;
                int maxLength = _settingsService.GetSettings().ShortSiteMaxLength;

                if (contentLength > maxLength)
                    contentLength = maxLength;

                viewModel.Content = viewModel.Content.Remove(contentLength);
                viewModel.Content += "...";

                viewModel.IsReadMore = true;
            }

            return viewModel;
        }

        private SiteViewModel ConvertModel(SiteModel model)
        {
            var viewModel = new SiteViewModel();
            viewModel.InjectFrom<CustomInjection>(model);
            viewModel.IsReadMore = false;

            return viewModel;
        }

        public List<SiteViewModel> GetAll(ArticleSiteAccessSettings settings, ref PaginationSettings pagination)
        {
            IQueryable<SiteModel> queryable = _db.Set<SiteModel>();
            if (settings.Published)
                queryable = queryable.Where(p => p.IsPublished);
            if (!settings.Removed)
                queryable = queryable.Where(p => !p.IsRemoved);
            if (!settings.WithParent)
                queryable = queryable.Where(p => p.Parent == null);

            if (pagination != null)
                pagination.TotalItems = queryable.Count();

            var sites = queryable
                .OrderBy(p => p.ID)
                .Paginate(pagination)
                .ToList();

            var viewModels = new List<SiteViewModel>();

            for (int i = 0; i < sites.Count; i++)
            {
                viewModels.Add(ConvertModel(sites[i]));
            }

            return viewModels;
        }


        public bool SetSiteStatus(int id, bool status)
        {
            var site = _db.Set<SiteModel>().FirstOrDefault(p => p.ID == id);

            if (site == null)
                return false;

            site.IsPublished = status;
            _db.SaveChanges();

            return true;
        }


        public SiteViewModel GetByAlias(string id)
        {
            var site = _db.Set<SiteModel>().FirstOrDefault(p => p.Alias == id);

            if (site == null)
                return null;

            var viewModel = ConvertModel(site);

            return viewModel;
        }

        public List<SiteViewModel> GetVersionsByID(int siteID)
        {
            var viewModel = new List<SiteViewModel>();
            var siteVersions = _db.Set<SiteModel>().Where(p => p.Parent == siteID && !p.IsRemoved).ToList();

            for (int i = 0; i < siteVersions.Count; i++)
            {
                viewModel.Add(ConvertModel(siteVersions[i]));
            }

            return viewModel;
        }
    }
}