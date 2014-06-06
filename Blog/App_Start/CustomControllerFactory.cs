using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using System.Reflection;
using System.Web.Routing;

namespace Blog.App_Start
{
    public class CustomControllerFactory : IControllerFactory
    {
        private IUnityContainer _container;

        public CustomControllerFactory(IUnityContainer container)
        {
            _container = container;
        }

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            if (controllerName == String.Empty)
                throw new ArgumentNullException("controllerName");

            String areaName = GetAreaName(requestContext);
            Type controllerType = GetControllerType(controllerName, areaName);
            ConstructorInfo constructor = GetInjectedConstructor(controllerType);

            var parameters = constructor.GetParameters();
            List<object> parametersInstances = new List<object>();

            for (int i = 0; i < parameters.Count(); i++)
            {
                parametersInstances.Add(_container.Resolve(parameters[i].ParameterType));
            }

            return Activator.CreateInstance(controllerType, parametersInstances.ToArray()) as IController;
        }

        public System.Web.SessionState.SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return System.Web.SessionState.SessionStateBehavior.Default;
        }

        public void ReleaseController(IController controller)
        {
            var asDisposable = controller as IDisposable;

            if (asDisposable != null)
            {
                asDisposable.Dispose();
            }
        }

        private Type GetControllerType(String controllerName, String areaName)
        {
            String assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            String controllerPath = String.Empty;

            if (areaName != String.Empty)
                controllerPath = String.Format("{0}.Areas.{1}.Controllers.{2}Controller", assemblyName, areaName, controllerName);
            else
                controllerPath = String.Format("{0}.Controllers.{1}Controller", assemblyName, controllerName);

            return Type.GetType(controllerPath);
        }

        private ConstructorInfo GetInjectedConstructor(Type controller)
        {
            var constructors = controller.GetConstructors();

            //for (int i = 0; i < constructors.Count(); i++)
            //{
                //var attributes = constructors[i].GetCustomAttributes(false);
                //if (attributes.FirstOrDefault(p => p.GetType() == typeof(InjectionConstructorAttribute)) != null)
                //{
                //    return constructors[i];
                //}
            //}

            return constructors.First();
        }

        private String GetAreaName(RequestContext requestContext)
        {
            if (requestContext.RouteData.DataTokens.Any(p => p.Key == "area"))
            {
                return requestContext.RouteData.DataTokens.First(p => p.Key == "area").Value.ToString();
            }

            return String.Empty;
        }
    }
}