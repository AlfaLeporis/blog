using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.ViewModels
{
    public class SearchViewModel
    {
        public String Phrase { get; set; }
        public List<ArticleViewModel> Articles { get; set; }
        public List<SiteViewModel> Sites { get; set; }
    }
}