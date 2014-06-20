using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.ViewModels
{
    public class RecentCommentsModuleViewModel
    {
        public ArticleViewModel Article { get; set; }
        public CommentViewModel Comment { get; set; }
    }
}