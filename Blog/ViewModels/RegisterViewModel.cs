using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage="Pole musi zostać wypełnione.")]
        [MinLength(3, ErrorMessage="Pole musi zawierać przynajmniej 3 znaki.")]
        [MaxLength(15, ErrorMessage="Pole nie może zawierać więcej niż 15 znaków.")]
        public String UserName { get; set; }

        [Required(ErrorMessage = "Pole musi zostać wypełnione.")]
        [Compare("UserName", ErrorMessage="Pola nazwy użytkownika są niezgodne ze sobą.")]
        public String ConfirmUserName { get; set; }

        [Required(ErrorMessage = "Pole musi zostać wypełnione.")]
        [MinLength(5, ErrorMessage = "Pole musi zawierać przynajmniej 5 znaki.")]
        public String Password { get; set; }

        [Required(ErrorMessage = "Pole musi zostać wypełnione.")]
        [Compare("Password", ErrorMessage = "Pola hasła są niezgodne ze sobą.")]
        public String ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Pole musi zostać wypełnione.")]
        [EmailAddress(ErrorMessage="Podany E-Mail jest w złym formacie.")]
        public String EMail { get; set; }

        [Required(ErrorMessage = "Pole musi zostać wypełnione.")]
        [Compare("EMail", ErrorMessage = "Pola adresu E-Mail są niezgodne ze sobą.")]
        public String ConfirmEMail { get; set; }
    }
}