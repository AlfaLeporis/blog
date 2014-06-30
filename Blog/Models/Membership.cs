using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class Membership
    {
        [Key]
        public int UserId { get; set; }
        public DateTime? CreateDate { get; set; }
        public String ConfirmationToken { get; set; }
        public bool? IsConfirmed { get; set; }
        public DateTime? LastPasswordFailureDate { get; set; }
        public DateTime PasswordFailuresSinceLastSuccess { get; set; }
        public String Password { get; set; }
        public DateTime? PasswordChangedDate { get; set; }
        public String PasswordSalt { get; set; }
        public String PasswordVerificationToken { get; set; }
        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }
    }
}