using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Text;

namespace DesktopApi.Server.WebServer
{
    public class HttpResponse
    {
        public string Type { get; set; }
        public byte[] Content { get; set; }
        public int Code { get; set; } = 200;

        public static HttpResponse ReturnJson(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            HttpResponse response = new HttpResponse
            {
                Type = "text/json",
                Content = Encoding.UTF8.GetBytes(json)
            };
            return response;
        }

        public static HttpResponse ReturnPage(string path)
        {
            byte[] content;

            try
            {
                content = File.ReadAllBytes(path);
            }
            catch
            {
                content = Encoding.UTF8.GetBytes("Page not found");
            }

            HttpResponse response = new HttpResponse()
            {
                Type = "text/html",
                Content = content
            };
            return response;
        }

        public static HttpResponse ReturnImage(Bitmap img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                HttpResponse response = new HttpResponse()
                {
                    Type = "image/png",
                    Content = ms.ToArray()
                };
                return response;
            }
        }
    }
}
