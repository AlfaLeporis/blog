﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.Practices.Unity;
using Blog.Infrastructure;

namespace Blog.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SitesController : Controller
    {
        private ISitesService _sitesService = null;

        public SitesController(ISitesService sitesService)
        {
            _sitesService = sitesService;
        }

        [HttpGet]
        public ActionResult Sites(bool? showRemoved)
        {
            PaginationSettings pagination = null;
            var settings = new ArticleSiteAccessSettings(showRemoved.HasValue ? showRemoved.Value : false, false, false);
            var viewModel = _sitesService.GetAll(settings, ref pagination);
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult EditSite(int? id)
        {
            if (id.HasValue)
            {
                var viewModel = _sitesService.Get(id.Value, false);

                if (viewModel == null)
                    throw new Exception("Strona (" + id.Value + ") nie istnieje.");

                return View(viewModel);
            }
            return View(new SiteViewModel()
            {
                CreationDate = DateTime.Now,
                LastUpdateDate = DateTime.Now
            });
        }

        [HttpPost]
        public ActionResult EditSite(SiteViewModel viewModel)
        {
            bool result;
            if (viewModel.ID.HasValue)
                result = _sitesService.Edit(viewModel);
            else
                result = _sitesService.Add(viewModel);

            if (result)
            {
                if (Request.Form.AllKeys.Any(p => p == "save-and-exit"))
                    return RedirectToAction("Sites");
                else
                {
                    var articleID = viewModel.Parent == null ? viewModel.ID : viewModel.Parent;
                    return RedirectToAction("EditSite", new { id = articleID });
                }
            }
            else
            {
                TempData.Add("ErrorMsg", "Wystąpił błąd podczas próby edycji strony. Spróbuj ponownie.");
                return View(viewModel);
            }
        }

        [HttpGet]
        public ActionResult InvertSiteStatus(int id)
        {
            var site = _sitesService.Get(id, false);
            bool result = _sitesService.SetSiteStatus(id, !site.IsPublished);

            if (!result)
                throw new Exception("Zmiana statusu strony (" + id + ") nie powiodła się.");

            return RedirectToAction("Sites");
        }

        [HttpGet]
        public ActionResult RemoveSite(int id)
        {
            bool result = _sitesService.Remove(id);

            if (!result)
                throw new Exception("Usunięcie komentarza (" + id + ") nie powiodła się.");

            return RedirectToAction("Sites");
        }

        [HttpGet]
        public ActionResult RestoreSite(int id)
        {
            var site = _sitesService.Get(id, false);
            site.IsRemoved = false;
            _sitesService.Edit(site);

            return RedirectToAction("Sites", new { showRemoved = "true" });
        }
	}
}