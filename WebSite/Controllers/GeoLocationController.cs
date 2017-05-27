using Google.Maps.Geocoding;
using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebSite.Controllers
{
    public class GeoLocationController : ApiController
    {
        //public string Get(int id)
        //{
        //    return "geo";
        //}

        /* GET: api/geolocation/<address>
         * Examples:
         * 210 ADAMS DR, MAITLAND, FL  32751
           719 ALAMEDA ST, ORLANDO, FL  32804
           1302 42ND ST, Orlando, FL 32839

           http://localhost:32734/api/geolocation/1302 42ND ST, Orlando, FL 32839
           http://localhost:32734/api/geolocation/210 ADAMS DR, MAITLAND, FL  32751
           http://localhost:32734/api/geolocation/719 ALAMEDA ST, ORLANDO, FL  32804
         */
        public GeoLocation GetLatLong(string id)
        {
            GeocodingRequest request = new GeocodingRequest();
            request.Address = id;
            request.Sensor = false;
            GeocodingService svc = new GeocodingService();           
            var response = svc.GetResponse(request);
            if (response != null)
            {
                if (response.Results != null && response.Results.Length > 0)
                {
                    var result = response.Results.First();

                    var longitude = result.Geometry.Location.Longitude;
                    var latitude = result.Geometry.Location.Latitude;
                    var addr = result.FormattedAddress;
                    Console.WriteLine("Full Address: " + addr);         // "1600 Amphitheatre Pkwy, Mountain View, CA 94043, USA"
                    Console.WriteLine("Latitude: " + latitude);   // 37.4230180
                    Console.WriteLine("Longitude: " + longitude); // -122.0818530
                    var loc = new GeoLocation(addr, longitude, latitude);
                    return loc;
                }
            }
            return null;
        }
    }
}
