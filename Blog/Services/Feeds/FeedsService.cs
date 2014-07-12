using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Blog.ViewModels;
using Blog.Services;

namespace Blog.Services
{
    public class FeedsService : IFeedsService
    {
        private ISettingsService _settingsService = null;
        private ICommentsService _commentsService = null;
        private const String _articleUrl = "Artykuł/";

        public FeedsService(ISettingsService settingsService,
                            ICommentsService commentsService)
        {
            _settingsService = settingsService;
            _commentsService = commentsService;
        }

        public XmlDocument GenerateArticlesATOMFeed(List<ArticleViewModel> articles, int count)
        {
            var document = new XmlDocument();

            var header = GetHeader(document, _settingsService.GetSettings().Title, articles.Max(p => p.LastUpdateDate));

            for (int i = 0; i < articles.Count; i++)
            {
                var entry = document.CreateElement("entry");
                var articleUrl = GetBaseUrl() + _articleUrl + articles[i].Alias;

                entry.AppendChild(CreateNode(document, "id", articleUrl));
                entry.AppendChild(CreateNode(document, "title", articles[i].Title));
                entry.AppendChild(CreateNode(document, "updated", XmlConvert.ToString(articles[i].LastUpdateDate, XmlDateTimeSerializationMode.Utc)));

                var link = CreateNode(document, "link", "");
                link.SetAttribute("href", articleUrl);
                entry.AppendChild(link);

                var authorNode = CreateNode(document, "author", "");
                authorNode.AppendChild(CreateNode(document, "name", _settingsService.GetSettings().Author));
                entry.AppendChild(authorNode);

                header.AppendChild(entry);
            }

            document.AppendChild(header);
            document.InsertBefore(document.CreateXmlDeclaration("1.0", "UTF-8", "yes"), header);

            return document;
        }

        public XmlDocument GenerateCommentsATOMFeed(ArticleViewModel article)
        {
            var document = new XmlDocument();
            var comments = _commentsService.GetByTargetID(article.ID.Value, Models.TargetType.Article);

            var lastUpdate = comments.Count != 0 ? comments.Max(p => p.PublishDate) : article.LastUpdateDate;
            var header = GetHeader(document, article.Title, lastUpdate, _articleUrl + article.Alias);

            for (int i = 0; i < comments.Count; i++)
            {
                var entry = document.CreateElement("entry");
                var articleUrl = GetBaseUrl() + _articleUrl + article.Alias + "#" + comments[i].ID;

                entry.AppendChild(CreateNode(document, "id", articleUrl));
                entry.AppendChild(CreateNode(document, "title", "Komentarz użytkownika " + comments[i].AuthorName));
                entry.AppendChild(CreateNode(document, "updated", XmlConvert.ToString(comments[i].PublishDate, XmlDateTimeSerializationMode.Utc)));

                var link = CreateNode(document, "link", "");
                link.SetAttribute("href", articleUrl);
                entry.AppendChild(link);

                var authorNode = CreateNode(document, "author", "");
                authorNode.AppendChild(CreateNode(document, "name", comments[i].AuthorName));
                entry.AppendChild(authorNode);

                header.AppendChild(entry);
            }

            document.AppendChild(header);
            document.InsertBefore(document.CreateXmlDeclaration("1.0", "UTF-8", "yes"), header);

            return document;
        }

        private XmlElement GetHeader(XmlDocument document, String title, DateTime lastUpdate, String additionalUrl = "")
        {
            var header = document.CreateElement("feed");
            header.SetAttribute("xmlns", "http://www.w3.org/2005/Atom");

            header.AppendChild(CreateNode(document, "id", GetBaseUrl()));
            header.AppendChild(CreateNode(document, "title", title));

            header.AppendChild(CreateNode(document, "updated", XmlConvert.ToString(lastUpdate, XmlDateTimeSerializationMode.Utc)));

            var link = CreateNode(document, "link", "");
            link.SetAttribute("href", GetBaseUrl() + additionalUrl);
            header.AppendChild(link);

            return header;
        }

        private XmlElement CreateNode(XmlDocument doc, String key, String value)
        {
            var node = doc.CreateElement(key);
            node.InnerText = value;
            return node;
        }

        private String GetBaseUrl()
        {
            var baseUrl = HttpContext.Current.Request.Url.Scheme + "://" +
                HttpContext.Current.Request.Url.Authority +
                HttpContext.Current.Request.ApplicationPath.TrimEnd('/') +
                "/";

            return baseUrl;
        }
    }
}