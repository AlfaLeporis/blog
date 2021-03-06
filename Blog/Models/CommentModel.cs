﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public enum TargetType
    {
        Article = 1,
        Site = 2
    }

    public class CommentModel
    {
        public int ID { get; set; }
        public int AuthorID { get; set; }
        public int ArticleID { get; set; }
        public DateTime PublishDate { get; set; }
        public String Content { get; set; }
        public TargetType Target { get; set; }
        public bool IsRemoved { get; set; }
        public String AuthorName { get; set; }
    }
}