using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.ViewModels
{
    public class SettingsViewModel
    {
        public String Title { get; set; }
        public String ShortDescription { get; set; }
        public String Description { get; set; }
        public String Tags { get; set; }

        public String SMTPAdress { get; set; }
        public String SMTPPort { get; set; }
        public String SMTPUserName { get; set; }
        public String SMTPPassword { get; set; }
        public String SMTPUseSSL { get; set; }
        public String SMTPUserAdress { get; set; }

        public String TagsCount { get; set; }
    }
}