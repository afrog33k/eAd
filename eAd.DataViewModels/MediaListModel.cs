﻿namespace eAd.DataViewModels
{
    using System;
    using System.Runtime.CompilerServices;

    public class MediaListModel
    {
        public bool Downloaded { get; set; }

        public TimeSpan Duration { get; set; }

        public string Location { get; set; }

        public long MediaID { get; set; }

        public bool Selected { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }
    }
}

