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
        bool Remove(int id);
        SiteViewModel Get(int id, bool shortVersion);
        List<SiteViewModel> GetAll();
        bool SetSiteStatus(int id, bool status);
        SiteViewModel GetByAlias(String id);
    }
}
