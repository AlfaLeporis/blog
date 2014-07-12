using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Models;

namespace Blog.ViewModels
{
    public class CommentsModuleViewModel
    {
        public int TargetID { get; set; }
        public TargetType Target { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }
}