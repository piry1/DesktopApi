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
            var thread = webSocketServer.Start();
            thread.Join();
        }
    }
}
