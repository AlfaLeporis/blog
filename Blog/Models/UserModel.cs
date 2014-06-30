using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class UserModel
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public String EMail { get; set; }
        public String WebSite { get; set; }
        public DateTime LastVisit { get; set; }
    }
}