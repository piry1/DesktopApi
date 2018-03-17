using DesktopApi.Data.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DesktopApi.Crawler
{
    internal static class CategoryManager
    {
        private static readonly FlatFileDataStorage<Dictionary<string, string>> Categories =
            new FlatFileDataStorage<Dictionary<string, string>>("categories.json");

        private const string NonCategoryNameKey = "none";
        private const string DirectoryKey = "directory";

        static CategoryManager()
        {
            if (Categories.Elems.Count == 0)
                InitCategories();
        }

        internal static string GetFileCategory(string path)
        {
            var attr = File.GetAttributes(path);
            if (attr.HasFlag(FileAttributes.Directory))
                return Categories.Elems[DirectoryKey];
            var ext = Path.GetExtension(path);
            return Categories.Elems.ContainsKey(ext)
                ? Categories.Elems[ext]
                : Categories.Elems[NonCategoryNameKey];
        }

        private static void InitCategories()
        {
            string[] keys = { ".lnk", ".pdf", ".mp3", ".txt", ".jpg", ".png", DirectoryKey, NonCategoryNameKey, ".gif" };
            int[] valuesIndex = { 0, 1, 2, 1, 3, 3, 4, 5, 3 };
            string[] values = { "application", "document", "music", "image", "directory", "none" };

            for (int i = 0; i < keys.Length; ++i)
                Categories.Elems.Add(keys[i], values[valuesIndex[i]]);
            Categories.Serialize();
        }
    }
}
