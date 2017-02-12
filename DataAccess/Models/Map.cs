using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.DataAccess.Models
{
    public class Markers : List<Marker>
    {

    }

    public class Marker
    {
        
        public string title;
        public string lat;
        public string lng;
        public string description;
        public string icon;
   
    }
}
