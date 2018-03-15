using OSIcon;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using DesktopApi.Data.Model;
using static DesktopApi.Server.Database;

namespace DesktopApi.Server.WebServer.Controllers
{
    internal class IconController
    {
        public HttpResponse Get(int id)
        {
            int tryouts = 0;
            Bitmap img = null;

            var elem = Desktop.Data.Elems.First(x => x.Id == id);
            var graphicExtensions = new HashSet<string> { ".jpg", ".png", ".bmp", ".jpeg", ".gif" };
            var extension = Path.GetExtension(elem.Path)?.ToLower();
            if (graphicExtensions.Contains(extension))
                img = GetThumb(elem.Path);
            else
                while (img == null && tryouts < 100)
                {
                    img = GetIon(elem.Type, elem.Path);
                    ++tryouts;
                }

            return HttpResponse.ReturnImage(img);
        }

        private Bitmap GetIon(PathType type, string path)
        {
            var ii = type == PathType.File
                ? IconReader.GetFileIcon(path, IconSize.Jumbo)
                : IconReader.GetFolderIcon(IconSize.Jumbo, FolderState.Open);
            return ii?.Icon.ToBitmap();
        }

        private Bitmap GetThumb(string path)
        {
            var img = Image.FromFile(path).GetThumbnailImage(100, 100, null, IntPtr.Zero);
            return new Bitmap(img);
        }
    }
}
