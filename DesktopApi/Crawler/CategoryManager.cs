using DesktopApi.Data.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DesktopApi.Crawler
{
    internal class CategoryManager
    {
        private readonly FlatFileDataStorage<KeyValuePair<string, string>> _categories =
            new FlatFileDataStorage<KeyValuePair<string, string>>("categories.json");

        private const string NonCategoryNameKey = "none";
        private const string DirectoryKey = "directory";

        internal CategoryManager()
        {
            if (_categories.Elems.Count == 0)
                InitCategories();
        }

        internal string GetFileCategory(string path)
        {
            var attr = File.GetAttributes(path);
            if (attr.HasFlag(FileAttributes.Directory))
                return "directory";
            var ext = Path.GetExtension(path);
            var category = _categories.Elems.Where(x => x.Key == ext).ToList();
            return category.Count != 0 ? category.First().Value : "none";
        }

        private void InitCategories()
        {
            string[] keys = { ".lnk", ".pdf", ".mp3", ".txt", ".jpg", ".png", DirectoryKey, NonCategoryNameKey, ".gif" };
            int[] valuesIndex = { 0, 1, 2, 1, 3, 3, 4, 5, 3 };
            string[] values = { "application", "document", "music", "image", "directory", "none" };

            for (int i = 0; i < keys.Length; ++i)
                _categories.Elems.Add(new KeyValuePair<string, string>(keys[i], values[valuesIndex[i]]));
        }
    }
}
