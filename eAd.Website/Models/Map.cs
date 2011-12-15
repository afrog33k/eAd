using System.Collections.Generic;

namespace eAd.Website.Models
{
    public class Map
    {
        public string Name { get; set; }
        public LatLng LatLng { get; set; }
        public int Zoom { get; set; }

        private List<Location>  _locations = new List<Location>();

        public List<Location> Locations
        {
            get { return _locations; }
            set { _locations = value; }
        }
    }
}