using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopApi.Data.Model;
using System.IO;

namespace DesktopApi.Crawler
{
    internal class PathElemManager
    {
        private string publicPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
        private string privatePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private CategoryManager cm = new CategoryManager();

        internal List<PathElem> GetAllPaths()
        {
            return GetFiles();
        }

        private List<PathElem> GetFiles()
        {
            List<PathElem> elems = new List<PathElem>();
            var f = Directory.GetFiles(privatePath);
            foreach (var file in f)
                elems.Add(new PathElem() { Path = file, Name = Path.GetFileNameWithoutExtension(file), Type = PathElem.PathType.File, Category = cm.GetFileCategory(file) });

            f = Directory.GetFiles(publicPath);
            foreach (var file in f)
                elems.Add(new PathElem() { Path = file, Name = Path.GetFileNameWithoutExtension(file), Type = PathElem.PathType.File, Category = cm.GetFileCategory(file) });

            var d = Directory.GetDirectories(privatePath);
            foreach (var dir in d)
                elems.Add(new PathElem() { Path = dir, Name = Path.GetFileNameWithoutExtension(dir), Type = PathElem.PathType.Directory, Category = cm.GetFileCategory(dir) });

            d = Directory.GetDirectories(publicPath);
            foreach (var dir in d)
                elems.Add(new PathElem() { Path = dir, Name = Path.GetFileNameWithoutExtension(dir), Type = PathElem.PathType.Directory, Category = cm.GetFileCategory(dir) });

            return elems;
        }

        public PathElem GetPathElem(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            PathElem pe = new PathElem()
            {
                Path = path,
                Name = Path.GetFileNameWithoutExtension(path),
                Category = cm.GetFileCategory(path),
                Type = attr.HasFlag(FileAttributes.Directory) ? PathElem.PathType.Directory : PathElem.PathType.File
            };

            return pe;
        }

    }
}
