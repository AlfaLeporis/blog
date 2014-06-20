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
    [Authorize(Roles = "Administrator")]
    public class SettingsController : Controller
    {
        private ISettingsService _settingsService = null;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
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
            _settingsService.Save(viewModel.Settings);
            return RedirectToAction("Settings");
        }
	}
}