using DesktopApi.Data.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DesktopApi.Crawler
{
    internal class PathElemManager
    {
        private readonly CategoryManager _cm = new CategoryManager();

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

        internal Elem GetPathElem(string path)
        {
            var elem = new Elem
            {
                Path = path,
                Name = Path.GetFileNameWithoutExtension(path),
                Category = _cm.GetFileCategory(path),
                Type = GetPathType(path)
            };

            return elem;
        }

        private void FillElemsList(string[] paths, List<Elem> elems)
        {
            elems.AddRange(paths.Select(file => new Elem
            {
                Id = elems.Count,
                Path = file,
                Name = Path.GetFileNameWithoutExtension(file),
                Type = GetPathType(file),
                Category = _cm.GetFileCategory(file)
            }));
        }

        private PathType GetPathType(string path)
        {
            return File.GetAttributes(path)
                .HasFlag(FileAttributes.Directory)
                ? PathType.Directory
                : PathType.File;
        }

    }
}
