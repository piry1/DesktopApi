using System;
using System.Linq;
using System.Reflection;
using DesktopApi.Server.WebServer.Controllers;

namespace DesktopApi.Server.WebServer
{
    internal class Router
    {
        private DesktopController desktop = new DesktopController();
        private IconController icon = new IconController();
        private FileController file = new FileController();
        private DatabaseController database = new DatabaseController();
        private PageController page = new PageController();
        private CategoriesController categories = new CategoriesController();

        public HttpResponse RouteApiMethod(Uri url)
        {
            string controllerName = url.Segments[1].Replace("/", "");
            string methodName = url.Segments[2].Replace("/", "");
            string[] strParams = url.Segments.Skip(3).Select(s => s.Replace("/", "")).ToArray();
            return RouteApiMethod(controllerName, methodName, strParams);
        }

        public HttpResponse RouteApiMethod(Request request)
        {
            return RouteApiMethod(request.Controller, request.Method, request.Params);
        }

        public HttpResponse RouteApiMethod(string controllerName, string methodName, string[] strParams)
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
                var resp = page.Help();
                resp.Code = 400;
                return resp;
            }

            return method?.Invoke(param, @params) as HttpResponse;
        }
    }
}
