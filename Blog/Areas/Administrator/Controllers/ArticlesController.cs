using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.Practices.Unity;

namespace Blog.Areas.Administrator.Controllers
{
    public class ArticlesController : Controller
    {
        private IArticlesService _articlesService = null;
        private ICategoriesService _categoriesService = null;

        public ArticlesController(IArticlesService articlesService,
                                  ICategoriesService categoriesService)
        {
            _articlesService = articlesService;
            _categoriesService = categoriesService;
        }

        [HttpGet]
        public ActionResult Articles()
        {
            var articles = _articlesService.GetAll();
            return View(articles);
        }

        [HttpGet]
        public ActionResult EditArticle(int? id)
        {
            ViewBag.CategoriesList = _categoriesService.GetAll();

            if (id.HasValue)
            {
                var article = _articlesService.Get(id.Value);
                return View(article);
            }
            else
            {
                var viewModel = new ArticleViewModel()
                {
                    CreationDate = DateTime.Now,
                    Tags = new List<string>()
                };
                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult EditArticle(ArticleViewModel viewModel)
        {
            ViewData.Add(new KeyValuePair<string,object>("CategoriesList", _categoriesService.GetAll()));

            ModelState.Remove("CategoryName");
            ModelState.Remove("Tags");

            bool result;
            if (viewModel.ID.HasValue)
                result = _articlesService.Edit(viewModel);
            else
                result = _articlesService.Add(viewModel);

            if (result)
            {
                return RedirectToAction("Articles");
            }
            else
            {
                TempData.Add("ErrorMsg", "Wystąpił błąd podczas próby edycji artkułu. Spróbuj ponownie.");
                return View(viewModel);
            }
        }

        [HttpGet]
        public ActionResult RemoveArticle(int id)
        {
            bool result = _articlesService.Remove(id);

            if (!result)
                throw new HttpException(404, "Usunięcie artykułu (" + id + ") nie powiodło się");

            return RedirectToAction("Articles");
        }

        [HttpGet]
        public ActionResult InvertArticleStatus(int id)
        {
            var article = _articlesService.Get(id);
            bool result = _articlesService.SetArticleStatus(id, !article.IsPublished);

            if (!result)
                throw new HttpException(404, "Błąd w czasie próby zmiany statusu artykułu (" + id + ")");

            return RedirectToAction("Articles");
        }
	}
}