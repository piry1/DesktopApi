using DesktopApi.Data.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DesktopApi.Crawler
{
    internal class PathElemManager
    {
      
        internal List<Elem> GetAllPaths(string[] paths)
        {
            var elems = new List<Elem>();

            foreach (var path in paths)
            {
                FillElemsList(Directory.GetFiles(path), elems);
                FillElemsList(Directory.GetDirectories(path), elems);
            }

            return elems;
        }

        private void FillElemsList(string[] paths, List<Elem> elems)
        {
            elems.AddRange(paths.Select(file => new Elem(file)));
        }

    }
}
