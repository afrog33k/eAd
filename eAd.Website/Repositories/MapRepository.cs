using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eAd.DataAccess;
using eAd.Website.Models;

namespace eAd.Website.Repositories
{
public class MapRepository
{
    eAd.DataAccess.eAdEntities _eAdEntities = new eAdEntities();

    public Map GetById(int id)
    {
        var station = _eAdEntities.Stations.Where(s => s.StationID == id).FirstOrDefault();
        var coords = station.Location.Split(',');

        return new Map
        {
            Name = station.Name,
            Zoom = 1,
            LatLng = new LatLng { Latitude = Convert.ToDouble(coords[0]), Longitude = Convert.ToDouble(coords[1]), },
            Locations =
            {
                new Location
                {
                    Name = station.Name,
                    LatLng =new LatLng { Latitude = Convert.ToDouble(coords[0]), Longitude = Convert.ToDouble(coords[1]), },
                    Image = "electricpumpicon.png"
                },

            }
        };
    }
}
}