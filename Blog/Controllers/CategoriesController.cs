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
    public class CategoriesController : Controller
    {
        private IArticlesService _articlesService = null;
        private ISettingsService _settingsService = null;

        public CategoriesController(IArticlesService articlesService,
                                 ISettingsService settingsService)
        {
            _articlesService = articlesService;
            _settingsService = settingsService;
        }

        [HttpGet]
        public ActionResult Category(String id, int? page)
        {
            if (!page.HasValue)
                page = 1;

            int pageSize = _settingsService.GetSettings().ItemsPerPage;
            PaginationSettings pagination = new PaginationSettings(page.Value, pageSize);
            var articles = _articlesService.GetByCategoryAlias(id, true, ref pagination).Where(p => p.IsPublished).ToList();

            if (articles == null)
                throw new Exception("Podana kategoria (" + id + ") nie istnieje.");

            ViewBag.PaginationCurrent = page.Value;
            ViewBag.PaginationTotal = PaginationSystem.GetPagesCount(pagination.TotalItems, pageSize);

            var viewModel = new CategoriesSiteViewModel()
            {
                CategoryName = id,
                TotalCount = pagination.TotalItems
            };
            viewModel.Articles = articles;

            return View(viewModel);
        }
	}
}