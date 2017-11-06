using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using HouseFlipper.Utility.Objects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility
{
    public static class Globals
    {
        public static readonly MlsContext Database = new MlsContext();

        //key: mlsnumber
        public static ConcurrentDictionary<string, int> Listings = new ConcurrentDictionary<string, int>();

        //key: propertyid
        public static ConcurrentDictionary<string, List<Listing>> Sold = new ConcurrentDictionary<string, List<Listing>>();

        //public static ConcurrentDictionary<string, List<Flip>> Flips = new ConcurrentDictionary<string, List<Flip>>();
        public static ConcurrentDictionary<string, Dictionary<DateTime, Flip>> PropertyFlips = new ConcurrentDictionary<string, Dictionary<DateTime, Flip>>();

        //key: zip code
        //second key: listing.propertyname
        //value: before sum/count, after sum/count
        public static ConcurrentDictionary<string, Dictionary<string, Tuple<SumCount, SumCount>>> ZipFlips = new ConcurrentDictionary<string, Dictionary<string, Tuple<SumCount, SumCount>>>();

        public static ConcurrentDictionary<string, List<string>> SubdivFlips = new ConcurrentDictionary<string, List<string>>();

        public static ConcurrentDictionary<string, List<string>> CountyFlips = new ConcurrentDictionary<string, List<string>>();

        public static ConcurrentDictionary<string, List<string>> CommSubdivFlips = new ConcurrentDictionary<string, List<string>>();

        public static bool TryAdd<T>(this HashSet<T> hashSet, T value)
        {
            if(hashSet == null)
            {
                throw new ArgumentNullException("Error: hashSet cannot be null");
            }

            var exists = false;
            lock(hashSet)
            {
                exists = !hashSet.Add(value);                
            }
            return !exists;
        }
    }
}
