using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Pipeline.Stages
{
    public class CheckFlip : Stage
    {
        public override void Process(object data)
        {
            if(data is Listing)
            {
                var listing = data as Listing;
                var id = listing.PropertyId().ToLower();
                if(!listing.IsSold())
                {
                    return;
                }
                if (!Globals.Sold.TryAdd(id))
                {
                    Globals.Flips.AddOrUpdate(
                        id,
                        new List<string>() { listing.MLNumber },
                        (k, currentList) => {
                            //TODO: BinarySearch update list - log n for each n elements -> nlogn
                            //      or O(1) insert at end for now, sort later
                            currentList.Add(id);
                            return currentList;
                        });

                    this.Pass(listing);
                }
                else
                {
                    throw new InvalidCastException("Error: data is expected to be Listing");
                }
            }
        }
    }
}
