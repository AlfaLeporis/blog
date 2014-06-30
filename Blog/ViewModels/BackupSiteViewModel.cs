using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.ViewModels
{
    public class BackupSiteViewModel
    {
        public List<BackupViewModel> Backups { get; set; }

        public int Count { get; set; }
        public DateTime LastBackup { get; set; }
    }
}