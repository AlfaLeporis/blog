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

        [InjectionConstructor]
        public HomeController(ISecurityService securityService,
                              ISettingsService settingsService)
        {
            _securityService = securityService;
            _settingsService = settingsService;
        }

        public ActionResult Index()
        {
            return View();
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
    }
}