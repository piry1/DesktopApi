using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading;

namespace DesktopApi.Server.WebServer
{
    internal static class DesktopServer
    {
        private static readonly HttpListener HttpListener = new HttpListener();
        private static Thread _responseThread;
        private static readonly Router Router = new Router();
        public static int Port { get; } = 5001;
        public static string Url { get; } = $"http://localhost:{Port}/";

        public static void StartServer()
        {
            Console.WriteLine("Starting server...");

            try
            {
                HttpListener.Prefixes.Add(Url);
               // HttpListener.Prefixes.Add("http://*:5432/");
                HttpListener.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Server started at " + Url);
            _responseThread = new Thread(ResponseThread);
            _responseThread.Start();
        }

        public static void StopServer()
        {
            _responseThread.Abort();
        }

        private static void ResponseThread()
        {
            while (true)
            {
                try
                {
                    var context = HttpListener.GetContext();
                    context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                    QueueUserWorkItem(context);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private static void QueueUserWorkItem(HttpListenerContext context)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    HttpResponse ret = Router.RouteApiMethod(context.Request.Url);
                    context.Response.ContentType = ret.Type;
                    context.Response.OutputStream.Write(ret.Content, 0, ret.Content.Length);
                    context.Response.StatusCode = ret.Code;
                }
                catch (Exception e)
                {
                    var errorRes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e));
                    context.Response.OutputStream.Write(errorRes, 0, errorRes.Length);
                    context.Response.StatusCode = 500;
                }

                context.Response.KeepAlive = false;
                context.Response.Close();
            });
        }
    }

}
