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
    public class CategoriesController : Controller
    {
        private IArticlesService _articlesService = null;
        private IPaginationService _paginationService = null;

        public CategoriesController(IArticlesService articlesService,
                                    IPaginationService paginationService)
        {
            _articlesService = articlesService;
            _paginationService = paginationService;
        }

        [HttpGet]
        public ActionResult Category(String id, int? page)
        {
            var articles = _articlesService.GetByCategoryName(id, false).Where(p => p.IsPublished).ToList();

            if (articles == null)
                throw new Exception("Podana kategoria (" + id + ") nie istnieje.");

            if (!page.HasValue)
                page = 1;

            ViewBag.PaginationCurrent = page.Value;
            ViewBag.PaginationTotal = _paginationService.GetTotalPagination(articles.Count);

            var viewModel = new CategoriesSiteViewModel()
            {
                CategoryName = id
            };
            viewModel.Articles = _paginationService.ToPaginationList<ArticleViewModel>(articles, page.Value);

            return View(viewModel);
        }
	}
}