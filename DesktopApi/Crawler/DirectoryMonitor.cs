using DesktopApi.Data.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DesktopApi.Crawler
{
    public class DirectoryMonitor
    {
        private readonly string[] _paths;
        private static readonly Dictionary<string, bool> Changed = new Dictionary<string, bool>();
        private readonly FlatFileDataStorage<List<Elem>> _dataStorage;

        internal DirectoryMonitor(FlatFileDataStorage<List<Elem>> dataStorage, IEnumerable<string> paths)
        {
            _dataStorage = dataStorage;
            _paths = paths.ToArray();
        }

        internal void StartMonitoring()
        {   
            foreach (var path in _paths)
            {
                var fsw = new FileSystemWatcher(path)
                {
                    IncludeSubdirectories = false,
                    EnableRaisingEvents = true,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName
                };

                fsw.Created += Fsw_Created;
                fsw.Renamed += Fsw_Renamed;
                fsw.Deleted += Fsw_Deleted;
            }
        }

        private void Fsw_Deleted(object sender, FileSystemEventArgs e)
        {
            var elem = _dataStorage.Elems.First(x => x.Path == e.FullPath);
            _dataStorage.Elems.Remove(elem);
            Console.WriteLine("remove: " + elem.Name);
            _dataStorage.Serialize();
            SetChanged();
        }

        private void Fsw_Renamed(object sender, RenamedEventArgs e)
        {
            var pem = new PathElemManager();
            var pe = pem.GetPathElem(e.FullPath);
            var elem = _dataStorage.Elems.First(x => x.Path == e.OldFullPath);
            _dataStorage.Elems.Remove(elem);
            pe.Id = _dataStorage.Elems.Count;
            _dataStorage.Elems.Add(pe);
            Console.WriteLine($"rename: {e.OldName} => {e.Name}");
            _dataStorage.Serialize();
            SetChanged();
        }

        private void Fsw_Created(object sender, FileSystemEventArgs e)
        {
            var pem = new PathElemManager();
            var pe = pem.GetPathElem(e.FullPath);
            pe.Id = _dataStorage.Elems.Count;
            _dataStorage.Elems.Add(pe);
            Console.WriteLine("add: " + pe.Name);
            _dataStorage.Serialize();
            SetChanged();
        }

        public static void SetChanged()
        {
            var keys = new List<string>();
            foreach (var k in Changed.Keys)
                keys.Add(k);
            foreach (var x in keys)
                Changed[x] = true;
        }

        public static bool GetChanged(string key)
        {
            bool result = true;
            if (Changed.ContainsKey(key))
            {
                result = Changed[key];
                Changed[key] = false;
            }
            else
            {
                Changed.Add(key, false);
            }
            return result;
        }
    }
}
