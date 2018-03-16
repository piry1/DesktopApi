using System.Drawing;
using DesktopApi.Data.Model;
using OSIcon;

namespace DesktopApi.Crawler
{
    internal class IconManager
    {
        private Bitmap GetIcon(PathType type, string path)
        {
            var ii = type == PathType.File
                ? IconReader.GetFileIcon(path, IconSize.Jumbo)
                : IconReader.GetFolderIcon(IconSize.Jumbo, FolderState.Open);
            return ii?.Icon.ToBitmap();
        }
    }
}