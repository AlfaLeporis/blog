using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.DAL;
using Blog.ViewModels;
using Blog.Models;
using Omu.ValueInjecter;
using Blog.App_Start;

namespace Blog.Services
{
    public class SitesService : ISitesService
    {
        public SitesService()
        {
            
        }

        public bool Add(SiteViewModel viewModel)
        {
            using (var db = new DatabaseContext())
            {
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
                var model = db.Sites.First(p => p.ID == viewModel.ID);
                model.InjectFrom(viewModel);

                db.SaveChanges();
            }

            return true;
        }

        public void Remove(int id)
        {
            using (var db = new DatabaseContext())
            {
                if (!db.Sites.Any(p => p.ID == id))
                    return;

                var element = db.Sites.First(p => p.ID == id);
                db.Sites.Remove(element);

                db.SaveChanges();
            }
        }

        public SiteViewModel Get(int id)
        {
            using (var db = new DatabaseContext())
            {
                var element = db.Sites.FirstOrDefault(p => p.ID == id);

                var viewModel = new SiteViewModel();
                viewModel.InjectFrom<CustomInjection>(element);

                return viewModel;
            }
        }

        public List<SiteViewModel> GetAll()
        {
            using (var db = new DatabaseContext())
            {
                var comments = db.Sites.ToList();
                var viewModels = new List<SiteViewModel>();

                for (int i = 0; i < comments.Count; i++)
                {
                    var vm = new SiteViewModel();
                    vm.InjectFrom<CustomInjection>(comments[i]);

                    viewModels.Add(vm);
                }

                return viewModels;
            }
        }


        public void SetSiteStatus(int id, bool status)
        {
            using (var db = new DatabaseContext())
            {
                var site = db.Sites.First(p => p.ID == id);
                site.IsPublished = status;
                db.SaveChanges();
            }
        }
    }
}