using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using HouseFlipper.Utility.Objects;
using HouseFlipper.Utility.Objects.Statistics;
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
        public static ConcurrentDictionary<string, Dictionary<string, Tuple<SumCount, SumCount>>> ZipFlipTotals = new ConcurrentDictionary<string, Dictionary<string, Tuple<SumCount, SumCount>>>();

        public static ConcurrentDictionary<string, Dictionary<string, Tuple<SumCount, SumCount>>> SubdivFlipTotals = new ConcurrentDictionary<string, Dictionary<string, Tuple<SumCount, SumCount>>>();

        public static ConcurrentDictionary<string, Dictionary<string, Tuple<SumCount, SumCount>>> CountyFlipTotals = new ConcurrentDictionary<string, Dictionary<string, Tuple<SumCount, SumCount>>>();

        //key: zip
        //value: property id
        public static ConcurrentDictionary<string, HashSet<string>> ZipFlips = new ConcurrentDictionary<string, HashSet<string>>();
        public static ConcurrentDictionary<string, HashSet<string>> SubdivFlips = new ConcurrentDictionary<string, HashSet<string>>();
        public static ConcurrentDictionary<string, HashSet<string>> CountyFlips = new ConcurrentDictionary<string, HashSet<string>>();
        public static ConcurrentDictionary<string, HashSet<string>> CommSubdivFlips = new ConcurrentDictionary<string, HashSet<string>>();

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

        public static List<string> FlipProperties = new List<string>()
                {
                    //property names of listing we want to keep track
                    //of averages of
                    "CurrentPrice",
                    "Beds",
                    "FullBaths",
                    "HalfBaths",
                    "SqFtHeated",
                    "YearBuilt",
                    "Pool",
                    "Taxes",
                    "CDOM",
                    "ADOM",
                    "LPSqFt",
                    "SPSqFt",
                    "SPLP"
                };
    }
}
