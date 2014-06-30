using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.ViewModels
{
    public class ArchiveViewModel
    {
        public List<ArticleViewModel> Articles { get; set; }
        public String FirstDate { get; set; }
        public String LastDate { get; set; }
    }
}