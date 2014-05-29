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
        private ISecurityService _securityService = null;
        private ISettingsService _settingsService = null;
        private IArticlesService _articlesService = null;
        private ITagsService _tagsService = null;
        private ICategoriesService _categoriesService = null;
        private ICommentsService _commentsService = null;

        private const float _maxTagSize = 1.5f;
        private const float _minTagSize = 1.0f;

        [InjectionConstructor]
        public HomeController(ISecurityService securityService,
                              ISettingsService settingsService,
                              IArticlesService articlesService,
                              ITagsService tagsService,
                              ICategoriesService categoriesService,
                              ICommentsService commentsService)
        {
            _securityService = securityService;
            _settingsService = settingsService;
            _articlesService = articlesService;
            _tagsService = tagsService;
            _categoriesService = categoriesService;
            _commentsService = commentsService;
        }

        public ActionResult Index()
        {
            var articles = _articlesService.GetAllShortVersion();
            for(int i=0; i<articles.Count; i++)
            {
                articles[i].Comments = _commentsService.GetByArticleID(articles[i].ID.Value);
            }

            var viewModel = new ClientViewModel()
            {
                Articles = articles
            };
            
            return View(viewModel);
        }

        public ActionResult Login(FormCollection viewModel)
        {
            if (viewModel["login"] == null || viewModel["password"] == null)
            {
                TempData.Add("ErrorMsg", "Nazwa użytkownika lub hasło zostały błędnie wypełnione. Spróbuj ponownie!");
                return RedirectToAction("Index");
            }

            var authViewModel = new AuthViewModel()
            {
                Login = viewModel["login"],
                Password = viewModel["password"]
            };

            bool result = _securityService.Login(authViewModel);
            if (result)
                TempData.Add("SuccessMsg", "Zalogowano poprawnie!");
            else
                TempData.Add("ErrorMsg", "Nieprawidłowe dane, spróbuj ponownie!");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            _securityService.LogOut();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var result = _securityService.Register(viewModel, _settingsService.GetSettings());
                if (result)
                {
                    TempData.Add("SuccessMsg", "Rejestracja przebiegła pomyślnie! Aby zalogować się na swoje konto, musisz kliknać na link aktywacyjny który znajduje się w wiadomości EMail wysłanej na Twoją skrzynkę pocztową.");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData.Add("ErrorMsg", "Rejestracja nieudana! Prawdopodobnie wpisana nazwa użytkownika lub EMail są już zarejestrowane. Spróbuj ponownie.");
                    return RedirectToAction("Register");
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult EditAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EditAccount(EditPasswordViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var result = _securityService.ChangePassword(viewModel);
                if (result)
                {
                    TempData.Add("SuccessMsg", "Edycja hasła zakończona sukcesem!");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData.Add("ErrorMsg", "Błąd podczas próby zmiany hasła. Prawdopodobnie źle wpisałeś swoje obecne hasło, spróbuj ponownie.");
                    return View(viewModel);
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Tag(String id)
        {
            var articles = _articlesService.GetShortByTagName(id);

            for (int i = 0; i < articles.Count; i++)
            {
                articles[i].Comments = _commentsService.GetByArticleID(articles[i].ID.Value);
            }

            var viewModel = new TagsSiteViewModel()
            {
                Articles = articles,
                TagName = id,
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Category(String id)
        {
            var articles = _articlesService.GetShortByCategoryName(id);

            for(int i=0; i<articles.Count; i++)
            {
                articles[i].Comments = _commentsService.GetByArticleID(articles[i].ID.Value);
            }

            var viewModel = new CategoriesSiteViewModel()
            {
                Articles = articles,
                CategoryName = id,
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Article(String id)
        {
            var article = _articlesService.GetByAlias(id);
            article.IsReadMode = false;
            article.Comments = _commentsService.GetByArticleID(article.ID.Value);
            return View(article);
        }

        [HttpGet]
        public ActionResult HeaderModule()
        {
            return PartialView("_HeaderModule", _settingsService.GetSettings());
        }

        [HttpGet]
        public ActionResult TagsModule()
        {
            var viewModel = _tagsService.GetMostPopularTags(Convert.ToInt32(_settingsService.GetSettings().TagsCount));

            float delta = _maxTagSize - _minTagSize;
            int maxCount = viewModel.Count == 0 ? 0 : viewModel.Max(p => p.Count);
            int minCount = viewModel.Count == 0 ? 0 : viewModel.Min(p => p.Count);

            for (int i = 0; i < viewModel.Count; i++)
            {
                float deltaSize = (100 * viewModel[i].Count) / maxCount;
                deltaSize *= delta / 100;

                viewModel[i].Size = _minTagSize + deltaSize;
            }

            return PartialView("_TagsModule", viewModel);
        }

        [HttpGet]
        public ActionResult CategoriesModule()
        {
            return PartialView("_CategoriesModule", _categoriesService.GetAllWithDetails());
        }

        [HttpPost]
        public ActionResult AddComment(FormCollection viewModel)
        {
            int articleID = Convert.ToInt32(viewModel["ArticleID"]);
            String content = viewModel["CommentContent"];

            var articleAlias = _articlesService.Get(articleID).Alias;

            var commentVM = new CommentViewModel()
            {
                ArticleID = articleID,
                Content = content,
                PublishDate = DateTime.Now,
                AuthorID = _securityService.GetCurrentID()
            };

            _commentsService.Add(commentVM);

            return RedirectToAction("Article", new { id = articleAlias });
        }

        [HttpGet]
        public ActionResult RemoveComment(int id, int articleID)
        {
            _commentsService.Remove(id);

            var articleAlias = _articlesService.Get(articleID).Alias;

            return RedirectToAction("Article", new { id = articleAlias });
        }
    }
}