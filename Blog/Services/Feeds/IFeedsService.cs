using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Blog.ViewModels;

namespace Blog.Services
{
    public interface IFeedsService
    {
        XmlDocument GenerateArticlesATOMFeed(List<ArticleViewModel> articles, int count);
        XmlDocument GenerateCommentsATOMFeed(ArticleViewModel article);
    }
}
