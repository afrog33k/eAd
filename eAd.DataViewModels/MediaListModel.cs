namespace eAd.DataViewModels
{
    using System;
    using System.Runtime.CompilerServices;

    public class MediaListModel
    {
        private TimeSpan _duration;
        private string _Location;
        private long _MediaID;

        public bool Downloaded { get; set; }

        public TimeSpan Duration
        {
            get
            {
                return this._duration;
            }
            set
            {
                this._duration = value;
            }
        }

        public string Location
        {
            get
            {
                return this._Location;
            }
            set
            {
                this._Location = value;
            }
        }

        public long MediaID
        {
            get
            {
                return this._MediaID;
            }
            set
            {
                this._MediaID = value;
            }
        }
    }
}

