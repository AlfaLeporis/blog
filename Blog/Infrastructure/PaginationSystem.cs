using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Blog.Infrastructure
{
    public static class PaginationSystem
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> items, PaginationSettings settings)
        {
            if (settings == null)
                return items;

            return items.Skip((settings.CurrentPage - 1) * settings.PageSize).Take(settings.PageSize);
        }

        public static int GetPagesCount(int totalItems, int pageSize)
        {
            double result = Math.Ceiling(Convert.ToDouble(totalItems) / Convert.ToDouble(pageSize));
            return Convert.ToInt32(result);
        }
    }

    public class PaginationSettings
    {
        //In
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        //Out
        public int TotalItems { get; set; }

        public PaginationSettings(int currentPage, int pageSize)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
        }
    }
}