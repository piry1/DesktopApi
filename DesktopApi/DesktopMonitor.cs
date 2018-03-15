using System;
using DesktopApi.Crawler;
using DesktopApi.Data.Model;

namespace DesktopApi
{
    public class DesktopMonitor
    {
        /// <summary>
        /// List of elements that are displayed on desktop
        /// </summary>
        public FlatFileDataStorage<Elem> Data { get; } = new FlatFileDataStorage<Elem>("desktop.json");

        private readonly DesktopCrawler _crawler = new DesktopCrawler();
        private readonly string[] _paths =
        {
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
        };

        /// <summary>
        /// Monitors desktop files and directories
        /// </summary>
        public DesktopMonitor()
        {
            _crawler.Crwal(Data, _paths);
            var directoryMonitor = new DirectoryMonitor(Data, _paths);
            directoryMonitor.StartMonitoring();
        }
    }
}