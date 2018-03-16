using System;
using System.Linq;
using System.Reflection;
using DesktopApi.Server.WebServer.Controllers;

namespace DesktopApi.Server.WebServer
{
    internal class Router
    {
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once InconsistentNaming
        private DesktopController desktop = new DesktopController();
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once InconsistentNaming
        private FileController file = new FileController();
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once InconsistentNaming
        private DatabaseController database = new DatabaseController();
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once InconsistentNaming
        private CategoriesController categories = new CategoriesController();

        public object RouteApiMethod(Uri url)
        {
            string controllerName = url.Segments[1].Replace("/", "");
            string methodName = url.Segments[2].Replace("/", "");
            string[] strParams = url.Segments.Skip(3).Select(s => s.Replace("/", "")).ToArray();
            return RouteApiMethod(controllerName, methodName, strParams);
        }

        public object RouteApiMethod(Request request)
        {
            return RouteApiMethod(request.Controller, request.Method, request.Params);
        }

        public object RouteApiMethod(string controllerName, string methodName, string[] strParams)
        {
            MethodInfo method;
            Object param;
            object[] @params;

            try
            {
                param = GetType().GetField(controllerName, BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.GetValue(this);

                method = param?.GetType()
                                    .GetMethods()
                                    .First(m => m.Name.ToLower() == methodName && m.GetParameters().Count() == strParams.Count());

                @params = method?.GetParameters()
                                    .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                    .ToArray();

                string par = "";
                strParams.ToList().ForEach(x => par += x + " ");
                Console.WriteLine("controller: " + controllerName + " - " + methodName + " | " + par);
            }
            catch
            {
                return null;
            }

            return method?.Invoke(param, @params);
        }
    }
}
