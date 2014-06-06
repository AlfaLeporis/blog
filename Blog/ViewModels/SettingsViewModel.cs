using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class SettingsViewModel
    {
        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String Title { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String ShortDescription { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String Description { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String Tags { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String SMTPAdress { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String SMTPPort { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String SMTPUserName { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String SMTPPassword { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String SMTPUseSSL { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String SMTPUserAdress { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String TagsCount { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String ShortSiteMaxLength { get; set; }
    }
}