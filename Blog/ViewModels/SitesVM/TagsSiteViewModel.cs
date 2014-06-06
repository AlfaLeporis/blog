using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.ViewModels
{
    public class TagsSiteViewModel
    {
        public List<ViewModels.ArticleViewModel> Articles { get; set; }
        public String TagName { get; set; }
    }
}