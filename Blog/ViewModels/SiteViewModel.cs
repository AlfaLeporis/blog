using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class SiteViewModel
    {
        public int? ID { get; set; }
        public bool IsReadMore { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        [RegularExpression("^[a-z0-9\\-]+$", ErrorMessage = "Pole nie ma odpowiedniego formatu")]
        public String Alias { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String Title { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String Description { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String Content { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public bool IsPublished { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public int Version { get; set; }
        public int? Parent { get; set; }
        public bool IsRemoved { get; set; }
    }
}