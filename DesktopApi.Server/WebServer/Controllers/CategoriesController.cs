using System;
using System.Collections.Generic;
using System.Linq;
using static DesktopApi.Server.Database;

namespace DesktopApi.Server.WebServer.Controllers
{
    public class CategoriesController
    {
        public HttpResponse Get()
        {
            var categories = new List<string>();
            Desktop.Data.Elems.ToList().ForEach(x =>
            {
                if (!categories.Contains(x.Category))
                    categories.Add(x.Category);
            });

            categories.Sort();
            return HttpResponse.ReturnJson(categories);
        }


        public HttpResponse Rename(string oldValue, string newValue)
        {
            try
            {
                foreach (var elem in Desktop.Data.Elems)
                    if (elem.Category == oldValue)
                        elem.Category = newValue;
            }
            catch (Exception e)
            {
                return HttpResponse.ReturnJson(e);
            }
            return HttpResponse.ReturnJson("OK");
        }

    }
}
