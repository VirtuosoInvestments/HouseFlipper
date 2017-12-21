using HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Index
{
    public class SoldIndex
    {
        private Dictionary<string, List<Listing>> index = 
            new Dictionary<string, List<Listing>>();

        private ISortedListAddStrategy addStrategy = new BinarySearchAddStrategy();
        
        public Tuple<Listing, Listing> AddOrUpdate(string propertyId, Listing listing)
        {
            Tuple<Listing, Listing> inBetween = null;
            lock (this)
            {
                if (index.ContainsKey(propertyId))
                {
                    var currentList = index[propertyId];
                    inBetween = addStrategy.Insert(listing, currentList);
                }
                else
                {
                    index.Add(propertyId, new List<Listing>() { listing });
                }
            }
            
            return inBetween;
        }        
    }
}
