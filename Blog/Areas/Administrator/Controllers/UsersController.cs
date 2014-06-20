using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Services;
using Blog.ViewModels;

namespace Blog.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private ISecurityService _securityService = null;

        public UsersController(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        public ActionResult Users()
        {
            var users = _securityService.GetAll();
            return View(users);
        }

        [HttpGet]
        public ActionResult EditUser(int id)
        {
            var user = _securityService.Get(id);
            return View(user);
        }

        [HttpPost]
        public ActionResult EditUser(UserViewModel viewModel)
        {
            bool result = _securityService.Edit(viewModel);
            if (!result)
                throw new Exception("Edycja użytkownika nie powiodła się");

            return RedirectToAction("Users");
        }

        [HttpGet]
        public ActionResult RemoveUser(int id)
        {
            bool result = _securityService.Remove(id);
            if (!result)
                throw new Exception("Użytkownik o podanym id (" + id + ") nie istnieje.");

            return RedirectToAction("Users");
        }
	}
}