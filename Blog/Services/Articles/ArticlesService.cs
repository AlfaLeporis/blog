using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.DAL;
using Blog.ViewModels;
using Blog.Models;
using Omu.ValueInjecter;
using Blog.App_Start;
using System.Data.Entity;

namespace Blog.Services
{
    public class ArticlesService : IArticlesService
    {
        private ITagsService _tagsService = null;
        private ICategoriesService _categoriesService = null;
        private ICommentsService _commentsService = null;
        private DbContext _db = null;

        private const String _readMoreTag = "<!--more-->";

        public ArticlesService(ITagsService tagsService,
                               ICategoriesService categoriesService,
                               ICommentsService commentsService,
                               DbContext databaseContext)
        {
            _tagsService = tagsService;
            _categoriesService = categoriesService;
            _commentsService = commentsService;
            _db = databaseContext;
        }

        public bool Add(ViewModels.ArticleViewModel viewModel)
        {
            if (_db.Set<ArticleModel>().Any(p => p.Alias == viewModel.Alias))
                return false;

            var model = new ArticleModel();
            model.InjectFrom<CustomInjection>(viewModel);
            model.LastUpdateDate = DateTime.Now;

            _db.Set<ArticleModel>().Add(model);
            _db.SaveChanges();

            _tagsService.Parse(viewModel.TagsString, model.ID);

            return true;
        }

        public ViewModels.ArticleViewModel Get(int id, bool shortVersion)
        {
            var model = _db.Set<ArticleModel>().FirstOrDefault(p => p.ID == id);

            if (model == null)
                return null;

            var viewModel = ConvertModel(model);

            if (shortVersion)
            {
                viewModel.IsReadMode = viewModel.Content.Contains(_readMoreTag);

                if (viewModel.IsReadMode)
                {
                    var readMorePosition = viewModel.Content.LastIndexOf(_readMoreTag);
                    viewModel.Content = viewModel.Content.Remove(readMorePosition);
                }
            }
    
            return viewModel;
        }

        private ArticleViewModel ConvertModel(ArticleModel model)
        {
            var viewModel = new ArticleViewModel();
            viewModel.InjectFrom<CustomInjection>(model);
            viewModel.Tags = _tagsService.GetListByArticleID(model.ID);
            viewModel.TagsString = _tagsService.GetStringByArticleID(model.ID);
            viewModel.CategoryName = _categoriesService.Get(model.CategoryID).Title;
            viewModel.IsReadMode = model.Content.Contains(_readMoreTag);
            viewModel.Comments = _commentsService.GetByTargetID(model.ID, Models.CommentTarget.Article);
            viewModel.CommentsView = false;

            return viewModel;
        }

        public ArticleViewModel GetByAlias(String alias)
        {
            var model = _db.Set<ArticleModel>().FirstOrDefault(p => p.Alias == alias);

            if (model == null)
                return null;

            var viewModel = ConvertModel(model);

            return viewModel;
        }

        public List<ViewModels.ArticleViewModel> GetAll(bool shortVersion)
        {
            var models = _db.Set<ArticleModel>().Select(p => p.ID).ToList();
            var viewModels = new List<ArticleViewModel>();

            for(int i=0; i<models.Count; i++)
            {
                viewModels.Add(Get(models[i], shortVersion));
            }

            return viewModels.OrderByDescending(p => p.PublishDate).ToList();
        }

        public List<ViewModels.ArticleViewModel> GetByTagName(String tag, bool shortVersion)
        {
            var articlesID = _tagsService.GetArticlesIDByTagName(tag);
            var articleViewModels = new List<ArticleViewModel>();

            for (int i = 0; i < articlesID.Count; i++)
            {
                articleViewModels.Add(Get(articlesID[i], shortVersion));
            }

            return articleViewModels.OrderByDescending(p => p.PublishDate).ToList();
        }

        public bool Remove(int id)
        {
            var model = _db.Set<ArticleModel>().FirstOrDefault(p => p.ID == id);

            if (model == null)
                return false;

            _db.Set<ArticleModel>().Remove(model);
            _tagsService.RemoveByArticleID(id);

            _db.SaveChanges();
            
            return true;
        }

        public bool Edit(ViewModels.ArticleViewModel viewModel)
        {
            var model = _db.Set<ArticleModel>().FirstOrDefault(p => p.ID == viewModel.ID);

            if (model == null)
                return false;

            model.InjectFrom<CustomInjection>(viewModel);
            model.LastUpdateDate = DateTime.Now;

            _db.SaveChanges();

            _tagsService.Parse(viewModel.TagsString, model.ID);

            return true;
        }

        public bool SetArticleStatus(int id, bool status)
        {
            var model = _db.Set<ArticleModel>().FirstOrDefault(p => p.ID == id);

            if (model == null)
                return false;

            model.IsPublished = status;
            _db.SaveChanges();
            
            return true;
        }

        public List<ArticleViewModel> GetByCategoryName(string name, bool shortVersion)
        {
            var categoryID = _categoriesService.GetIDByName(name);

            if (categoryID == -1)
                return null;

            var articleViewModels = new List<ArticleViewModel>();
            var articlesID = _db.Set<ArticleModel>().Where(p => p.CategoryID == categoryID).Select(p => p.ID).ToList();

            for (int i = 0; i < articlesID.Count; i++)
            {
                articleViewModels.Add(Get(articlesID[i], shortVersion));
            }

            return articleViewModels.OrderByDescending(p => p.PublishDate).ToList();
        }


        public List<ArticleViewModel> GetByDate(String date)
        {
            var articles = _db.Set<ArticleModel>().ToList();
            var viewModel = new List<ArticleViewModel>();
            
            for (int i = 0; i < articles.Count; i++)
            {
                if (articles[i].PublishDate.ToString("yyyyMM") == date)
                {
                    var simpleArticle = Get(articles[i].ID, true);
                    viewModel.Add(simpleArticle);
                }
            }

            return viewModel.OrderByDescending(p => p.PublishDate).ToList();
        }
    }
}