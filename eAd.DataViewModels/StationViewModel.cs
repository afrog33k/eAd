using System;

namespace eAd.DataViewModels
{
    public class StationViewModel
    {
        private bool _isOnline;

        public long StationID
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }

        public bool Available
        {
            get;
            set;
        }

        public string Location { get; set; }

        public DateTime? LastCheckIn { get; set; }

        public bool IsOnline
        {
            get {
                return _isOnline;
            }
            set {
                _isOnline = value;
            }
        }
    }
}