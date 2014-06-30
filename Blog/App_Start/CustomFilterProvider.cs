using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace Blog.App_Start
{
    public class CustomFilterProvider : FilterAttributeFilterProvider
    {
        private IUnityContainer _unityContainer = null;

        public CustomFilterProvider(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public override IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = base.GetFilters(controllerContext, actionDescriptor);

            for (int i = 0; i < filters.Count(); i++ )
            {
                _unityContainer.BuildUp(filters.ElementAt(i).Instance.GetType(), filters.ElementAt(i).Instance);
            }

            return filters;
        }

        protected override IEnumerable<FilterAttribute> GetActionAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = base.GetActionAttributes(controllerContext, actionDescriptor);

            for (int i = 0; i < filters.Count(); i++)
            {
                _unityContainer.BuildUp(filters.ElementAt(i).GetType(), filters.ElementAt(i));
            }

            return filters;
        }

        protected override IEnumerable<FilterAttribute> GetControllerAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = base.GetControllerAttributes(controllerContext, actionDescriptor);

            for (int i = 0; i < filters.Count(); i++)
            {
                _unityContainer.BuildUp(filters.ElementAt(i).GetType(), filters.ElementAt(i));
            }

            return filters;
        }
    }
}