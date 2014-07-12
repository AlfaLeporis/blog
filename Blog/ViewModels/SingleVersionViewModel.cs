using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.ViewModels
{
    public class SingleVersionViewModel
    {
        public int TargetID { get; set; }
        public int Version { get; set; }
        public DateTime SaveDate { get; set; }
    }
}