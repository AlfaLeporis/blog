using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class Roles
    {
        public int RoleId { get; set; }
        public String RoleName { get; set; }
    }
}