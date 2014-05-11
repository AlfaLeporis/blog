using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class TagModel
    {
        public int ID { get; set; }
        public int ArticleID { get; set; }
        public String Name { get; set; }
    }
}