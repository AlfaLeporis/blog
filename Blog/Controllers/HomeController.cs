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
    public class HomeController : Controller
    {
        private IArticlesService _articlesService = null;
        private IFeedsService _feedsService = null;
        private IPaginationService _paginationService = null;

        public HomeController(IArticlesService articlesService,
                              IFeedsService feedsService,
                              IPaginationService paginationService)
        {
            _articlesService = articlesService;
            _feedsService = feedsService;
            _paginationService = paginationService;
        }

        [HttpGet]
        public ActionResult Index(int? page)
        {
            var articles = _articlesService.GetAll(true).Where(p => p.IsPublished).ToList();
            articles.ForEach(p => p.CommentsView = false);

            if (!page.HasValue)
                page = 1;

            ViewBag.PaginationCurrent = page.Value;
            ViewBag.PaginationTotal = _paginationService.GetTotalPagination(articles.Count);

            var viewModel = new ClientViewModel();
            viewModel.Articles = _paginationService.ToPaginationList<ArticleViewModel>(articles, page.Value);
            
            return View(viewModel);
        }

        public ActionResult Error()
        {
            return View("Error");
        }

        public ActionResult GetArticlesATOM()
        {
            var articles = _articlesService.GetAll(false);
            var result = _feedsService.GenerateArticlesATOMFeed(articles, 10);

            return Content(result.OuterXml, "text/xml");
        }

        public ActionResult GetCommentsATOM(int id)
        {
            var article = _articlesService.Get(id, false);
            var result = _feedsService.GenerateCommentsATOMFeed(article);

            return Content(result.OuterXml, "text/xml");
        }
    }
}