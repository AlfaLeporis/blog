using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Services;
using Blog.ViewModels;
using Blog.Infrastructure;

namespace Blog.Controllers
{
    public class ArchiveController : Controller
    {
        private IArticlesService _articlesService = null;
        private ISettingsService _settingsService = null;

        public ArchiveController(IArticlesService articlesService,
                                 ISettingsService settingsService)
        {
            _articlesService = articlesService;
            _settingsService = settingsService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Archive(String id, int? page)
        {
            var viewModel = new ArchiveViewModel();

            if (!page.HasValue)
                page = 1;

            int pageSize = _settingsService.GetSettings().ItemsPerPage;
            PaginationSettings pagination = new PaginationSettings(page.Value, pageSize);
            viewModel.Articles = _articlesService.GetByDate(id, false, ref pagination).Where(p => p.IsPublished).ToList();

            viewModel.FirstDate = viewModel.Articles.Min(p => p.PublishDate).ToShortDateString();
            viewModel.LastDate = viewModel.Articles.Max(p => p.PublishDate).ToShortDateString();

            ViewBag.PaginationCurrent = page.Value;
            ViewBag.PaginationTotal = PaginationSystem.GetPagesCount(pagination.TotalItems, pageSize);

            return View(viewModel);
        }
	}
}