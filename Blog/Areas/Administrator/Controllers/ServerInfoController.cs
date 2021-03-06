﻿using System;
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
    public class ServerInfoController : Controller
    {
        public ServerInfoController()
        {

        }

        [HttpGet]
        public ActionResult ServerInfo()
        {
            return RedirectToAction("Settings", new { area = "Administrator" });

            //var viewModel = new AdminServerInfoViewModel();
            //viewModel.ServerInfo = System.Web.Helpers.ServerInfo.GetHtml().ToString();

            //return View(viewModel);
        }
	}
}