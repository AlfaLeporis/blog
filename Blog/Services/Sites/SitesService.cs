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
            if (_db.Set<SiteModel>().Any(p => p.Alias == viewModel.Alias))
                return false;

            var model = new SiteModel();
            model.InjectFrom<CustomInjection>(viewModel);
            model.LastUpdateDate = DateTime.Now;

            _db.Set<SiteModel>().Add(model);
            _db.SaveChanges();

            return true;
        }

        public bool Edit(SiteViewModel viewModel)
        {
            var model = _db.Set<SiteModel>().FirstOrDefault(p => p.ID == viewModel.ID);

            if (model == null)
                return false;

            model.LastUpdateDate = DateTime.Now;
            model.InjectFrom<CustomInjection>(viewModel);

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
            viewModel.Comments = _commentsService.GetByTargetID(model.ID, CommentTarget.Site);
            viewModel.IsReadMore = false;

            return viewModel;
        }

        public List<SiteViewModel> GetAll(ref PaginationSettings pagination)
        {
            if (pagination != null)
                pagination.TotalItems = _db.Set<ArticleModel>().Where(p => !p.IsRemoved).Count();

            var sites = _db.Set<SiteModel>()
                .Where(p => !p.IsRemoved)
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
    }
}