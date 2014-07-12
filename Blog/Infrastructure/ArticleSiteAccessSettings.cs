using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Infrastructure
{
    public class ArticleSiteAccessSettings
    {
        public bool Removed { get; set; }
        public bool Published { get; set; }
        public bool WithParent { get; set; }

        public ArticleSiteAccessSettings(bool withRemoved, bool onlyPublished, bool withParent)
        {
            Removed = withRemoved;
            Published = onlyPublished;
            WithParent = withParent;
        }
    }
}