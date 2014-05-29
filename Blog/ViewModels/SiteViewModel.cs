﻿using System;
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

        [Required(ErrorMessage = "Pole jest wymagane!")]
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
    }
}