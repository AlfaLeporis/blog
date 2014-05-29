using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.ViewModels;

namespace Blog.Services
{
    public interface ISitesService
    {
        bool Add(SiteViewModel viewModel);
        bool Edit(SiteViewModel viewModel);
        void Remove(int id);
        SiteViewModel Get(int id);
        List<SiteViewModel> GetAll();
        void SetSiteStatus(int id, bool status);
    }
}
