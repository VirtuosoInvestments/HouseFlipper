using HouseFlipper.DataAccess.DB;
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
        public static ConcurrentDictionary<string, int> Listings = new ConcurrentDictionary<string, int>();

        public static HashSet<string> Sold = new HashSet<string>();

        public static ConcurrentDictionary<string, List<string>> Flips = new ConcurrentDictionary<string, List<string>>();

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
