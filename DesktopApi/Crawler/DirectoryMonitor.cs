using DesktopApiService.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopApi.Crawler
{
    class DirectoryMonitor
    {
        private string privatePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private string publicPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
        private static Dictionary<string, bool> changed = new Dictionary<string, bool>();

        public void StartMonitoring()
        {
            string[] paths = new string[] { privatePath, publicPath };
            foreach (var path in paths)
            {
                FileSystemWatcher fsw = new FileSystemWatcher(path);
                fsw.IncludeSubdirectories = false;
                fsw.EnableRaisingEvents = true;
                fsw.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName;
                fsw.Created += Fsw_Created;
                fsw.Renamed += Fsw_Renamed;
                fsw.Deleted += Fsw_Deleted;
            }
        }

        private void Fsw_Deleted(object sender, FileSystemEventArgs e)
        {
            using (var db = new Context())
            {
                db.PathElems.Remove(db.PathElems.First(x => x.Path == e.FullPath));
                db.SaveChanges();
            }
            SetChanged();
        }

        private void Fsw_Renamed(object sender, RenamedEventArgs e)
        {
            PathElemManager pem = new PathElemManager();
            using (var db = new Context())
            {
                var pe = pem.GetPathElem(e.FullPath);
                db.PathElems.Remove(db.PathElems.First(x => x.Path == e.OldFullPath));
                db.PathElems.Add(pe);
                db.SaveChanges();
            }
            new Thread(() => DelayedHideIcon(e.FullPath)).Start();
            SetChanged();
        }

        private void Fsw_Created(object sender, FileSystemEventArgs e)
        {
            PathElemManager pem = new PathElemManager();
            using (var db = new Context())
            {
                var pe = pem.GetPathElem(e.FullPath);
                db.PathElems.Add(pe);
                db.SaveChanges();
            }
            new Thread(() => DelayedHideIcon(e.FullPath)).Start();
            SetChanged();
        }

        public static List<string> HideIcons(bool hide)
        {
            List<string> errors = new List<string>();

            using (var db = new Context())
            {
                var elems = db.PathElems.ToList();

                foreach (var elem in elems)
                {
                    var res = HideIcon(hide, elem.Path);
                    if (res != null)
                        errors.Add(res);
                }
            }

            return errors;
        }

        private static string HideIcon(bool hide, string path)
        {
            FileAttributes fa = hide ? FileAttributes.Hidden : FileAttributes.Normal;
            try
            {
                File.SetAttributes(path, fa);
            }
            catch
            {
                return path;
            }
            return null;
        }

        private static void DelayedHideIcon(string path)
        {
            Thread.Sleep(TimeSpan.FromSeconds(30));
            HideIcon(true, path);
        }

        public static void SetChanged()
        {
            List<string> keys = new List<string>();
            foreach (var k in changed.Keys)
                keys.Add(k.ToString());
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
