using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Services
{
    public interface IPaginationService
    {
        List<T> ToPaginationList<T>(IEnumerable<T> collection, int page);
        int GetTotalPagination(int itemsCount);
    }
}
