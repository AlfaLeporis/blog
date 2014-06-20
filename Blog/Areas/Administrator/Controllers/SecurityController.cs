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
    public class SecurityController : Controller
    {
        private ISecurityService _securityService = null;

        public SecurityController(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        [HttpGet]
        public ActionResult Auth()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Auth(AuthViewModel viewModel)
        {
            var result = _securityService.Login(viewModel);

            if (result)
                return RedirectToAction("Index", "Home", new { area = "Administrator" });
            else
            {
                TempData.Add("ErrorMsg", "Błąd w trakcie próby zalogowania się. Spróbuj ponownie!");
                return RedirectToAction("Auth");
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            _securityService.LogOut();
            return RedirectToAction("Auth");
        }
	}
}