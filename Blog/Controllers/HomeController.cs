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

        public HomeController(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var articles = _articlesService.GetAllShortVersion();
            articles.ForEach(p => p.CommentsView = false);

            var viewModel = new ClientViewModel()
            {
                Articles = articles
            };
            
            return View(viewModel);
        }

        public ActionResult Error()
        {
            return View("Error");
        }
    }
}