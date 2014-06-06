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
    public class ServerInfoController : Controller
    {
        public ServerInfoController()
        {

        }

        [HttpGet]
        public ActionResult ServerInfo()
        {
            var viewModel = new AdminServerInfoViewModel();
            viewModel.ServerInfo = System.Web.Helpers.ServerInfo.GetHtml().ToString();

            return View(viewModel);
        }
	}
}