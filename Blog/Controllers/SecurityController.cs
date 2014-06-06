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
    public class SecurityController : Controller
    {
        private ISecurityService _securityService = null;
        private ISettingsService _settingsService = null;

        public SecurityController(ISecurityService securityService,
                                  ISettingsService settingsService)
        {
            _securityService = securityService;
            _settingsService = settingsService;
        }

        public ActionResult Login(FormCollection viewModel)
        {
            if (viewModel["login"] == String.Empty || viewModel["password"] == String.Empty)
            {
                TempData.Add("ErrorMsg", "Nazwa użytkownika lub hasło zostały błędnie wypełnione. Spróbuj ponownie!");
                return Redirect(HttpContext.Request.UrlReferrer.ToString());
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

            return Redirect(HttpContext.Request.UrlReferrer.ToString());
        }

        [HttpGet]
        public ActionResult Logout()
        {
            _securityService.LogOut();
            return Redirect(HttpContext.Request.UrlReferrer.ToString());
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel viewModel, String returnUrl)
        {
            var result = _securityService.Register(viewModel, _settingsService.GetSettings());
            if (result)
            {
                TempData.Add("SuccessMsg", "Rejestracja przebiegła pomyślnie! Aby zalogować się na swoje konto, musisz kliknać na link aktywacyjny który znajduje się w wiadomości EMail wysłanej na Twoją skrzynkę pocztową.");
                return Redirect(returnUrl);
            }
            else
            {
                TempData.Add("ErrorMsg", "Rejestracja nieudana! Prawdopodobnie wpisana nazwa użytkownika lub EMail są już zarejestrowane. Spróbuj ponownie.");
                ViewBag.ReturnUrl = returnUrl;
                return View(viewModel);
            }
        }

        [HttpGet]
        public ActionResult EditAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EditAccount(EditPasswordViewModel viewModel, String returnUrl)
        {
            var result = _securityService.ChangePassword(viewModel);
            if (result)
            {
                TempData.Add("SuccessMsg", "Edycja hasła zakończona sukcesem!");
                return Redirect(returnUrl);
            }
            else
            {
                TempData.Add("ErrorMsg", "Błąd podczas próby zmiany hasła. Prawdopodobnie źle wpisałeś swoje obecne hasło, spróbuj ponownie.");
                ViewBag.ReturnUrl = returnUrl;
                return View(viewModel);
            }
        }
	}
}