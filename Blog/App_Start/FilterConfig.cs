using System.Web;
using System.Web.Mvc;
using Blog.Filters;
using Microsoft.Practices.Unity;

namespace Blog
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, IUnityContainer container)
        {
            filters.Add(container.Resolve<ErrorFilterAttribute>());
            filters.Add(container.Resolve<ReturnUrlAttribute>());
            filters.Add(container.Resolve<ValidationModelState>());
            filters.Add(container.Resolve<GlobalInfoAttribute>());
        }
    }
}