using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using DesktopApi.Data.Model;

namespace DesktopApi.Crawler
{
    internal class CategoryManager
    {
        private Dictionary<string, string> dictionary = new Dictionary<string, string>();
        private string _nonCategoryNameKey = "none";
        private string _directoryKey = "directory";

        public CategoryManager()
        {
            //  LoadDictionary();
        }

        public string GetFileCategory(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            string ext = string.Empty;

            if (attr.HasFlag(FileAttributes.Directory))
                return "directory";
            else
                ext = Path.GetExtension(path);

            //if (dictionary.ContainsKey(ext))
            //    return dictionary[ext];

            return "none"; //dictionary[nonCategoryNameKey];
        }

    }
}
