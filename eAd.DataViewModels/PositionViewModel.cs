namespace eAd.DataViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class PositionViewModel
    {
        private string _contentURL;
        private double? _height;
        private long _mosaicID;
        private string _name;
        private long _positionID;
        private double? _width;
        private double? _x;
        private double? _y;

        public static PositionViewModel CreatePosition(long positionID)
        {
            return new PositionViewModel { PositionID = positionID };
        }

        public string ContentURL
        {
            get
            {
                return this._contentURL;
            }
            set
            {
                this._contentURL = value;
            }
        }

        public double? Height
        {
            get
            {
                return this._height;
            }
            set
            {
                this._height = value;
            }
        }

        public List<MediaListModel> Media { get; set; }

        public List<string> MediaUri { get; set; }

        public long MosaicID
        {
            get
            {
                return this._mosaicID;
            }
            set
            {
                this._mosaicID = value;
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public long PositionID
        {
            get
            {
                return this._positionID;
            }
            set
            {
                if (this._positionID != value)
                {
                    this._positionID = value;
                }
            }
        }

        public double? Width
        {
            get
            {
                return this._width;
            }
            set
            {
                this._width = value;
            }
        }

        public double? X
        {
            get
            {
                return this._x;
            }
            set
            {
                this._x = value;
            }
        }

        public double? Y
        {
            get
            {
                return this._y;
            }
            set
            {
                this._y = value;
            }
        }
    }
}

