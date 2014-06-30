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
    public class ArticlesController : Controller
    {
        private IArticlesService _articlesService = null;
        private ISettingsService _settingsService = null;

        public ArticlesController(IArticlesService articlesService,
                                 ISettingsService settingsService)
        {
            _articlesService = articlesService;
            _settingsService = settingsService;
        }

        [HttpGet]
        public ActionResult Article(String id)
        {
            var article = _articlesService.GetByAlias(id, false);

            if (article == null || !article.IsPublished)
                throw new Exception("Artykuł o podanym id (" + id + ") nie może zostać odnaleziony.");

            article.CommentsView = true;

            return View(article);
        }
	}
}