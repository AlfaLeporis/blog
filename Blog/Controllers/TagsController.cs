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
    public class TagsController : Controller
    {
        private IArticlesService _articlesService = null;
        private IPaginationService _paginationService = null;

        public TagsController(IArticlesService articlesService,
                              IPaginationService paginationService)
        {
            _articlesService = articlesService;
            _paginationService = paginationService;
        }

        [HttpGet]
        public ActionResult Tag(String id, int? page)
        {
            var articles = _articlesService.GetByTagName(id, false).Where(p => p.IsPublished).ToList();

            if (articles == null)
                throw new Exception("Podany tag (" + id + ") nie istnieje");

            if (!page.HasValue)
                page = 1;

            ViewBag.PaginationCurrent = page.Value;
            ViewBag.PaginationTotal = _paginationService.GetTotalPagination(articles.Count);

            var viewModel = new TagsSiteViewModel()
            {
                TagName = id
            };
            viewModel.Articles = _paginationService.ToPaginationList<ArticleViewModel>(articles, page.Value);

            return View(viewModel);
        }
	}
}