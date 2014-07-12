using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.ViewModels;
using Blog.Services;

namespace Blog.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ModulesController : Controller
    {
        private ISettingsService _settingsService = null;
        private IArticlesService _articlesService = null;
        private ISitesService _sitesService = null;
        private ICategoriesService _categoriesService = null;

        public ModulesController(ISettingsService settingsService,
                                 IArticlesService articlesService,
                                 ISitesService sitesService,
                                 ICategoriesService categoriesService)
        {
            _settingsService = settingsService;
            _articlesService = articlesService;
            _sitesService = sitesService;
            _categoriesService = categoriesService;
        }

        public ActionResult ArticleVersionsList(int id)
        {
            var viewModel = new VersionsViewModel();
            viewModel.Target = Models.TargetType.Article;

            var versionsViewModel = new List<SingleVersionViewModel>();
            var versions = _articlesService.GetVersionsByID(id);

            for(int i=0; i<versions.Count; i++)
            {
                var singleVersion = new SingleVersionViewModel();
                singleVersion.TargetID = versions[i].ID.Value;
                singleVersion.SaveDate = versions[i].LastUpdateDate;
                singleVersion.Version = versions[i].Version;
                versionsViewModel.Add(singleVersion);
            }

            viewModel.Versions = versionsViewModel.OrderByDescending(p => p.Version).ToList();
            return PartialView("_VersionsList", viewModel);
        }

        public ActionResult SitesVersionsList(int id)
        {
            var viewModel = new VersionsViewModel();
            viewModel.Target = Models.TargetType.Site;

            var versionsViewModel = new List<SingleVersionViewModel>();
            var versions = _sitesService.GetVersionsByID(id);

            for (int i = 0; i < versions.Count; i++)
            {
                var singleVersion = new SingleVersionViewModel();
                singleVersion.TargetID = versions[i].ID.Value;
                singleVersion.SaveDate = versions[i].LastUpdateDate;
                singleVersion.Version = versions[i].Version;
                versionsViewModel.Add(singleVersion);
            }

            viewModel.Versions = versionsViewModel.OrderByDescending(p => p.Version).ToList();
            return PartialView("_VersionsList", viewModel);
        }

        public ActionResult PartialCategoriesList(int? selectedCategory)
        {
            var categories = _categoriesService.GetAll();
            ViewBag.SelectedCategoryID = selectedCategory;
            return PartialView("_CategoriesList", categories);
        }
    }
}