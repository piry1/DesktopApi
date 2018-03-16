using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DesktopApi.Data.Model;
using OSIcon;

namespace DesktopApi.Crawler
{
    internal static class IconManager
    {
        private const string IconsDirPath = @"Icons/";

        static IconManager()
        {
            if (!Directory.Exists(IconsDirPath))
                Directory.CreateDirectory(IconsDirPath);
        }

        public static string GetIcon(PathType type, string path)
        {
            string iconPath = IconsDirPath + path.GetHashCode() + ".png";
            if (File.Exists(iconPath))
                return Path.GetFullPath(iconPath);

            Bitmap bitmap = null;
            int tryouts = 0;

            while (bitmap == null && tryouts < 100)
            {
                bitmap = ExtractIcon(type, path);
                tryouts++;
            }

            bitmap?.Save(iconPath, ImageFormat.Png);
            bitmap?.Dispose();
            return Path.GetFullPath(iconPath);
        }

        private static Bitmap ExtractIcon(PathType type, string path)
        {
            var ii = type == PathType.File
                ? IconReader.GetFileIcon(path, IconSize.Jumbo)
                : IconReader.GetFolderIcon(IconSize.Jumbo, FolderState.Open);
            return ii?.Icon.ToBitmap();
        }
    }
}