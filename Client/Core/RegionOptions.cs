using System;
using System.Collections.Generic;
using eAd.DataViewModels;

namespace Client.Core
{
    /// <summary>
    /// The options specific to a region
    /// </summary>
    struct RegionOptions
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

        //xml
        public List<LayoutRegionMedia> mediaNodes;

        //rss options
        public string direction;
        public string text;
        public string documentTemplate;
        public string copyrightNotice;
        public string javaScript;
        public int updateInterval;
        public int scrollSpeed;

        //The identification for this region
        public string mediaid;
        public int layoutId;
        public string regionId;
        public int scheduleId;

        //general options
        public string backgroundImage;
        public string backgroundColor;

        public MediaDictionary Dictionary;

        public override string ToString()
        {
            return String.Format("({0},{1},{2},{3},{4},{5})", Width, Height, Top, Left, FileType, Uri);
        }
    }
}