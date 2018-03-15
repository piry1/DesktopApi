using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using DesktopApi.Data.Model;

namespace DesktopApi.Crawler
{
    class DirectoryMonitor
    {
        private readonly string _privatePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private readonly string _publicPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
        private static Dictionary<string, bool> changed = new Dictionary<string, bool>();
        private FlatFileDataStorage<Elem> _dataStorage;

        public DirectoryMonitor(FlatFileDataStorage<Elem> dataStorage)
        {
            _dataStorage = dataStorage;
        }

        public void StartMonitoring()
        {
            string[] paths = { _privatePath, _publicPath };
            foreach (var path in paths)
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
            SetChanged();
        }

        private void Fsw_Created(object sender, FileSystemEventArgs e)
        {
            var pem = new PathElemManager();
            var pe = pem.GetPathElem(e.FullPath);
            pe.Id = _dataStorage.Elems.Count;
            _dataStorage.Elems.Add(pe);
            Console.WriteLine("add: " + pe.Name);

            SetChanged();
        }

        public static void SetChanged()
        {
            var keys = new List<string>();
            foreach (var k in changed.Keys)
                keys.Add(k);
            foreach (var x in keys)
                changed[x] = true;
        }

        public static bool GetChanged(string key)
        {
            bool result = true;
            if (changed.ContainsKey(key))
            {
                result = changed[key];
                changed[key] = false;
            }
            else
            {
                changed.Add(key, false);
            }
            return result;
        }
    }
}
