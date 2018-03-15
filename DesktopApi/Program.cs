using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using DesktopApi.Data.Model;
using DesktopApi.Crawler;

namespace DesktopApi
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine("Start");

            FlatFileDataStorage<Elem> desktop = new FlatFileDataStorage<Elem>("desktop.json");
            DesktopCrawler crawler = new DesktopCrawler();
            crawler.Crwal(desktop);


            Console.WriteLine("End");

            Console.ReadLine();
        }
    }
}
