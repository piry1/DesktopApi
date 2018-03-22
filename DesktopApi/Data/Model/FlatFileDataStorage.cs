﻿using Newtonsoft.Json;
using System;
using System.IO;

namespace DesktopApi.Data.Model
{
    public class FlatFileDataStorage<T> where T : new()
    {
        private const string FileDir = @"DataStorages/";
        private readonly string _fileName;

        public T Elems { get; set; }

        public FlatFileDataStorage(string fileName)
        {
            if (fileName == string.Empty)
                throw new Exception("Flat file database name not specified (empty string)");

            _fileName = fileName;

            if (!FileExist())
                Init();
            else
                Deserialize();
        }

        public void Serialize()
        {
            var jsonElems = JsonConvert.SerializeObject(Elems);
            File.WriteAllText(FileDir + _fileName, jsonElems);
        }

        public void Deserialize()
        {
            if (FileExist())
                Elems = JsonConvert.DeserializeObject<T>(File.ReadAllText(FileDir + _fileName));
            else
                Init();
        }

        private void Init()
        {
            Elems = new T();
            Serialize();
        }

        private bool FileExist()
        {
            if (!Directory.Exists(FileDir))
                Directory.CreateDirectory(FileDir);
            return File.Exists(FileDir + _fileName);
        }
    }
}
