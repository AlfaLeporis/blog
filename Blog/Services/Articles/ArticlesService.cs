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
        private ISettingsService _settingsService = null;
        private DbContext _db = null;

        private const String _readMoreTag = "<!--more-->";

        public ArticlesService(ITagsService tagsService,
                               ICategoriesService categoriesService,
                               ICommentsService commentsService,
                               ISettingsService settingsService,
                               DbContext databaseContext)
        {
            _tagsService = tagsService;
            _categoriesService = categoriesService;
            _commentsService = commentsService;
            _settingsService = settingsService;
            _db = databaseContext;
        }

        public bool Add(ViewModels.ArticleViewModel viewModel)
        {
            return Add(viewModel, 1, null);
        }

        public bool Add(ViewModels.ArticleViewModel viewModel, int version, int? parent)
        {
            if (parent == null && _db.Set<ArticleModel>().Any(p => p.Alias == viewModel.Alias && !p.IsRemoved))
                return false;

            var model = new ArticleModel();
            model.InjectFrom<CustomInjection>(viewModel);
            model.LastUpdateDate = DateTime.Now;
            model.IsRemoved = false;

            model.Version = version;
            model.Parent = parent;

            _db.Set<ArticleModel>().Add(model);
            _db.SaveChanges();

            viewModel.ID = model.ID;

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
            viewModel.CommentsView = !shortVersion;
            viewModel.CommentsCount = _commentsService.GetCommentsCountByTargetID(model.ID, TargetType.Article);

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

        public List<ViewModels.ArticleViewModel> GetAll(bool shortVersion, ArticleSiteAccessSettings settings, ref PaginationSettings pagination)
        {
            IQueryable<ArticleModel> queryable = _db.Set<ArticleModel>();
            if (settings.Published)
                queryable = queryable.Where(p => p.IsPublished);
            if (!settings.Removed)
                queryable = queryable.Where(p => !p.IsRemoved);
            if (!settings.WithParent)
                queryable = queryable.Where(p => p.Parent == null);

            if (pagination != null)
                pagination.TotalItems = queryable.Count();

            var models = queryable
                .OrderByDescending(p => p.PublishDate)
                .Paginate(pagination)
                .ToList();
            var viewModels = new List<ArticleViewModel>();
            
            for(int i=0; i<models.Count; i++)
            {
                viewModels.Add(ConvertModel(models[i], shortVersion));
            }

            return viewModels.ToList();
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
            var model = _db.Set<ArticleModel>().FirstOrDefault(p => (p.ID == viewModel.ID || p.ID == viewModel.Parent) 
                                                                                          && p.Parent == null);
            var lastVersion = _db.Set<ArticleModel>().Where(p => p.Parent == model.ID)
                                                     .OrderByDescending(p => p.Version)
                                                     .FirstOrDefault();
            if (model == null)
                return false;

            int version = 0;
            if (lastVersion != null)
                version = lastVersion.Version + 1;
            else
                version = 1;

            var parentViewModel = ConvertModel(model, false);
            Add(parentViewModel, version, model.ID);

            var maxVersions = _settingsService.GetSettings().VersionsCount;
            var versionsList = _db.Set<ArticleModel>().Where(p => p.Parent == model.ID).OrderBy(p => p.Version);
            var versionsCount = versionsList.Count();

            if(versionsCount > maxVersions)
            {
                var versionsToRemove = versionsList.Take(versionsCount - maxVersions).ToList();
                for(int i=0; i<versionsToRemove.Count(); i++)
                {
                    versionsToRemove[i].Content = "";
                    versionsToRemove[i].IsRemoved = true;
                }
            }

            model.InjectFrom(new IgnoreProperties("ID", "Version", "Parent"), viewModel);
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

            var queryable = _db.Set<ArticleModel>().Where(p => p.CategoryID == categoryID && p.IsPublished && !p.IsRemoved && p.Parent == null);

            if (pagination != null)
                pagination.TotalItems = queryable.Count();

            var articleViewModels = new List<ArticleViewModel>();
            var articlesID = queryable
                .OrderByDescending(p => p.PublishDate)
                .Paginate(pagination).ToList();

            for (int i = 0; i < articlesID.Count; i++)
            {
                articleViewModels.Add(ConvertModel(articlesID[i], shortVersion));
            }

            return articleViewModels.ToList();
        }


        public List<ArticleViewModel> GetByDate(String date, bool shortVersion, ref PaginationSettings pagination)
        {
            int year = Convert.ToInt32(date.Substring(0, 4));
            int month = Convert.ToInt32(date.Substring(4, 2));

            var queryable = _db.Set<ArticleModel>().Where(p => !p.IsRemoved && p.IsPublished && 
                                                          p.PublishDate.Month == month && p.PublishDate.Year == year &&
                                                          p.Parent == null);

            if (pagination != null)
                pagination.TotalItems = queryable.Count();

            var articles = queryable
                .OrderByDescending(p => p.PublishDate)
                .Paginate(pagination)
                .ToList();

            var viewModel = new List<ArticleViewModel>();
            for (int i = 0; i < articles.Count; i++)
            {
                var simpleArticle = ConvertModel(articles[i], true);
                viewModel.Add(simpleArticle);
            }

            return viewModel.ToList();
        }


        public List<ArticleViewModel> GetVersionsByID(int articleID)
        {
            var viewModel = new List<ArticleViewModel>();
            var articleVersions = _db.Set<ArticleModel>().Where(p => p.Parent == articleID && !p.IsRemoved).ToList();

            for(int i=0; i<articleVersions.Count; i++)
            {
                viewModel.Add(ConvertModel(articleVersions[i], false));
            }

            return viewModel;
        }
    }
}