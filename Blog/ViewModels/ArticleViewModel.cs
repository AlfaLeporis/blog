using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class ArticleViewModel
    {
        public int? ID { get; set; }
        public int CategoryID { get; set; }

        public bool IsReadMode { get; set; }
        public String CategoryName { get; set; }
        public List<String> Tags { get; set; }
        public List<CommentViewModel> Comments { get; set; }
        public bool CommentsView { get; set; }

        [Required(ErrorMessage="Pole jest wymagane!")]
        [RegularExpression("^[a-z0-9\\-]+$", ErrorMessage="Pole nie ma odpowiedniego formatu")]
        public String Alias { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String Title { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String Description { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        [AllowHtml]
        public String Content { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        [DataType(DataType.DateTime, ErrorMessage="Pole nei ma odpowiedniego formatu")]
        public DateTime CreationDate { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        [DataType(DataType.DateTime, ErrorMessage="Pole nei ma odpowiedniego formatu")]
        public DateTime PublishDate { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public bool IsPublished { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String TagsString { get; set; }
    }
}