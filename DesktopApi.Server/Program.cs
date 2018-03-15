using DesktopApi.Server.WebServer;

namespace DesktopApi.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            DesktopServer.StartServer();
            WebSocketServer webSocketServer = new WebSocketServer();
            webSocketServer.Start();
        }
    }
}
