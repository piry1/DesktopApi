using System;

namespace DesktopApi.Server.WebServer
{
    public class DesktopChangedEventArgs : EventArgs
    {
        public int ChangingEntity { get; }

        public DesktopChangedEventArgs(int entity)
        {
            ChangingEntity = entity;
        }
    }
}