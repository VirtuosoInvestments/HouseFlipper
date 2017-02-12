using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.DataAccess.Models
{
    public class GeoLocation
    {
        public string Address { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public GeoLocation() { }
        public GeoLocation(string address, double longitude, double latitude)
        {
            this.Address = address;
            this.Longitude = longitude;
            this.Latitude = latitude;
        }
    }
}
