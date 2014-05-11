using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class EditPasswordViewModel
    {
        [Required(ErrorMessage = "Pole musi zostać wypełnione.")]
        public String OldPassword { get; set; }

        [Required(ErrorMessage = "Pole musi zostać wypełnione.")]
        [MinLength(5, ErrorMessage = "Pole musi zawierać przynajmniej 5 znaki.")]
        public String NewPassword { get; set; }

        [Required(ErrorMessage = "Pole musi zostać wypełnione.")]
        [Compare("NewPassword", ErrorMessage="Pola haseł nie są ze sobą zgodne!")]
        public String ConfirmNewPassword { get; set; }
    }
}