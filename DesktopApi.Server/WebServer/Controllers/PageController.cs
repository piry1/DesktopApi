namespace DesktopApi.Server.WebServer.Controllers
{
    internal class PageController
    {
        public HttpResponse Help()
        {
            return HttpResponse.ReturnPage(@"HtmlPages\help.html");
        }
    }
}
