using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Services;

namespace Blog.Services
{
    public class PaginationService : IPaginationService
    {
        private ISettingsService _settingsService = null;

        public PaginationService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public List<T> ToPaginationList<T>(IEnumerable<T> collection, int page)
        {
            var list = new List<T>();
            int itemsPerPage = Convert.ToInt32(_settingsService.GetSettings().ItemsPerPage);

            int firstElement = itemsPerPage * (page - 1);
            int lastElement = itemsPerPage * page;

            if (collection.Count() - 1 < lastElement)
                lastElement = collection.Count();
            
            int itemsToTake = lastElement - firstElement;

            list.AddRange(collection.Skip(firstElement).Take(itemsToTake));
            return list;
        }


        public int GetTotalPagination(int itemsCount)
        {
            int itemsPerPage = Convert.ToInt32(_settingsService.GetSettings().ItemsPerPage);
            double result = Math.Ceiling(Convert.ToDouble(itemsCount) / Convert.ToDouble(itemsPerPage));
            return Convert.ToInt32(result);
        }
    }
}