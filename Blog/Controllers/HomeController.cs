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
    public class HomeController : Controller
    {
        private IArticlesService _articlesService = null;
        private IFeedsService _feedsService = null;
        private ISettingsService _settingsService = null;
        private ISiteMapsService _siteMapsService = null;

        public HomeController(IArticlesService articlesService,
                              IFeedsService feedsService,
                              ISettingsService settingsService,
                              ISiteMapsService siteMapsService)
        {
            _articlesService = articlesService;
            _feedsService = feedsService;
            _settingsService = settingsService;
            _siteMapsService = siteMapsService;
        }

        [HttpGet]
        public ActionResult Index(int? page)
        {
            if (!page.HasValue)
                page = 1;

            var pageSize = _settingsService.GetSettings().ItemsPerPage;
            var pagination = new PaginationSettings(page.Value, pageSize);
            var articles = _articlesService.GetAll(true, new ArticleSiteAccessSettings(false, true, false), ref pagination).ToList();
            articles.ForEach(p => p.CommentsView = false);
            
            int totalItems = PaginationSystem.GetPagesCount(pagination.TotalItems, pageSize);

            ViewBag.PaginationCurrent = page.Value;
            ViewBag.PaginationTotal = totalItems;

            if (page.Value > totalItems && totalItems != 0)
                throw new Exception("Podany numer strony nie istnieje.");

            var viewModel = new ClientViewModel();
            viewModel.Articles = articles;
            
            return View(viewModel);
        }

        public ActionResult GetArticlesATOM()
        {
            PaginationSettings pagination = null;
            var articles = _articlesService.GetAll(false, new ArticleSiteAccessSettings(false, true, false), ref pagination);
            var result = _feedsService.GenerateArticlesATOMFeed(articles, 10);

            return Content(result.OuterXml, "text/xml");
        }

        public ActionResult GetCommentsATOM(int id)
        {
            var article = _articlesService.Get(id, false);
            var result = _feedsService.GenerateCommentsATOMFeed(article);

            return Content(result.OuterXml, "text/xml");
        }

        public ActionResult GetSiteMap()
        {
            var xml = _siteMapsService.GenerateNewSiteMap();
            return Content(xml.OuterXml, "text/xml");
        }
    }
}