namespace eAd.DataViewModels
{
    using System;

    public class MediaViewModel
    {
        private string _Location;
        private long _MediaID;
        private string _Name;
        private string _Tags;
        private string _Type;

        public static MediaViewModel Empty
        {
            get
            {
                return new MediaViewModel { Name = "Invalid Media" };
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

        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        public string Tags
        {
            get
            {
                return this._Tags;
            }
            set
            {
                this._Tags = value;
            }
        }

        public string Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }
    }
}

