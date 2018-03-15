using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using DesktopApi.Data.Model.Interfaces;

namespace DesktopApi.Data.Model
{
    public class DataStorage : IDataStorage
    {
        private readonly string _fileDir = @"Data/";
        private readonly string _fileName = @"DataStorage.json";

        public List<Elem> Elems { get; set; }

        public DataStorage()
        {
            Init();
        }

        public void Serialize()
        {
            var jsonElems = JsonConvert.SerializeObject(Elems);
            File.WriteAllText(_fileDir + _fileName, jsonElems);
        }

        public void Deserialize()
        {
            Elems = JsonConvert.DeserializeObject<List<Elem>>(_fileDir + _fileName);
        }

        private void Init()
        {
            if (!FileExist())
            {
                Elems = new List<Elem>();
                Serialize();
            }
            else
            {
                Deserialize();
            }
        }

        private bool FileExist()
        {
            if (!Directory.Exists(_fileDir))
                Directory.CreateDirectory(_fileDir);
            return File.Exists(_fileDir + _fileName);
        }
    }
}
