using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Models;

namespace Blog.ViewModels
{
    public class VersionsViewModel
    {
        public TargetType Target { get; set; }
        public List<SingleVersionViewModel> Versions { get; set; }
    }
}