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
        [RegularExpression("^[0-9]+$")]
        public int SMTPPort { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String SMTPUserName { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String SMTPPassword { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public bool SMTPUseSSL { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String SMTPUserAdress { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        [RegularExpression("^[0-9]+$")]
        public int TagsCount { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        [RegularExpression("^[0-9]+$")]
        public int ShortSiteMaxLength { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String Author { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        [RegularExpression("^[0-9]+$")]
        public int ShortCommentMaxLength { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        [RegularExpression("^[0-9]+$")]
        public int RecentCommentsCount { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        [RegularExpression("^[0-9]+$")]
        public int ItemsPerPage { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        [RegularExpression("^[0-9]+$")]
        public int VersionsCount { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public bool UseCaptcha { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String CaptchaPublicKey { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane!")]
        public String CaptchaPrivateKey { get; set; }
    }
}