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
        //   private static readonly Dictionary<string, bool> Changed = new Dictionary<string, bool>();
        private readonly FlatFileDataStorage<List<Elem>> _dataStorage;

        public static event EventHandler Changed;

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
            SetChanged(EventArgs.Empty);
        }

        private void Fsw_Renamed(object sender, RenamedEventArgs e)
        {
            var elem = _dataStorage.Elems.First(x => x.Path == e.OldFullPath);
            _dataStorage.Elems.Remove(elem);
            _dataStorage.Elems.Add(new Elem(e.FullPath));
            Console.WriteLine($"rename: {e.OldName} => {e.Name}");
            _dataStorage.Serialize();
            SetChanged(EventArgs.Empty);
        }

        private void Fsw_Created(object sender, FileSystemEventArgs e)
        {
            _dataStorage.Elems.Add(new Elem(e.FullPath));
            Console.WriteLine("add: " + e.Name);
            _dataStorage.Serialize();
            SetChanged(EventArgs.Empty);
        }

        public static void SetChanged(EventArgs e)
        {
            Changed?.Invoke(null, e);
        }

    }
}
