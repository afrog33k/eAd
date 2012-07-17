namespace eAd.Website
{

using eAd.DataViewModels;

using System;



public class GoogleMaps
{

    public static int CurrentStationID;

    public static StationViewModel[] Locations;



    protected static string AllLocationsText
    {

        get
        {

            string str = "";

            foreach (StationViewModel model in Locations)
            {

                string str3 = str;

                str = str3 + "var latlng = new GLatLng(" + model.Location + ");\r\n            bounds.extend(latlng);\r\n            var marker = new GMarker(latlng" + (model.Available ? ", markerOptions" : "") + ");\r\n            map.addOverlay(marker);" + ((model.StationID == CurrentStationID) ? "marker.openInfoWindowHtml('You Are Here');" : "");

            }

            return str;

        }

    }



    public static string Webpage
    {

        get
        {

            return ("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\"\r\n    \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\"  xmlns:v=\"urn:schemas-microsoft-com:vml\">\r\n  <head>\r\n    <meta http-equiv=\"content-type\" content=\"text/html; charset=utf-8\"/>\r\n    <title>Google maps Navigator - Developed by Deadkota</title>\r\n    <script src=\" http://maps.google.com.au/?file=api&amp;v=3.x&amp;key=ABQIAAAAYyVTDBbP_v7seVpRwE6XsRTmUW7xDOfdW5uMV3ILvZqGgxktdhTvafjwdMeZGhyNcWVaKkbD6H7waQ\"\r\n      type=\"text/javascript\">\r\n      </script>\r\n      \r\n    <style type=\"text/css\">\r\n      body {\r\n        font-family: Verdana, Arial, sans serif;\r\n        font-size: 11px;\r\n        margin: 2px;\r\n      }\r\n      table.directions th {\r\n\tbackground-color:#EEEEEE;\r\n      }\r\n\t  \r\n      img {\r\n        color: #000000;\r\n      }\r\n    </style>\r\n    <script type=\"text/javascript\">\r\n\r\n        var map;\r\n        var gdir;\r\n        var geocoder = null;\r\n        var addressMarker;\r\n        var route;\r\n\r\n        function initialize() {\r\n            if (GBrowserIsCompatible()) {\r\n                map = new GMap2(document.getElementById(\"map_canvas\"),\r\n                {\r\n                    zoom: 5\r\n                }\r\n                );\r\n                setCenter(" + Locations[0].Location + ");\r\n            }\r\n        }\r\n\r\n        function setCenter(lat, lng) {\r\n            map.setCenter(new GLatLng(lat, lng), 13);\r\n            map.clearOverlays();\r\n            addDefaultMarkers();\r\n        }\r\n        function addDefaultMarkers() {\r\n            var bounds = new google.maps.LatLngBounds();\r\n            var point = map.getCenter();\r\n            // Create our \"tiny\" marker icon\r\n            var blueIcon = new GIcon(G_DEFAULT_ICON);\r\n            blueIcon.image = \"http://www.google.com/intl/en_us/mapfiles/ms/micons/blue-dot.png\";\r\n\r\n            // Set up our GMarkerOptions object\r\n            markerOptions = { icon: blueIcon };\r\n" + AllLocationsText + "\r\n            \r\n        }\r\n\r\n       \r\n    </script>\r\n\r\n  </head>\r\n  <body onload=\"initialize()\" onunload=\"GUnload()\" scroll=\"no\">\r\n   <table class=\"directions\">\r\n    <tr>\r\n        <td colspan=\"2\">\r\n\t        <div id=\"getStatus\"></div>\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n    <td valign=\"top\"><div id=\"directions\" style=\"width: 1000px; display:none;\"></div></td>\r\n    <td valign=\"top\"><div id=\"map_canvas\" style=\"width: 1000px; height: 1000px;\"></div></td>\r\n\r\n    </tr>\r\n    </table> \r\n  </body>\r\n</html>\r\n");

        }

    }

}

}



