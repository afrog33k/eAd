using System;

namespace eAd.Utilities
{
    public  class ClientConfiguration
    {
        public int MyStationID { get; set; }

        public int MessageWaitTime { get; set; }
        public string ServerUrl { get; set; }
        public string AppPath { get; set; }
        public double DefaultDuration { get; set; }
        public string PlayListFile { get; set; }
        public static string ConfigurationFile = "appconfig.xml";
        public ClientConfiguration()
        {
            MyStationID = 3;

            MessageWaitTime = 1000;
            ServerUrl = "http://1.9.13.61";
            AppPath = AppDomain.CurrentDomain.BaseDirectory;
            DefaultDuration = 10000;
            PlayListFile = "playlist.xml";
        }
    }
}