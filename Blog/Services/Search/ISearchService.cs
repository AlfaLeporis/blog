using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.ViewModels;

namespace Blog.Services
{
    public interface ISearchService
    {
        SearchViewModel Search(String phrase, int maxLength);
    }
}
