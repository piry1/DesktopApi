using System.Linq;
using DesktopApi.Crawler;
using static DesktopApi.Server.Database;

namespace DesktopApi.Server.WebServer.Controllers
{
    internal class DesktopController
    {
        public HttpResponse Get()
        {
            var desktop = Desktop.Data.Elems.ToList();         
            return HttpResponse.ReturnJson(desktop);
        }

        public HttpResponse Get(string c)
        {
            var desktop = Desktop.Data.Elems.Where(x => x.Category == c).ToList();        
            return HttpResponse.ReturnJson(desktop);
        }

        public HttpResponse Changed(string key)
        {
            return HttpResponse.ReturnJson(DirectoryMonitor.GetChanged(key));
        }

    }
}
