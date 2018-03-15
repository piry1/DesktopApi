using System.IO;
using DesktopApi.Data.Model.Interfaces;

namespace DesktopApi.Data.Model
{
    public class Elem : IElem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public PathType Type { get; set; }
        public uint UseCount { get; set; } = 0;
        public bool IsFavourite { get; set; }

        public bool Exist()
        {
            return Type == PathType.Directory
                ? Directory.Exists(Path)
                : File.Exists(Path);
        }
    }
}
