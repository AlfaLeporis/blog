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
using Blog.Infrastructure;

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
            if (_db.Set<ArticleModel>().Any(p => p.Alias == viewModel.Alias && !p.IsRemoved))
                return false;

            var model = new ArticleModel();
            model.InjectFrom<CustomInjection>(viewModel);
            model.LastUpdateDate = DateTime.Now;
            model.IsRemoved = false;

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

            var viewModel = ConvertModel(model, shortVersion);
    
            return viewModel;
        }

        private ArticleViewModel ConvertModel(ArticleModel model, bool shortVersion)
        {
            var category = _categoriesService.Get(model.CategoryID);

            var viewModel = new ArticleViewModel();
            viewModel.InjectFrom<CustomInjection>(model);
            viewModel.Tags = _tagsService.GetListByArticleID(model.ID);
            viewModel.TagsString = _tagsService.GetStringByArticleID(model.ID);
            viewModel.CategoryName = category.Title;
            viewModel.CategoryAlias = category.Alias;
            viewModel.IsReadMode = shortVersion;
            viewModel.Comments = _commentsService.GetByTargetID(model.ID, Models.CommentTarget.Article);
            viewModel.CommentsView = !shortVersion;

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

        public ArticleViewModel GetByAlias(String alias, bool shortVersion)
        {
            var model = _db.Set<ArticleModel>().FirstOrDefault(p => p.Alias == alias);

            if (model == null)
                return null;

            var viewModel = ConvertModel(model, shortVersion);

            return viewModel;
        }

        public List<ViewModels.ArticleViewModel> GetAll(bool shortVersion, ref PaginationSettings pagination)
        {
            if (pagination != null)
                pagination.TotalItems = _db.Set<ArticleModel>().Where(p => !p.IsRemoved).Count();

            var models = _db.Set<ArticleModel>()
                .Where(p => !p.IsRemoved)
                .OrderBy(p => p.ID)
                .Paginate(pagination)
                .ToList();
            var viewModels = new List<ArticleViewModel>();
            
            for(int i=0; i<models.Count; i++)
            {
                viewModels.Add(ConvertModel(models[i], shortVersion));
            }

            return viewModels.OrderByDescending(p => p.PublishDate).ToList();
        }

        public List<ViewModels.ArticleViewModel> GetByTagName(String tag, bool shortVersion, ref PaginationSettings pagination)
        {
            var articlesID = _tagsService.GetArticlesIDByTagName(tag);
            var articleViewModels = new List<ArticleViewModel>();

            if (pagination != null)
                pagination.TotalItems = articlesID.Count;

            for (int i = 0; i < articlesID.Count; i++)
            {
                articleViewModels.Add(Get(articlesID[i], shortVersion));
            }

            return articleViewModels.OrderByDescending(p => p.PublishDate).AsQueryable().Paginate(pagination).ToList();
        }

        public bool Remove(int id)
        {
            var model = _db.Set<ArticleModel>().FirstOrDefault(p => p.ID == id);

            if (model == null)
                return false;

            model.IsRemoved = true;
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

        public List<ArticleViewModel> GetByCategoryAlias(string name, bool shortVersion, ref PaginationSettings pagination)
        {
            var categoryID = _categoriesService.GetIDByAlias(name);
            
            if (categoryID == -1)
                return null;

            if (pagination != null)
                pagination.TotalItems = _db.Set<ArticleModel>().Where(p => p.CategoryID == categoryID).Where(p => !p.IsRemoved).Count();

            var articleViewModels = new List<ArticleViewModel>();
            var articlesID = _db.Set<ArticleModel>()
                .Where(p => p.CategoryID == categoryID && !p.IsRemoved)
                .OrderBy(p => p.ID)
                .Paginate(pagination).ToList();

            for (int i = 0; i < articlesID.Count; i++)
            {
                articleViewModels.Add(ConvertModel(articlesID[i], shortVersion));
            }

            return articleViewModels.OrderByDescending(p => p.PublishDate).ToList();
        }


        public List<ArticleViewModel> GetByDate(String date, bool shortVersion, ref PaginationSettings pagination)
        {
            int year = Convert.ToInt32(date.Substring(0, 4));
            int month = Convert.ToInt32(date.Substring(4, 2));

            if (pagination != null)
                pagination.TotalItems = _db.Set<ArticleModel>().Where(
                    p => !p.IsRemoved && 
                    p.PublishDate.Month == month && 
                    p.PublishDate.Year == year)
                    .Count();

            var articles = _db.Set<ArticleModel>()
                .Where(p => 
                    !p.IsRemoved && 
                    p.PublishDate.Month == month && 
                    p.PublishDate.Year == year)
                .OrderBy(p => p.ID)
                .Paginate(pagination)
                .ToList();
            var viewModel = new List<ArticleViewModel>();
            
            for (int i = 0; i < articles.Count; i++)
            {
                var simpleArticle = ConvertModel(articles[i], true);
                viewModel.Add(simpleArticle);
            }

            return viewModel.OrderByDescending(p => p.PublishDate).ToList();
        }
    }
}