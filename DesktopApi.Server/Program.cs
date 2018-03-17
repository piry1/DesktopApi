using System;
using System.Threading;
using DesktopApi.Server.WebServer;

namespace DesktopApi.Server
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var webSocketServer = new WebSocketServer();
#if DEBUG
            var thread = webSocketServer.Start(5000);
#else
            var thread = webSocketServer.Start(5001);
#endif
            thread.Join();
        }
    }
}
