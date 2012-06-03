using eAd.DataViewModels;

namespace ClientApp.Core
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct RegionOptions
    {
        public double ScaleFactor;
        public int Width;
        public int Height;
        public int Top;
        public int Left;
        public int BackgroundLeft;
        public int BackgroundTop;
        public string FileType;
        public string Uri;
        public int Duration;
        public List<LayoutRegionMedia> mediaNodes;
        public string direction;
        public string text;
        public string documentTemplate;
        public string copyrightNotice;
        public string javaScript;
        public int updateInterval;
        public int scrollSpeed;
        public string mediaid;
        public int layoutId;
        public string regionId;
        public int scheduleId;
        public string backgroundImage;
        public string backgroundColor;
        public MediaDictionary Dictionary;
        public string Name { get; set; }
        public override string ToString()
        {
            return string.Format("({0},{1},{2},{3},{4},{5})", new object[] { this.Width, this.Height, this.Top, this.Left, this.FileType, this.Uri });
        }
    }
}

