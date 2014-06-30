using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.ViewModels;
using Blog.Services;
using Microsoft.Practices.Unity;

namespace Blog.Controllers
{
    public class SitesController : Controller
    {
        private ISitesService _sitesService = null;
        private ISettingsService _settingsService = null;

        public SitesController(ISitesService sitesService,
                               ISettingsService settingsService)
        {
            _sitesService = sitesService;
            _settingsService = settingsService;
        }

        [HttpGet]
        public ActionResult Site(String id)
        {
            var viewModel = _sitesService.GetByAlias(id);

            if (viewModel == null || !viewModel.IsPublished)
                throw new Exception("Strona o podanym id (" + id + ") nie istnieje");

            return View(viewModel);
        }
	}
}