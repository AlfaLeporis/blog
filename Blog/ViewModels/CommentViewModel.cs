using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Blog.Models;

namespace Blog.ViewModels
{
    public class CommentViewModel
    {
        public int? ID { get; set; }
        public int AuthorID { get; set; }
        public int ArticleID { get; set; }
        public DateTime PublishDate { get; set; }
        public String AvatarSource { get; set; }
        public String AuthorSite { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String Content { get; set; }

        public String AuthorName { get; set; }

        public TargetType Target { get; set; }
    }
}