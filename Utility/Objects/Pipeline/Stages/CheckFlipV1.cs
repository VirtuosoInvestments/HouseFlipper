using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Pipeline.Stages
{
    public class CheckFlipV1 : Stage
    {
        private TimeSpan time;
        public CheckFlipV1(TimeSpan time)
        {
            this.time = time;
        }
        public override void Process(object data)
        {
            if(data is Listing)
            {
                var listing = data as Listing;
                var propertyId = listing.PropertyId().ToLower();
                if(!listing.IsSold())
                {
                    return;
                }

                if(Globals.Sold.ContainsKey(propertyId))
                {

                    Tuple<Listing, Listing> inBetween = null;
                    Globals.Sold.AddOrUpdate(
                        propertyId,
                        (x)=>new List<Listing>() { listing },
                        (k, currentList) =>
                        {
                            inBetween = BinarySearchAdd(currentList, listing);
                            return currentList;
                        });

                    var thisDate = listing.CloseDateValue();
                    List<Flip> flips = new List<Flip>();
                    if(inBetween!=null)
                    {
                        var before = inBetween.Item1;
                        var after = inBetween.Item2;

                        if(before!=null)
                        {
                            var span = thisDate - before.CloseDateValue();
                            if(span<=time)
                            {
                                var flip = new Flip(before, listing);
                                flips.Add(flip);
                            }
                        }

                        if (after != null)
                        {
                            var span = after.CloseDateValue() - thisDate;
                            if (span <= time)
                            {
                                var flip = new Flip(listing,after);
                                flips.Add(flip);
                            }
                        }

                        /*
                        Globals.Flips.AddOrUpdate(
                        propertyId,
                        new List<string>() { listing.MLNumber },
                        (k, currentList) =>
                        {
                            //TODO: BinarySearch update list - log n for each n elements -> nlogn
                            //      or O(1) insert at end for now, sort later
                            //BinarySearchAdd(currentList, listing);
                            return currentList;
                        });
                        */
                        
                        if(flips.Count>0)
                        {
                            this.Pass(flips);
                        }
                    }

                    
                }                
                else
                {
                    Globals.Sold.TryAdd(propertyId, new List<Listing>() { listing });
                }
            }
            else
            {
                throw new InvalidCastException("Error: data is expected to be Listing");
            }
        }

        private static Tuple<Listing,Listing> BinarySearchAdd(List<Listing> array, Listing listing)
        {
            var newVal = listing.CloseDateValue();
            int start = 0, end = array.Count - 1;
            if(newVal==null)
            {
                throw new InvalidOperationException("Error: Cannot add a listing with null CloseDateValue()");
            }
            var location = BinarySearchAdd(array, listing, newVal, start, end);
            Listing before = null;
            for (var i = location - 1; i >= start; i--)
            {
                var thisListing = array[start];
                var thisVal = thisListing.CloseDateValue();
                if (thisVal < newVal)
                {
                    before = thisListing;
                }
            }

            Listing after = null;
            for (var i = location + 1; i <= end; i++)
            {
                var thisListing = array[start];
                var thisVal = thisListing.CloseDateValue();
                if (newVal < thisVal )
                {
                    after = thisListing;
                }
            }
            return new Tuple<Listing, Listing>(before, after);
        }

        private static int BinarySearchAdd(List<Listing> array, Listing listing, DateTime? newVal, int start, int end)
        {
            while (start < end)
            {
                var mid = start + (end - start) / 2;
                var midVal = array[mid].CloseDateValue();
                if (midVal == newVal)
                {
                    array.Insert(mid, listing);                    
                    return mid;
                }
                else if (newVal < midVal)
                {
                    end = mid;
                    return BinarySearchAdd(array, listing, newVal, start, end);
                }
                else
                {
                    start = mid;
                    return BinarySearchAdd(array, listing, newVal, start, end);
                }
            }

            // case end moves all the way to start
            if(start==end)
            {
                if (newVal > array[end].CloseDateValue())
                {
                    if (end == (array.Count - 1))
                    {
                        array.Add(listing);
                        return end + 1;
                    }
                    else
                    {
                        array.Insert(end + 1, listing);
                        return end + 1;
                    }
                }
                else
                {
                    if (start == 0)
                    {
                        array.Insert(start, listing);
                        return start;
                    }
                    else
                    {
                        array.Insert(start - 1, listing);
                        return start - 1;
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Error: Unhandled case!");
            }
        }
    }
}
