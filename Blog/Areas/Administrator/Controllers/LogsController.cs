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
    public class LogsController : Controller
    {
        private ILogService _logService = null;

        public LogsController(ILogService logService)
        {
            _logService = logService;
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
	}
}