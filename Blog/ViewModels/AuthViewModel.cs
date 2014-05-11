using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class AuthViewModel
    {
        [Required]
        public String Login { get; set; }

        [Required]
        public String Password { get; set; }
    }
}