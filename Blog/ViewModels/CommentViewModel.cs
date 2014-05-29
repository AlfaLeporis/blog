using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.ViewModels
{
    public class CommentViewModel
    {
        public int? ID { get; set; }
        public int AuthorID { get; set; }
        public int ArticleID { get; set; }
        public DateTime PublishDate { get; set; }
        public String AvatarSource { get; set; }
        public String Content { get; set; }
        public String AuthorName { get; set; }
    }
}