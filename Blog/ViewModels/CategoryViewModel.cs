using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class CategoryViewModel
    {
        public int? ID { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String Title { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        [RegularExpression("^[a-z0-9\\-]+$", ErrorMessage = "Pole nie ma odpowiedniego formatu")]
        public String Alias { get; set; }
    }
}