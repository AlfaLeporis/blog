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
    public class ArticlesService : IArticlesService
    {
        private ITagsService _tagsService = null;

        public ArticlesService(ITagsService tagsService)
        {
            _tagsService = tagsService;
        }

        public bool Add(ViewModels.ArticleViewModel viewModel)
        {
            using(var db = new DatabaseContext())
            {
                if (db.Articles.Any(p => p.Alias == viewModel.Alias))
                    return false;

                var model = new ArticleModel();
                model.InjectFrom(viewModel);

                db.Articles.Add(model);
                db.SaveChanges();

                _tagsService.Parse(viewModel.Tags, model.ID);
            }

            return true;
        }

        public ViewModels.ArticleViewModel Get(int id)
        {
            using(var db = new DatabaseContext())
            {
                var model = db.Articles.FirstOrDefault(p => p.ID == id);

                var viewModel = new ArticleViewModel();
                viewModel.InjectFrom<CustomInjection>(model);

                return viewModel;
            }
        }

        public List<ViewModels.ArticleViewModel> GetAll()
        {
            using(var db = new DatabaseContext())
            {
                var models = db.Articles.ToList();
                var viewModels = new List<ArticleViewModel>();

                for(int i=0; i<models.Count; i++)
                {
                    var newViewModel = new ArticleViewModel();
                    newViewModel.InjectFrom<CustomInjection>(models[i]);

                    viewModels.Add(newViewModel);
                }

                db.SaveChanges();

                return viewModels;
            }
        }

        public void Remove(int id)
        {
            using(var db = new DatabaseContext())
            {
                var viewModel = db.Articles.First(p => p.ID == id);
                db.Articles.Remove(viewModel);

                _tagsService.RemoveByArticleID(id);

                db.SaveChanges();
            }
        }

        public bool Edit(ViewModels.ArticleViewModel viewModel)
        {
            using(var db = new DatabaseContext())
            {
                var element = db.Articles.First(p => p.ID == viewModel.ID);
                element.InjectFrom(viewModel);

                db.SaveChanges();
            }

            return true;
        }

        public void SetArticleStatus(int id, bool status)
        {
            using(var db = new DatabaseContext())
            {
                var article = db.Articles.First(p => p.ID == id);
                article.IsPublished = status;
                db.SaveChanges();
            }
        }
    }
}