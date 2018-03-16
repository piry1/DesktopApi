using System.Linq;
using DesktopApi.Crawler;
using static DesktopApi.Server.Database;

namespace DesktopApi.Server.WebServer.Controllers
{
    internal class DesktopController
    {
        public object Get()
        {
            var desktop = Desktop.Data.Elems.ToList();         
            return desktop;
        }

        public object Get(string c)
        {
            var desktop = Desktop.Data.Elems.Where(x => x.Category == c).ToList();        
            return desktop;
        }

        public object Changed(string key)
        {
            return DirectoryMonitor.GetChanged(key);
        }

    }
}
