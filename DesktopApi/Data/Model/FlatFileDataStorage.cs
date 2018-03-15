using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace DesktopApi.Data.Model
{
    public class FlatFileDataStorage<T>
    {
        private const string FileDir = @"DataStorages/";
        private readonly string _fileName;

        public ObservableCollection<T> Elems { get; set; }

        public FlatFileDataStorage(string fileName)
        {
            if (fileName == string.Empty)
                throw new Exception("Flat file database name not specified (empty string)");

            _fileName = fileName;

            if (!FileExist())
            {
                Elems = new ObservableCollection<T>();
                Serialize();
            }
            else
                Deserialize();

            Elems.CollectionChanged += HandleElemsChange;
        }

        private void HandleElemsChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            Serialize();
        }

        public void Serialize()
        {
            var jsonElems = JsonConvert.SerializeObject(Elems);
            File.WriteAllText(FileDir + _fileName, jsonElems);
        }

        public void Deserialize()
        {
            Elems = JsonConvert.DeserializeObject<ObservableCollection<T>>(File.ReadAllText(FileDir + _fileName));
        }

        private bool FileExist()
        {
            if (!Directory.Exists(FileDir))
                Directory.CreateDirectory(FileDir);
            return File.Exists(FileDir + _fileName);
        }
    }
}
