using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using System.IO;
using Blog.ViewModels;
using Blog.Services;
using NLog;

namespace Blog.Areas.Administrator.Controllers
{
    public class HomeController : Controller
    {
        private ISecurityService _securityService = null;
        private ILogService _logService = null;
        private ISettingsService _settingsService = null;
        private ICategoriesService _categoriesService = null;
        private IArticlesService _articlesService = null;
        private ITagsService _tagsService = null;
        private ICommentsService _commentsService = null;
        private ISitesService _sitesService = null;

        [InjectionConstructor]
        public HomeController(ISecurityService securityService,
                              ILogService logService,
                              ISettingsService settingsService,
                              ICategoriesService categoriesService,
                              IArticlesService articlesService,
                              ITagsService tagsService,
                              ICommentsService commentsService,
                              ISitesService sitesService)
        {
            _securityService = securityService;
            _logService = logService;
            _settingsService = settingsService;
            _categoriesService = categoriesService;
            _articlesService = articlesService;
            _tagsService = tagsService;
            _commentsService = commentsService;
            _sitesService = sitesService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Auth()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Auth(AuthViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _securityService.Login(viewModel);

                if (result)
                    return RedirectToAction("Index");
                else
                {
                    TempData.Add("ErrorMsg", "Błąd w trakcie próby zalogowania się. Spróbuj ponownie!");
                    return RedirectToAction("Auth");
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Logs()
        {
            var viewModel = new AdminLogsViewModel();
            viewModel.Log = _logService.GetContent();

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult ClearLog()
        {
            _logService.Clear();

            return RedirectToAction("Logs");
        }

        [HttpGet]
        public ActionResult ServerInfo()
        {
            var viewModel = new AdminServerInfoViewModel();
            viewModel.ServerInfo = System.Web.Helpers.ServerInfo.GetHtml().ToString();

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Settings()
        {
            var viewModel = new AdminSettingsViewModel();
            viewModel.Settings = _settingsService.GetSettings();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Settings(AdminSettingsViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                _settingsService.Save(viewModel.Settings);
                return RedirectToAction("Settings");
            }

            return Settings(viewModel);
        }

        [HttpGet]
        public ActionResult Categories()
        {
            var allCategories = _categoriesService.GetAll();
            return View(allCategories);
        }

        [HttpGet]
        public ActionResult EditCategory(int? id)
        {
            if (id.HasValue)
            {
                ViewBag.ButtonText = "Edytuj kategorię";
                var viewModel = _categoriesService.Get(id.Value);
                return View(viewModel);
            }
            else
            {
                ViewBag.ButtonText = "Dodaj kategorię";
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditCategory(CategoryViewModel viewModel)
        {
            ViewBag.ButtonText = "Edytuj kategorię";

            if(ModelState.IsValid)
            {
                bool result;

                if (!viewModel.ID.HasValue)
                    result = _categoriesService.Add(viewModel);
                else
                    result = _categoriesService.Edit(viewModel);

                if(!result)
                {
                    TempData.Add("ErrorMsg", "Wpisana nazwa kategorii już istnieje! Spróbuj ponownie.");
                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("Categories");
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult RemoveCategory(int id)
        {
            _categoriesService.Remove(id);
            return RedirectToAction("Categories");
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
            var test = _categoriesService.GetAll();   

            if(id.HasValue)
            {
                var article = _articlesService.Get(id.Value);
                article.Tags = _tagsService.GetListByArticleID(id.Value);
                ViewBag.ButtonText = "Edytuj artykuł";
                return View(article);
            }
            else
            {
                ViewBag.ButtonText = "Dodaj artykuł";

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
            ViewBag.CategoriesList = _categoriesService.GetAll();

            ModelState.Remove("CategoryName");
            ModelState.Remove("Tags");

            if (ModelState.IsValid)
            {
                bool result;
                if (viewModel.ID.HasValue)
                    result = _articlesService.Edit(viewModel);
                else
                    result = _articlesService.Add(viewModel);

                if(result)
                {
                    return RedirectToAction("Articles");
                }
                else
                {
                    TempData.Add("ErrorMsg", "Wystąpił błąd podczas próby edycji artkułu. Spróbuj ponownie.");
                    return View(viewModel);
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult RemoveArticle(int id)
        {
            _articlesService.Remove(id);
            return RedirectToAction("Articles");
        }

        [HttpGet]
        public ActionResult InvertArticleStatus(int id)
        {
            var article = _articlesService.Get(id);
            _articlesService.SetArticleStatus(id, !article.IsPublished);

            return RedirectToAction("Articles");
        }

        [HttpGet]
        public ActionResult Comments()
        {
            var comments = _commentsService.GetAll();
            return View(comments);
        }

        [HttpGet]
        public ActionResult RemoveComment(int id)
        {
            _commentsService.Remove(id);

            return RedirectToAction("Comments");
        }

        [HttpGet]
        public ActionResult EditComment(int id)
        {
            var viewModel = _commentsService.Get(id);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditComment(CommentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                bool result;
                if (viewModel.ID.HasValue)
                {
                    var comment = _commentsService.Get(viewModel.ID.Value);
                    comment.Content = viewModel.Content;
                    result = _commentsService.Edit(comment);
                }
                else
                    result = _commentsService.Add(viewModel);

                if (result)
                {
                    return RedirectToAction("Comments");
                }
                else
                {
                    TempData.Add("ErrorMsg", "Wystąpił błąd podczas próby edycji komentarza. Spróbuj ponownie.");
                    return View(viewModel);
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Sites()
        {
            var viewModel = _sitesService.GetAll();
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult EditSite(int? id)
        {
            if (id.HasValue)
            {
                var viewModel = _sitesService.Get(id.Value);
                return View(viewModel);
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditSite(SiteViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                bool result;
                if (viewModel.ID.HasValue)
                    result = _sitesService.Edit(viewModel);
                else
                    result = _sitesService.Add(viewModel);

                if (result)
                {
                    return RedirectToAction("Sites");
                }
                else
                {
                    TempData.Add("ErrorMsg", "Wystąpił błąd podczas próby edycji strony. Spróbuj ponownie.");
                    return View(viewModel);
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult InvertSiteStatus(int id)
        {
            var site = _sitesService.Get(id);
            _sitesService.SetSiteStatus(id, !site.IsPublished);

            return RedirectToAction("Sites");
        }

        [HttpGet]
        public ActionResult RemoveSite(int id)
        {
            _sitesService.Remove(id);
            return RedirectToAction("Sites");
        }
    }
}