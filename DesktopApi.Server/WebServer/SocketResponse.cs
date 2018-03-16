namespace DesktopApi.Server.WebServer
{
    internal class SocketResponse
    {
        public string Controller { get; set; }
        public string Method { get; set; }
        public object Response { get; set; }
    }
}
