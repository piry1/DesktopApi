using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace DesktopApi.Data.Model
{
    public class FlatFileDataStorage<T>
    {
        private readonly string _fileDir = @"DataStorages/";
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
            File.WriteAllText(_fileDir + _fileName, jsonElems);
        }

        public void Deserialize()
        {
            Elems = JsonConvert.DeserializeObject<ObservableCollection<T>>(File.ReadAllText(_fileDir + _fileName));
        }

        private bool FileExist()
        {
            if (!Directory.Exists(_fileDir))
                Directory.CreateDirectory(_fileDir);
            return File.Exists(_fileDir + _fileName);
        }
    }
}
