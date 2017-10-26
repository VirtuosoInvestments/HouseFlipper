using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility
{
    public class Globals
    {
        public static ConcurrentDictionary<string, int> Listings = new ConcurrentDictionary<string, int>();
    }
}
