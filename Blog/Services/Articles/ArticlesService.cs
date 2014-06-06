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
        private ICategoriesService _categoriesService = null;
        private ICommentsService _commentsService = null;

        private const String _readMoreTag = "<!--more-->";

        public ArticlesService(ITagsService tagsService,
                               ICategoriesService categoriesService,
                               ICommentsService commentsService)
        {
            _tagsService = tagsService;
            _categoriesService = categoriesService;
            _commentsService = commentsService;
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

                _tagsService.Parse(viewModel.TagsString, model.ID);
            }

            return true;
        }

        public ViewModels.ArticleViewModel Get(int id)
        {
            using(var db = new DatabaseContext())
            {
                var model = db.Articles.FirstOrDefault(p => p.ID == id);

                if (model == null)
                    return null;

                var viewModel = new ArticleViewModel();
                viewModel.InjectFrom<CustomInjection>(model);
                viewModel.Tags = _tagsService.GetListByArticleID(model.ID);
                viewModel.TagsString = _tagsService.GetStringByArticleID(model.ID);
                viewModel.CategoryName = _categoriesService.Get(model.CategoryID).Title;
                viewModel.IsReadMode = model.Content.Contains(_readMoreTag);
                viewModel.Comments = _commentsService.GetByTargetID(id, Models.CommentTarget.Article);
                viewModel.IsReadMode = false;
                viewModel.CommentsView = false;
                
                return viewModel;
            }
        }

        public ArticleViewModel GetByAlias(String alias)
        {
            using (var db = new DatabaseContext())
            {
                var model = db.Articles.FirstOrDefault(p => p.Alias == alias);

                if (model == null)
                    return null;

                var viewModel = Get(model.ID);

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
                    viewModels.Add(Get(models[i].ID));
                }

                db.SaveChanges();

                return viewModels;
            }
        }

        public List<ViewModels.ArticleViewModel> GetByTagName(String tag)
        {
            using (var db = new DatabaseContext())
            {
                if (!db.Tags.Any(p => p.Name == tag))
                    return null;

                var articlesID = _tagsService.GetArticlesIDByTagName(tag);
                var articleViewModels = new List<ArticleViewModel>();

                for (int i = 0; i < articlesID.Count; i++)
                {
                    articleViewModels.Add(Get(articlesID[i]));
                }
                
                return articleViewModels;
            }
        }

        public List<ViewModels.ArticleViewModel> GetShortByTagName(String tag)
        {
            using (var db = new DatabaseContext())
            {
                if (!db.Tags.Any(p => p.Name == tag))
                    return null;

                var articlesID = _tagsService.GetArticlesIDByTagName(tag);
                var articleViewModels = new List<ArticleViewModel>();

                for (int i = 0; i < articlesID.Count; i++)
                {
                    articleViewModels.Add(GetShortVersion(articlesID[i]));
                }

                return articleViewModels;
            }
        }

        public bool Remove(int id)
        {
            using(var db = new DatabaseContext())
            {
                var model = db.Articles.FirstOrDefault(p => p.ID == id);

                if (model == null)
                    return false;

                db.Articles.Remove(model);
                _tagsService.RemoveByArticleID(id);

                db.SaveChanges();
            }

            return true;
        }

        public bool Edit(ViewModels.ArticleViewModel viewModel)
        {
            using(var db = new DatabaseContext())
            {
                var model = db.Articles.FirstOrDefault(p => p.ID == viewModel.ID);

                if (model == null)
                    return false;

                model.InjectFrom(viewModel);

                db.SaveChanges();

                _tagsService.Parse(viewModel.TagsString, model.ID);
            }

            return true;
        }

        public bool SetArticleStatus(int id, bool status)
        {
            using(var db = new DatabaseContext())
            {
                var model = db.Articles.FirstOrDefault(p => p.ID == id);

                if (model == null)
                    return false;

                model.IsPublished = status;
                db.SaveChanges();
            }

            return true;
        }


        public ArticleViewModel GetShortVersion(int id)
        {
            var viewModel = Get(id);

            if (viewModel == null)
                return null;

            viewModel.IsReadMode = viewModel.Content.Contains(_readMoreTag);

            if (viewModel.IsReadMode)
            {
                var readMorePosition = viewModel.Content.LastIndexOf(_readMoreTag);
                viewModel.Content = viewModel.Content.Remove(readMorePosition);
            }

            return viewModel;
        }

        public List<ArticleViewModel> GetAllShortVersion()
        {
            using (var db = new DatabaseContext())
            {
                var models = db.Articles.ToList();
                var viewModels = new List<ArticleViewModel>();

                for (int i = 0; i < models.Count; i++)
                {
                    viewModels.Add(GetShortVersion(models[i].ID));
                }

                db.SaveChanges();

                return viewModels;
            }
        }


        public List<ArticleViewModel> GetByCategoryName(string name)
        {
            using (var db = new DatabaseContext())
            {
                var categoryID = _categoriesService.GetIDByName(name);

                if (categoryID == -1)
                    return null;

                var articleViewModels = new List<ArticleViewModel>();
                var articlesID = db.Articles.Where(p => p.CategoryID == categoryID).Select(p => p.ID).ToList();

                for (int i = 0; i < articlesID.Count; i++)
                {
                    articleViewModels.Add(Get(articlesID[i]));
                }

                return articleViewModels;
            }
        }

        public List<ArticleViewModel> GetShortByCategoryName(string name)
        {
            using (var db = new DatabaseContext())
            {
                var categoryID = _categoriesService.GetIDByName(name);

                if (categoryID == -1)
                    return null;

                var articleViewModels = new List<ArticleViewModel>();
                var articlesID = db.Articles.Where(p => p.CategoryID == categoryID).Select(p => p.ID).ToList();

                for (int i = 0; i < articlesID.Count; i++)
                {
                    articleViewModels.Add(GetShortVersion(articlesID[i]));
                }

                return articleViewModels;
            }
        }
    }
}