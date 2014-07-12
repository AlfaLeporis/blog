using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Services;
using System.Xml;
using Blog.Infrastructure;
using System.Web.Mvc;

namespace Blog.Services
{
    public class SiteMapsService : ISiteMapsService
    {
        private IArticlesService _articlesService = null;
        private ISitesService _sitesService = null;
        private ICategoriesService _categoriesService = null;
        private ITagsService _tagsService = null;

        public SiteMapsService(IArticlesService articlesService,
                               ISitesService sitesService,
                               ICategoriesService categoriesService,
                               ITagsService tagsService)
        {
            _articlesService = articlesService;
            _sitesService = sitesService;
            _categoriesService = categoriesService;
            _tagsService = tagsService;
        }

        public XmlDocument GenerateNewSiteMap()
        {
            var xmlDocument = new XmlDocument();

            var urlSet = xmlDocument.CreateElement("urlset");
            urlSet.SetAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

            AddArticlesToNode(xmlDocument, urlSet);
            AddSitesToNode(xmlDocument, urlSet);
            AddCategoriesToNode(xmlDocument, urlSet);
            AddTagsToNode(xmlDocument, urlSet);

            xmlDocument.AppendChild(urlSet);
            xmlDocument.InsertBefore(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", "yes"), urlSet);
            return xmlDocument;
        }

        private void AddArticlesToNode(XmlDocument document, XmlElement urlSet)
        {
            PaginationSettings pagination = null;
            var articles = _articlesService.GetAll(false, new ArticleSiteAccessSettings(false, true, false), ref pagination);
          
            for(int i=0; i<articles.Count; i++)
            {
                var url = document.CreateElement("url");
                url.AppendChild(CreateNode(document, "loc", GetBaseUrl() + "Artykuł/" + articles[i].Alias));
                url.AppendChild(CreateNode(document, "lastmod", articles[i].LastUpdateDate.ToShortDateString()));
                url.AppendChild(CreateNode(document, "changefreq", "always"));
                url.AppendChild(CreateNode(document, "priority", "1.0"));

                urlSet.AppendChild(url);
            }
        }

        private void AddSitesToNode(XmlDocument document, XmlElement urlSet)
        {
            PaginationSettings pagination = null;
            var sites = _sitesService.GetAll(new ArticleSiteAccessSettings(false, true, false), ref pagination);

            for (int i = 0; i < sites.Count; i++)
            {
                var url = document.CreateElement("url");
                url.AppendChild(CreateNode(document, "loc", GetBaseUrl() + "Strona/" + sites[i].Alias));
                url.AppendChild(CreateNode(document, "lastmod", sites[i].LastUpdateDate.ToShortDateString()));
                url.AppendChild(CreateNode(document, "changefreq", "always"));
                url.AppendChild(CreateNode(document, "priority", "1.0"));

                urlSet.AppendChild(url);
            }
        }

        private void AddCategoriesToNode(XmlDocument document, XmlElement urlSet)
        {
            PaginationSettings pagination = null;
            var categories = _categoriesService.GetAll();

            for (int i = 0; i < categories.Count; i++)
            {
                var articles = _articlesService.GetByCategoryAlias(categories[i].Alias, false, ref pagination);

                var url = document.CreateElement("url");
                url.AppendChild(CreateNode(document, "loc", GetBaseUrl() + "Kategoria/" + categories[i].Alias));
                url.AppendChild(CreateNode(document, "lastmod", 
                    articles.Count != 0 ? articles.Max(p => p.LastUpdateDate).ToShortDateString() : DateTime.MinValue.ToShortDateString()
                    ));
                url.AppendChild(CreateNode(document, "changefreq", "always"));
                url.AppendChild(CreateNode(document, "priority", "1.0"));

                urlSet.AppendChild(url);
            }
        }

        private void AddTagsToNode(XmlDocument document, XmlElement urlSet)
        {
            PaginationSettings pagination = null;
            var tags = _tagsService.GetAllWithoutDuplicates();

            for (int i = 0; i < tags.Count; i++)
            {
                var articles = _articlesService.GetByTagName(tags[i].Name, false, ref pagination);

                var url = document.CreateElement("url");
                url.AppendChild(CreateNode(document, "loc", GetBaseUrl() + "Tag/" + tags[i].Name));
                url.AppendChild(CreateNode(document, "lastmod", 
                    articles.Count != 0 ? articles.Max(p => p.LastUpdateDate).ToShortDateString() : DateTime.MinValue.ToShortDateString()
                    ));
                url.AppendChild(CreateNode(document, "changefreq", "always"));
                url.AppendChild(CreateNode(document, "priority", "1.0"));

                urlSet.AppendChild(url);
            }
        }

        private String GetBaseUrl()
        {
            var baseUrl = HttpContext.Current.Request.Url.Scheme + "://" +
                HttpContext.Current.Request.Url.Authority +
                HttpContext.Current.Request.ApplicationPath.TrimEnd('/') +
                "/";

            return baseUrl;
        }

        private XmlElement CreateNode(XmlDocument doc, String key, String value)
        {
            var node = doc.CreateElement(key);
            node.InnerText = value;
            return node;
        }

        public void Save(System.Xml.XmlDocument document)
        {
            throw new NotImplementedException();
        }
    }
}