using System;
using System.Collections.Generic;
using System.Linq;
using DesktopApi.Crawler;
using static DesktopApi.Server.Database;

namespace DesktopApi.Server.WebServer.Controllers
{
    public class CategoriesController
    {
        public object Get()
        {
            var categories = new List<string>();
            Desktop.Data.Elems.ToList().ForEach(x =>
            {
                if (!categories.Contains(x.Category))
                    categories.Add(x.Category);
            });

            categories.Sort();
            return categories;
        }

        public object RenameById(int id, string value)
        {
            try
            {
                foreach (var elem in Desktop.Data.Elems)
                    if (elem.Id == id)
                        elem.Category = value;
                Desktop.Data.Serialize();
                DirectoryMonitor.SetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public object Rename(string oldValue, string newValue)
        {
            try
            {
                foreach (var elem in Desktop.Data.Elems)
                    if (elem.Category == oldValue)
                        elem.Category = newValue;
                Desktop.Data.Serialize();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

    }
}
