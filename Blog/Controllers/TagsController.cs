﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Blog.ViewModels;
using Blog.Services;
using Blog.Infrastructure;
using Blog.Filters;

namespace Blog.Controllers
{
    public class TagsController : Controller
    {
        private IArticlesService _articlesService = null;
        private ISettingsService _settingsService = null;

        public TagsController(IArticlesService articlesService,
                              ISettingsService settingsService)
        {
            _articlesService = articlesService;
            _settingsService = settingsService;
        }

        [HttpGet]
        public ActionResult Tag(String id, int? page)
        {
            if (!page.HasValue)
                page = 1;

            int pageSize = _settingsService.GetSettings().ItemsPerPage;
            var pagination = new PaginationSettings(page.Value, pageSize);
            var articles = _articlesService.GetByTagName(id, true, ref pagination).Where(p => p.IsPublished).ToList();

            if (articles == null)
                throw new Exception("Podany tag (" + id + ") nie istnieje");

            int totalItems = PaginationSystem.GetPagesCount(pagination.TotalItems, pageSize);

            ViewBag.PaginationCurrent = page.Value;
            ViewBag.PaginationTotal = totalItems;

            if (page.Value > totalItems)
                throw new Exception("Podany numer strony nie istnieje.");

            var viewModel = new TagsSiteViewModel()
            {
                TagName = id,
                TotalCount = pagination.TotalItems
            };
            viewModel.Articles = articles;

            return View(viewModel);
        }
	}
}