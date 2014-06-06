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

        public TagsController(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }

        [HttpGet]
        public ActionResult Tag(String id)
        {
            var articles = _articlesService.GetShortByTagName(id);

            if (articles == null)
                throw new HttpException(404, "Podany tag (" + id + ") nie istnieje");

            var viewModel = new TagsSiteViewModel()
            {
                Articles = articles,
                TagName = id,
            };

            return View(viewModel);
        }
	}
}