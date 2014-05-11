using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class ArticleModel
    {
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public String Alias { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String Content { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime PublishDate { get; set; }
        public bool IsPublished { get; set; }
    }
}