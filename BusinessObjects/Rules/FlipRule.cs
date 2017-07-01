using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.BusinessObjects
{
    public class FlipRule : IRule
    {
        private PropertyListingsMap soldSet;       

        public FlipRule(PropertyListingsMap soldSet)
        {
            this.soldSet = soldSet;
            this.DictionaryResults = new Dictionary<PropertyId, Listings>();
        }

        public object Results
        {
            get
            {
                return this.FlipResults;
            }
            
        }

        private Dictionary<PropertyId, Listings> DictionaryResults
        {
            get;
            set;
        }
        
        private Dictionary<PropertyId, Flips> FlipResults
        {
            get
            {
                var results = new Dictionary<PropertyId, Flips>();
                foreach(var propId in this.DictionaryResults.Keys)
                {
                    var listings = this.DictionaryResults[propId];
                    results.Add(propId,Flips.Convert(listings));
                }
                return results;
            }
        }
        
        public bool IsSatisfied(params object[] variables)
        {
            Contract.Requires(variables != null);
            Contract.Requires(variables.Length > 0);
            var listing = variables[0] as Listing;
            Contract.Requires(listing!=null);
            var isSatisfied = listing.IsSold() && soldSet.ContainsKey(listing);
            if(isSatisfied)
            {
                var propRef = new PropertyId(listing);
                var results = this.DictionaryResults;
                Listings list = null;
                if (results.ContainsKey(propRef))
                {
                    list = results[propRef];
                }
                else
                {
                    list = new Listings();
                    results.Add(propRef, list);
                }
                list.Add(listing);                
                soldSet.Add(listing);
            }
            return isSatisfied;
        }
    }
}
