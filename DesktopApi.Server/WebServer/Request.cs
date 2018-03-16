namespace DesktopApi.Server.WebServer
{
    internal class Request
    {
        public string Controller { get; set; }
        public string Method { get; set; }
        public string[] Params { get; set; }
    }
}
