using eAd.DataViewModels;
using eAd.Website.eAdDataService;
//using StationViewModel = eAd.Website.eAdDataService.StationViewModel;

namespace eAd.Website
{
    public class GoogleMaps
    {

        public static StationViewModel[] Locations;

        public static string Webpage
        {

            get
            {
                return
                    @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN""
    ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml""  xmlns:v=""urn:schemas-microsoft-com:vml"">
  <head>
    <meta http-equiv=""content-type"" content=""text/html; charset=utf-8""/>
    <title>Google maps Navigator - Developed by Deadkota</title>
    <script src="" http://maps.google.com.au/?file=api&amp;v=3.x&amp;key=ABQIAAAAYyVTDBbP_v7seVpRwE6XsRTmUW7xDOfdW5uMV3ILvZqGgxktdhTvafjwdMeZGhyNcWVaKkbD6H7waQ""
      type=""text/javascript"">
      </script>
      
    <style type=""text/css"">
      body {
        font-family: Verdana, Arial, sans serif;
        font-size: 11px;
        margin: 2px;
      }
      table.directions th {
	background-color:#EEEEEE;
      }
	  
      img {
        color: #000000;
      }
    </style>
    <script type=""text/javascript"">

        var map;
        var gdir;
        var geocoder = null;
        var addressMarker;
        var route;

        function initialize() {
            if (GBrowserIsCompatible()) {
                map = new GMap2(document.getElementById(""map_canvas""),
                {
                    zoom: 5
                }
                );
                " +
                    "setCenter(" + Locations[0].Location + ");" +
                    @"
            }
        }

        function setCenter(lat, lng) {
            map.setCenter(new GLatLng(lat, lng), 13);
            map.clearOverlays();
            addDefaultMarkers();
        }
        function addDefaultMarkers() {
            var bounds = new google.maps.LatLngBounds();
            var point = map.getCenter();
            // Create our ""tiny"" marker icon
            var blueIcon = new GIcon(G_DEFAULT_ICON);
            blueIcon.image = ""http://www.google.com/intl/en_us/mapfiles/ms/micons/blue-dot.png"";

            // Set up our GMarkerOptions object
            markerOptions = { icon: blueIcon };
" +
                    AllLocationsText +
                    @"
            
        }

       
    </script>

  </head>
  <body onload=""initialize()"" onunload=""GUnload()"" scroll=""no"">
   <table class=""directions"">
    <tr>
        <td colspan=""2"">
	        <div id=""getStatus""></div>
        </td>
    </tr>
    <tr>
    <td valign=""top""><div id=""directions"" style=""width: 1000px; display:none;""></div></td>
    <td valign=""top""><div id=""map_canvas"" style=""width: 1000px; height: 1000px;""></div></td>

    </tr>
    </table> 
  </body>
</html>
";
            }

        }

        public static int CurrentStationID;

        protected static string AllLocationsText
        {
            get
            {
                string finalLocation = "";
                foreach (var location in Locations)
                {
                    finalLocation +=
                        @"var latlng = new GLatLng(" + location.Location +
                        @");
            bounds.extend(latlng);
            var marker = new GMarker(latlng" +
                        (location.Available ? ", markerOptions" : "") + @");
            map.addOverlay(marker);" +
                        (location.StationID == CurrentStationID
                             ? "marker.openInfoWindowHtml('You Are Here');"
                             : "");
                }
                return finalLocation;
            }

        }
    }
}
