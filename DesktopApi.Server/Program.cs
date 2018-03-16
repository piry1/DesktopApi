using System;
using DesktopApi.Server.WebServer;

namespace DesktopApi.Server
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            DesktopServer.StartServer();
            var webSocketServer = new WebSocketServer();
            webSocketServer.Start();
        }
    }
}
