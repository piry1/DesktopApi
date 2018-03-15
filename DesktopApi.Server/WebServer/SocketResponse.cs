namespace DesktopApi.Server.WebServer
{
    class SocketResponse
    {
        public string Controller { get; set; }
        public string Method { get; set; }
        public object Response { get; set; }
    }
}
