using DesktopApi.Data.Model;
using System;
using System.IO;
using System.Linq;

namespace DesktopApi.Crawler
{
    public class DesktopCrawler
    {
        internal void Crwal(FlatFileDataStorage<Elem> dataStorage)
        {
            Console.WriteLine("Crawler Start\n");
            RemoveNotExistingPaths(dataStorage);
            AddNewPaths(dataStorage);
            Console.WriteLine("Crawler End\n");
        }

        private void RemoveNotExistingPaths(FlatFileDataStorage<Elem> dataStorage)
        {
            dataStorage.Elems.Where(x => !x.Exist())
                .ToList().ForEach(i =>
                {
                    Console.WriteLine("removed: " + i.Name);
                    dataStorage.Elems.Remove(i);
                });
        }

        private void AddNewPaths(FlatFileDataStorage<Elem> dataStorage)
        {
            var pm = new PathElemManager();
            var elems = pm.GetAllPaths();

            elems.ForEach(x =>
            {
                if (File.GetAttributes(x.Path) == FileAttributes.Hidden || dataStorage.Elems.Contains(x)) return;
                dataStorage.Elems.Add(x);
                Console.WriteLine("added: " + x.Name);
            });
        }
    }

}

