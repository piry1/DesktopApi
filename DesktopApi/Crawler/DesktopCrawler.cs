using DesktopApi.Data.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DesktopApi.Crawler
{
    internal class DesktopCrawler
    {
        internal void Crwal(FlatFileDataStorage<Elem> dataStorage, IEnumerable<string> paths)
        {
            Console.WriteLine("*** crawler START ***");
            RemoveNotExistingPaths(dataStorage);
            AddNewPaths(dataStorage, paths.ToArray());
            Console.WriteLine("*** crawler END ***");
        }

        private void RemoveNotExistingPaths(FlatFileDataStorage<Elem> dataStorage)
        {
            dataStorage.Elems.Where(x => !x.Exist())
                .ToList().ForEach(i =>
                {
                    Console.WriteLine("remove: " + i.Name);
                    dataStorage.Elems.Remove(i);
                });
        }

        private void AddNewPaths(FlatFileDataStorage<Elem> dataStorage, string[] paths)
        {
            var pm = new PathElemManager();
            var elems = pm.GetAllPaths(paths);

            elems.ForEach(x =>
            {
                if (File.GetAttributes(x.Path) == FileAttributes.Hidden || dataStorage.Elems.Contains(x)) return;
                dataStorage.Elems.Add(x);
                Console.WriteLine("add: " + x.Name);
            });
        }
    }

}

