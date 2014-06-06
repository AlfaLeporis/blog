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

namespace Blog.Services
{
    public class SitesService : ISitesService
    {
        private ICommentsService _commentsService = null;

        public SitesService(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        public bool Add(SiteViewModel viewModel)
        {
            using (var db = new DatabaseContext())
            {
                if (db.Sites.Any(p => p.Alias == viewModel.Alias))
                    return false;

                var model = new SiteModel();
                model.InjectFrom(viewModel);

                db.Sites.Add(model);
                db.SaveChanges();
            }

            return true;
        }

        public bool Edit(SiteViewModel viewModel)
        {
            using (var db = new DatabaseContext())
            {
                if (db.Sites.Any(p => p.Alias == viewModel.Alias))
                    return false;

                var model = db.Sites.FirstOrDefault(p => p.ID == viewModel.ID);

                if (model == null)
                    return false;

                model.InjectFrom(viewModel);

                db.SaveChanges();
            }

            return true;
        }

        public bool Remove(int id)
        {
            using (var db = new DatabaseContext())
            {
                var element = db.Sites.FirstOrDefault(p => p.ID == id);

                if (element == null)
                    return false;

                db.Sites.Remove(element);

                db.SaveChanges();
            }

            return true;
        }

        public SiteViewModel Get(int id)
        {
            using (var db = new DatabaseContext())
            {
                var element = db.Sites.FirstOrDefault(p => p.ID == id);

                if (element == null)
                    return null;

                var viewModel = new SiteViewModel();
                viewModel.InjectFrom<CustomInjection>(element);
                viewModel.Comments = _commentsService.GetByTargetID(id, CommentTarget.Site);
                viewModel.IsReadMore = false;

                return viewModel;
            }
        }

        public List<SiteViewModel> GetAll()
        {
            using (var db = new DatabaseContext())
            {
                var sites = db.Sites.ToList();
                var viewModels = new List<SiteViewModel>();

                for (int i = 0; i < sites.Count; i++)
                {
                    var vm = new SiteViewModel();
                    vm.InjectFrom<CustomInjection>(sites[i]);
                    vm.Comments = _commentsService.GetByTargetID(sites[i].ID, CommentTarget.Site);
                    vm.IsReadMore = false;

                    viewModels.Add(vm);
                }

                return viewModels;
            }
        }


        public bool SetSiteStatus(int id, bool status)
        {
            using (var db = new DatabaseContext())
            {
                var site = db.Sites.FirstOrDefault(p => p.ID == id);

                if (site == null)
                    return false;

                site.IsPublished = status;
                db.SaveChanges();
            }

            return true;
        }


        public SiteViewModel GetByAlias(string id)
        {
            using(var db = new DatabaseContext())
            {
                var site = db.Sites.FirstOrDefault(p => p.Alias == id);

                if (site == null)
                    return null;

                var viewModel = Get(site.ID);

                return viewModel;
            }
        }


        public SiteViewModel GetShortVersion(int id, int maxLength)
        {
            var viewModel = Get(id);

            if (viewModel == null)
                return null;

            int contentLength = viewModel.Content.Length;
            if (contentLength > maxLength)
                contentLength = maxLength;

            viewModel.Content = viewModel.Content.Remove(contentLength);
            viewModel.Content += "...";

            viewModel.IsReadMore = true;

            return viewModel;
        }
    }
}