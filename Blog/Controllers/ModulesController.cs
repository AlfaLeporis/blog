using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Blog.ViewModels;
using Blog.Services;

namespace Blog.Controllers
{
    public class ModulesController : Controller
    {
        private ISettingsService _settingsService = null;
        private ITagsService _tagsService = null;
        private ICategoriesService _categoriesService = null;
        private ISitesService _sitesService = null;
        private ISearchService _searchService = null;

        private const float _maxTagSize = 1.5f;
        private const float _minTagSize = 1.0f;

        public ModulesController(ISettingsService settingsService,
                                 ITagsService tagsService,
                                 ICategoriesService categoriesService,
                                 ISitesService sitesService,
                                 ISearchService searchService)
        {
            _settingsService = settingsService;
            _tagsService = tagsService;
            _categoriesService = categoriesService;
            _sitesService = sitesService;
            _searchService = searchService;
        }

        public ActionResult HeaderModule()
        {
            return PartialView("_HeaderModule", _settingsService.GetSettings());
        }

        public ActionResult TagsModule()
        {
            var viewModel = _tagsService.GetMostPopularTags(Convert.ToInt32(_settingsService.GetSettings().TagsCount));

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
            var viewModel = _sitesService.GetAll().Select(p => new SitesModuleViewModel() { Title = p.Title, Alias = p.Alias }).ToList();
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
            int maxSiteLength = Convert.ToInt32(_settingsService.GetSettings().ShortSiteMaxLength);
            var result = _searchService.Search(phrase, maxSiteLength);

            return View(result);
        }
	}
}