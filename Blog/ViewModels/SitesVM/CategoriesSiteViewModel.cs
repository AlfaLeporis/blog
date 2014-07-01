using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.ViewModels
{
    public class CategoriesSiteViewModel
    {
        public List<ViewModels.ArticleViewModel> Articles { get; set; }
        public String CategoryName { get; set; }
        public int TotalCount { get; set; }
    }
}