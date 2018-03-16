using System.IO;
using DesktopApi.Crawler;

namespace DesktopApi.Data.Model
{
    public class Elem
    {
        public int Id { get; }
        public string Name { get; }
        public string Path { get; }
        public string Icon { get; }
        public string Category { get; set; }
        public PathType Type { get; }
        public uint UseCount { get; set; } = 0;
        public bool IsFavourite { get; set; } = false;

        public Elem(string path)
        {
            Path = path;
            Id = path.GetHashCode();
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            Type = GetPathType(path);
            Category = CategoryManager.GetFileCategory(path);
            Icon = IconManager.GetIcon(Type, path);
        }

        private PathType GetPathType(string path)
        {
            return File.GetAttributes(path)
                .HasFlag(FileAttributes.Directory)
                ? PathType.Directory
                : PathType.File;
        }

        public bool Exist()
        {
            return Type == PathType.Directory
                ? Directory.Exists(Path)
                : File.Exists(Path);
        }

        public override bool Equals(object obj)
        {
            return obj is Elem elem && Path.Equals(elem.Path);
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }
    }
}
