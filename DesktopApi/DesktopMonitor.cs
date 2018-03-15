using System;
using DesktopApi.Crawler;
using DesktopApi.Data.Model;

namespace DesktopApi
{
    public class DesktopMonitor
    {
        public FlatFileDataStorage<Elem> Data { get; } = new FlatFileDataStorage<Elem>("desktop.json");

        private readonly DesktopCrawler _crawler = new DesktopCrawler();
        private readonly string[] _paths =
        {
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
        };

        public DesktopMonitor()
        {
            _crawler.Crwal(Data, _paths);
            var directoryMonitor = new DirectoryMonitor(Data, _paths);
            directoryMonitor.StartMonitoring();
        }
    }
}