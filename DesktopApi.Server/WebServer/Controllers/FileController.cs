using DesktopApi.Server.ContextMenu;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static DesktopApi.Server.Database;

namespace DesktopApi.Server.WebServer.Controllers
{
    internal class FileController
    {
        private static ShellContextMenu _ctxMnu;

        #region Get Cursor position on screen
        public static Point GetMousePositionWindowsForms()
        {
            var point = Control.MousePosition;
            return new Point(point.X, point.Y);
        }

        internal class NativeMethods
        {
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetCursorPos(ref Win32Point pt);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        }

        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            NativeMethods.GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
        #endregion

        public HttpResponse Start(int id)
        {
            var elem = Desktop.Data.Elems.First(e => e.Id == id);
            try
            {
                Process.Start(elem.Path);
                Desktop.Data.Elems[Desktop.Data.Elems.IndexOf(elem)].UseCount++;
                Desktop.Data.Serialize();
            }
            catch
            {
                return HttpResponse.ReturnJson("error");
            }
            return HttpResponse.ReturnJson("OK");
        }

        public HttpResponse OpenMenu(int id)
        {
            var path = Desktop.Data.Elems.First(c => c.Id == id).Path;
            FileInfo[] arrFi = new FileInfo[1];
            arrFi[0] = new FileInfo(path);
            CloseMenu();
            new Thread(() => ShowCM(arrFi)).Start();
            return HttpResponse.ReturnJson("OK");
        }


        private void ShowCM(FileInfo[] f)
        {
            _ctxMnu = new ShellContextMenu();
            _ctxMnu.ShowContextMenu(f, GetMousePosition());
        }

        public HttpResponse CloseMenu()
        {
            try
            {
                _ctxMnu.DestroyHandle();
            }
            catch
            {
                return HttpResponse.ReturnJson("ups");
            }
            return HttpResponse.ReturnJson("OK");
        }
    }
}
