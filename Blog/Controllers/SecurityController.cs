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

            return RedirectToAction("Home", "Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Logout()
        {
            _securityService.LogOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel viewModel)
        {
            var result = _securityService.Register(viewModel, _settingsService.GetSettings());
            if (result)
            {
                TempData.Add("SuccessMsg", "Rejestracja przebiegła pomyślnie!");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData.Add("ErrorMsg", "Rejestracja nieudana! Prawdopodobnie wpisana nazwa użytkownika lub EMail są już zarejestrowane. Spróbuj ponownie.");
                return View(viewModel);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditPassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditPassword(EditPasswordViewModel viewModel)
        {
            var result = _securityService.ChangePassword(viewModel);
            if (result)
            {
                TempData.Add("SuccessMsg", "Edycja hasła zakończona sukcesem!");
                return View();
            }
            else
            {
                TempData.Add("ErrorMsg", "Błąd podczas próby zmiany hasła. Prawdopodobnie źle wpisałeś swoje obecne hasło, spróbuj ponownie.");
                return View(viewModel);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditAccount()
        {
            var user = _securityService.Get(_securityService.GetCurrentID());
            return View(user);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditAccount(UserViewModel viewModel)
        {
            var result = _securityService.Edit(viewModel);
            if (result)
            {
                TempData.Add("SuccessMsg", "Edycja danych użytkownika zakończona sukcesem!");
                return RedirectToAction("EditAccount");
            }
            else
            {
                TempData.Add("ErrorMsg", "Błąd podczas próby zmiany danych profilu.");
                return View(viewModel);
            }
        }
	}
}