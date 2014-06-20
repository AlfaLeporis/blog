using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.ViewModels;
using Blog.Services;

namespace Blog.Services
{
    public class SearchService : ISearchService
    {
        private ISitesService _sitesService = null;
        private IArticlesService _articlesService = null;

        public SearchService(ISitesService sitesService,
                             IArticlesService articlesService)
        {
            _sitesService = sitesService;
            _articlesService = articlesService;
        }

        public SearchViewModel Search(string phrase, int maxLength)
        {
            var viewModel = new SearchViewModel
            {
                Articles = new List<ArticleViewModel>(),
                Sites = new List<SiteViewModel>(),
                Phrase = phrase
            };

            var sitesIDs = _sitesService.GetAll().Where(p => 
                p.Content.Contains(phrase) || 
                p.Title.Contains(phrase) || 
                p.Alias.Contains(phrase))
                .Select(p => p.ID.Value)
                .ToList();

            var articlesIDs = _articlesService.GetAll(false).Where(p =>
                p.Content.Contains(phrase) ||
                p.Title.Contains(phrase) ||
                p.TagsString.Contains(phrase) ||
                p.Description.Contains(phrase) ||
                p.Alias.Contains(phrase) ||
                p.CategoryName.Contains(phrase))
                .OrderByDescending(p => p.PublishDate)
                .Select(p => p.ID.Value)
                .ToList();

            for (int i = 0; i < articlesIDs.Count(); i++)
            {
                var article = _articlesService.Get(articlesIDs[i], false);
                if (article == null)
                    throw new HttpException(404, "Artykuł o podanym id (" + articlesIDs[i] + ") nie istnieje");

                viewModel.Articles.Add(article);
            }

            for (int i = 0; i < sitesIDs.Count(); i++)
            {
                var site = _sitesService.Get(sitesIDs[i], true);
                if (site == null)
                    throw new HttpException(404, "Strona o podanym id (" + sitesIDs[i] + ") nie istnieje");

                viewModel.Sites.Add(site);
            }

            return viewModel;
        }
    }
}