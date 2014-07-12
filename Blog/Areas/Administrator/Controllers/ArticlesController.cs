using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.Practices.Unity;
using Blog.Infrastructure;
using Blog.Filters;

namespace Blog.Areas.Administrator.Controllers
{
    [Authorize(Roles="Administrator")]
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
        public ActionResult Articles(bool? showRemoved)
        {
            PaginationSettings pagination = null;
            var settings = new ArticleSiteAccessSettings(showRemoved.HasValue ? showRemoved.Value : false, false, false);
            var articles = _articlesService.GetAll(false, settings, ref pagination);
            return View(articles);
        }

        [HttpGet]
        public ActionResult EditArticle(int? id)
        {
            var categories = _categoriesService.GetAll();
            if (id.HasValue)
            {
                var article = _articlesService.Get(id.Value, false);
                article.Categories = categories;
                return View(article);
            }
            else
            {
                var viewModel = new ArticleViewModel()
                {
                    CreationDate = DateTime.Now,
                    PublishDate = DateTime.Now,
                    Tags = new List<string>(),
                    Categories = categories,
                    CategoryID = 0
                };
                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult EditArticle(ArticleViewModel viewModel)
        {
            ModelState.Remove("CategoryName");
            ModelState.Remove("Tags");

            bool result;
            if (viewModel.ID.HasValue)
                result = _articlesService.Edit(viewModel);
            else
                result = _articlesService.Add(viewModel);

            if (result)
            {
                if (Request.Form.AllKeys.Any(p => p == "save-and-exit"))
                    return RedirectToAction("Articles");
                else
                {
                    var articleID = viewModel.Parent == null ? viewModel.ID : viewModel.Parent;
                    return RedirectToAction("EditArticle", new { id = articleID });
                }
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
                throw new Exception("Usunięcie artykułu (" + id + ") nie powiodło się");

            return RedirectToAction("Articles");
        }

        [HttpGet]
        public ActionResult InvertArticleStatus(int id)
        {
            var article = _articlesService.Get(id, false);
            bool result = _articlesService.SetArticleStatus(id, !article.IsPublished);

            if (!result)
                throw new Exception("Błąd w czasie próby zmiany statusu artykułu (" + id + ")");

            return RedirectToAction("Articles");
        }

        [HttpGet]
        public ActionResult RestoreArticle(int id)
        {
            var article = _articlesService.Get(id, false);
            article.IsRemoved = false;
            _articlesService.Edit(article);

            return RedirectToAction("Articles", new { showRemoved = "true" });
        }
	}
}