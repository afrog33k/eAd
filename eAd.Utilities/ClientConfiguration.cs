namespace eAd.Utilities
{
    using System;
    using System.Runtime.CompilerServices;

    public class ClientConfiguration
    {
        public static string ConfigurationFile = "appconfig.xml";

        public ClientConfiguration()
        {
            this.MyStationID = 0x11;
            this.MessageWaitTime = 0x3e8;
            this.ServerUrl = "http://1.9.13.61";
            this.AppPath = AppDomain.CurrentDomain.BaseDirectory;
            this.DefaultDuration = 10000.0;
            this.PlayListFile = "playlist.xml";
        }

        public string AppPath { get; set; }

        public double DefaultDuration { get; set; }

        public int MessageWaitTime { get; set; }

        public int MyStationID { get; set; }

        public string PlayListFile { get; set; }

        public string ServerUrl { get; set; }

        public long CurrentMosaic { get; set; }
    }
}

