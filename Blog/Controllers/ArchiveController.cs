using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Services;
using Blog.ViewModels;

namespace Blog.Controllers
{
    public class ArchiveController : Controller
    {
        private IArticlesService _articlesService = null;
        private IPaginationService _paginationService = null;

        public ArchiveController(IArticlesService articlesService,
                                    IPaginationService paginationService)
        {
            _articlesService = articlesService;
            _paginationService = paginationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Archive(String id, int? page)
        {
            var viewModel = _articlesService.GetByDate(id).Where(p => p.IsPublished).ToList();

            if (!page.HasValue)
                page = 1;

            ViewBag.PaginationCurrent = page.Value;
            ViewBag.PaginationTotal = _paginationService.GetTotalPagination(viewModel.Count);

            viewModel = _paginationService.ToPaginationList<ArticleViewModel>(viewModel, page.Value);

            return View(viewModel);
        }
	}
}