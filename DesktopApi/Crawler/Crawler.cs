using DesktopApiService.Database;
using DesktopApiService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DesktopApi.Crawler
{
    class DesktopCrawler
    {
        internal void Crwal()
        {
            Console.WriteLine("Crawler Start\n");
            RemoveNotExisting();
            AddNew();
            Console.WriteLine("Crawler End\n");
        }

        private void RemoveNotExisting()
        {
            using (var db = new Context())
            {
                db.PathElems.ToList().ForEach((x) =>
                {
                    if (!x.Exist())
                    {
                        db.PathElems.Remove(x);
                        Console.WriteLine("removed: " + x.Name);
                    }
                });
                db.SaveChanges();
            }
        }

        private void AddNew()
        {
            using (var db = new Context())
            {
                PathElemManager pm = new PathElemManager();
                var elems = pm.GetAllPaths();

                foreach (var elem in elems)
                {
                    if (elem.Name != "desktop.ini" && db.PathElems.FirstOrDefault(x => x.Path == elem.Path) == null)
                    {
                        db.PathElems.Add(elem);
                        db.SaveChanges();
                        Console.WriteLine("added: " + elem.Name);
                    }
                }

            }
        }
    }

}

