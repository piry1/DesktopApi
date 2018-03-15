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
        private string nonCategoryNameKey = "none";
        private string directoryKey = "directory";
        public CategoryManager()
        {
            LoadDictionary();
        }

        public string GetFileCategory(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            string ext = string.Empty;

            if (attr.HasFlag(FileAttributes.Directory))
                ext = "directory";
            else
                ext = Path.GetExtension(path);

            if (dictionary.ContainsKey(ext))
                return dictionary[ext];

            return dictionary[nonCategoryNameKey];
        }

        public void AddCategory(string key, string value)
        {
            try
            {
                using (var db = new Context())
                {
                    // add category
                    db.CategorySelectors.Add(new CategorySelector() { Key = key, Value = value });
                    db.SaveChanges();
                    LoadDictionary();

                    // update path elements by new category
                    var nonCategory = dictionary[nonCategoryNameKey];
                    db.PathElems.Where(x => x.Category == nonCategory).ToList()
                                .ForEach(x => x.Category = GetFileCategory(x.Path));

                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
                //throw new Exception("Category already exist");
            }
        }

        public void UpdateCategory(string key, string newValue)
        {
            using (var db = new Context())
            {
                // rename selector value
                var cs = db.CategorySelectors.First(x => x.Key == key);
                var oldValue = cs.Value;
                cs.Value = newValue;
                db.SaveChanges();
                LoadDictionary();
                // rename values in PathElems table
                db.PathElems.Where(x => x.Category == oldValue).ToList()
                            .ForEach(x => x.Category = GetFileCategory(x.Path));

                db.SaveChanges();
            }

        }

        public void UpdateCategoryValue(string oldValue, string newValue)
        {
            using (var db = new Context())
            {
                // updates value in CategorySelectors table
                db.CategorySelectors.Where(x => x.Value == oldValue)
                    .ToList()
                    .ForEach(x => x.Value = newValue);

                // update values in PathElems table
                db.PathElems.Where(x => x.Category == oldValue)
                    .ToList()
                    .ForEach(x => x.Category = newValue);

                db.SaveChanges();
            }
            LoadDictionary();
        }

        public void RemoveCategoryByKey(string key)
        {
            if (!SecureRemove(key))
                return;
            using (var db = new Context())
            {
                // remove selector
                db.CategorySelectors.Remove(
                    db.CategorySelectors.First(x => x.Key == key)
                    );
                db.SaveChanges();
                LoadDictionary();
                // update path elements
                db.PathElems.ToList()
                            .ForEach(x => x.Category = GetFileCategory(x.Path));
                db.SaveChanges();
            }

        }

        public void RemoveCategoryByValue(string value)
        {
            using (var db = new Context())
            {
                // remove selectors with this value
                db.CategorySelectors.RemoveRange(
                    db.CategorySelectors.Where(x => x.Value == value && x.Key != directoryKey && x.Key != nonCategoryNameKey)
                    );
                LoadDictionary();
                // replace this category in PathElems table to NonCategoryName 
                var nonCategory = dictionary[nonCategoryNameKey];
                db.PathElems.Where(x => x.Category == value).ToList()
                            .ForEach(x => x.Category = nonCategory);
                db.SaveChanges();
            }
        }

        private void InitDictionary(Context db)
        {
            string[] keys = new string[] { ".lnk", ".pdf", ".mp3", ".txt", ".jpg", ".png", directoryKey, nonCategoryNameKey, ".gif" };
            int[] valuesIndex = new int[] { 0, 1, 2, 1, 3, 3, 4, 5, 3 };
            string[] values = new string[] { "application", "document", "music", "image", "directory", "none" };
            if (keys.Length != valuesIndex.Length)
                throw new Exception();
            for (int i = 0; i < keys.Length; ++i)
                db.CategorySelectors.Add(new CategorySelector() { Key = keys[i], Value = values[valuesIndex[i]] });
        }

        private void LoadDictionary()
        {
            using (var db = new Context())
            {
                dictionary.Clear();
                if (db.CategorySelectors.Count() == 0)
                {
                    InitDictionary(db);
                    db.SaveChanges();
                }
                db.CategorySelectors.ToList().ForEach(x => dictionary.Add(x.Key, x.Value));
            }
        }

        private bool SecureRemove(string key)
        {
            if (key == nonCategoryNameKey || key == directoryKey)
                return false;
            return true;
        }
    }
}
