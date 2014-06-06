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

        public CategoriesController(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }

        [HttpGet]
        public ActionResult Category(String id)
        {
            var articles = _articlesService.GetShortByCategoryName(id);

            if (articles == null)
                throw new HttpException(404, "Podana kategoria (" + id + ") nie istnieje.");

            var viewModel = new CategoriesSiteViewModel()
            {
                Articles = articles,
                CategoryName = id,
            };

            return View(viewModel);
        }
	}
}