using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Blog.ViewModels;
using Blog.Services;
using Blog.Infrastructure;

namespace Blog.Controllers
{
    public class ModulesController : Controller
    {
        private ISettingsService _settingsService = null;
        private ITagsService _tagsService = null;
        private ICategoriesService _categoriesService = null;
        private ISitesService _sitesService = null;
        private ISearchService _searchService = null;
        private ICommentsService _commentsService = null;
        private IArticlesService _articlesService = null;

        private const float _maxTagSize = 1.5f;
        private const float _minTagSize = 1.0f;

        public ModulesController(ISettingsService settingsService,
                                 ITagsService tagsService,
                                 ICategoriesService categoriesService,
                                 ISitesService sitesService,
                                 ISearchService searchService,
                                 ICommentsService commentsService,
                                 IArticlesService articlesService)
        {
            _settingsService = settingsService;
            _tagsService = tagsService;
            _categoriesService = categoriesService;
            _sitesService = sitesService;
            _searchService = searchService;
            _commentsService = commentsService;
            _articlesService = articlesService;
        }

        public ActionResult HeaderModule()
        {
            return PartialView("_HeaderModule", _settingsService.GetSettings());
        }

        public ActionResult TagsModule()
        {
            var viewModel = _tagsService.GetMostPopularTags(_settingsService.GetSettings().TagsCount);

            float delta = _maxTagSize - _minTagSize;
            int maxCount = viewModel.Count == 0 ? 0 : viewModel.Max(p => p.Count);
            int minCount = viewModel.Count == 0 ? 0 : viewModel.Min(p => p.Count);

            for (int i = 0; i < viewModel.Count; i++)
            {
                float deltaSize = (100 * viewModel[i].Count) / maxCount;
                deltaSize *= delta / 100;

                viewModel[i].Size = _minTagSize + deltaSize;
            }

            return PartialView("_TagsModule", viewModel);
        }

        public ActionResult CategoriesModule()
        {
            return PartialView("_CategoriesModule", _categoriesService.GetAllWithDetails());
        }

        public ActionResult SitesModule()
        {
            PaginationSettings pagination = null;
            var viewModel = _sitesService.GetAll(ref pagination).
                Select(p => new SitesModuleViewModel() { Title = p.Title, Alias = p.Alias }).
                ToList();
            return PartialView("_SitesModule", viewModel);
        }

        public ActionResult SearchModule()
        {
            return PartialView("_SearchModule");
        }

        [HttpPost]
        public ActionResult Search(FormCollection viewModel)
        {
            var phrase = viewModel["search-phrase"];
            if(phrase.Length == 0)
            {
                TempData.Add("ErrorMsg", "Do pola wyszukiwarki musi być wpisana fraza.");
                return RedirectToAction("Index", "Home");
            }

            int maxSiteLength = _settingsService.GetSettings().ShortSiteMaxLength;
            var result = _searchService.Search(phrase, maxSiteLength);

            return View(result);
        }

        public ActionResult RecentCommentsModule()
        {
            int commentsCount = _settingsService.GetSettings().RecentCommentsCount;
            int commentLength = _settingsService.GetSettings().ShortCommentMaxLength;

            var viewModel = new List<RecentCommentsModuleViewModel>();
            var recentComments = _commentsService.GetRecentComments(commentsCount);

            for (int i = 0; i < recentComments.Count; i++)
            {
                var article = _articlesService.Get(recentComments[i].ArticleID, false);
                var comment = recentComments[i];

                var length = comment.Content.Length;

                if(comment.Content.Length > commentLength)
                    comment.Content = comment.Content.Remove(commentLength).Trim();

                comment.Content += "...";

                viewModel.Add(new RecentCommentsModuleViewModel()
                    {
                        Comment = recentComments[i],
                        Article = article
                    });
            }

            return PartialView("_RecentCommentsModule", viewModel);
        }

        public ActionResult ArchiveModule()
        {
            PaginationSettings pagination = null;
            var articles = _articlesService.GetAll(false, ref pagination);
            var viewModel = new List<ArchiveModuleViewModel>();

            for (int i = 0; i < articles.Count; i++)
            {
                var singleArchive = viewModel.FirstOrDefault(p => p.Date.ToString("yyyyMM") == articles[i].PublishDate.ToString("yyyyMM"));
                if (singleArchive == null)
                {
                    var newArchive = new ArchiveModuleViewModel()
                    {
                        Count = 1,
                        Date = articles[i].PublishDate
                    };

                    viewModel.Add(newArchive);
                }
                else
                {
                    singleArchive.Count++;
                }
            }

            return PartialView("_ArchiveModule", viewModel);
        }
	}
}