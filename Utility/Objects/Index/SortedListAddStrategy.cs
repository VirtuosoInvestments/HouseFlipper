using HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Index
{
    public interface ISortedListAddStrategy
    {
        Tuple<Listing, Listing> Insert(Listing listing, List<Listing> array);
    }

    public class BinarySearchAddStrategy : ISortedListAddStrategy
    {
        public Tuple<Listing, Listing> Insert(Listing listing, List<Listing> array)
        {
            var newVal = listing.CloseDateValue();
            int start = 0, end = array.Count - 1;
            if (newVal == null)
            {
                throw new InvalidOperationException("Error: Cannot add a listing with null CloseDateValue()");
            }
            var newValLocation = HandleAdd(listing, array, newVal, start, end);
            return FindInBetween(newVal, newValLocation, array, start, end);
        }

        

        private int HandleAdd(Listing listing, List<Listing> array, DateTime? newVal, int start, int end)
        {
            if(start<0 || end<0)
            {
                throw new InvalidOperationException("Error: Invalid index(es) => Start=" + start + " End=" + end);
            }
            while (start <= end)
            {
                var mid = start + (end - start) / 2;
                var midVal = array[mid].CloseDateValue();
                if (midVal == newVal)
                {
                    array.Insert(mid, listing); //insert before midVal (move midVal over 1)
                    return mid;
                }
                else if (newVal < midVal)
                {
                    end = mid; // move end to middle
                    return HandleAdd(listing, array, newVal, start, end);
                }
                else
                {
                    start = mid; // move start to middle
                    return HandleAdd(listing, array, newVal, start, end);
                }
            }

            return HandleStartEqualsEnd(listing, array, newVal, start, end);
        }

        private static int HandleStartEqualsEnd(Listing listing, List<Listing> array, DateTime? newVal, int start, int end)
        {
            // case start and end index meet at the same spot
            //      & value at that same spot != newVal
            //        which means newVal is either < or > sameSpotVal
            if (start == end)
            {
                if (newVal > array[end].CloseDateValue()) // newVal greater than endVal
                {
                    if (end == (array.Count - 1)) //end is last item
                    {
                        array.Add(listing); //add as last item to list
                        return end + 1;
                    }
                    else  //end is not last item, but newVal is > than endVal
                    {
                        var next = end + 1;
                        for (; next<array.Count; next++) // account for duplicate endvals together in list, so walk forwards (Because newVal is greater than endVal)
                        {
                            var thisListing = array[next];
                            var nextVal = thisListing.CloseDateValue();
                            if (newVal < nextVal) //stop
                            {
                                array.Insert(next, listing); // insert before nextVal (Because it's greater than newVal)
                                return next;
                            }
                        }

                        if (next == (array.Count - 1)) //end is last item
                        {
                            array.Add(listing); //add as last item to list
                            return end + 1;
                        }
                        throw new InvalidOperationException("Unhandled condition!");
                    }
                }
                else  // newVal is less than start
                {                    
                    if (start == 0) // start is at beginning
                    {
                        array.Insert(start, listing); // insert before startVal (move startVal over 1)
                        return start;
                    }
                    else
                    {
                        var prev = start - 1;
                        for (; prev >= 0; prev--) // account for duplicate startVals together in list, so walk backwards (Because newVal is less than start)
                        {
                            var thisListing = array[prev];
                            var previousVal = thisListing.CloseDateValue();
                            if (newVal > previousVal) //stop
                            {
                                array.Insert(prev + 1, listing); // insert after the previousVal (Because newVal is greater than previousVal)
                                return prev + 1;
                            }
                        }

                        if (prev == 0) //prev is at beginning
                        {
                            array.Insert(0, listing); // insert before previousVal (Because it's less or equal to newVal)
                            return 0;
                        }
                        throw new InvalidOperationException("Unhandled condition!");
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Error: Unhandled case!");
            }
        }

        private static Tuple<Listing, Listing> FindInBetween(
            DateTime? newVal,
            int newValLocation,
            List<Listing> array,
            int start,
            int end)
        {
            Listing before = null;
            for (var previous = newValLocation - 1; previous >= start; previous--) //find prevousValue that is NOT equal to newVal, but is LESS than newVal (account for duplicates)
            {
                var thisListing = array[previous];
                var previousVal = thisListing.CloseDateValue();
                if (previousVal < newVal)
                {
                    before = thisListing;
                }
            }

            Listing after = null;
            for (var next = newValLocation + 1; next <= end; next++) // find nextValue that is NOT equal to newVal, but is GREATER than newVal (account for duplicates) 
            {
                var thisListing = array[next];
                var nextValue = thisListing.CloseDateValue();
                if (newVal < nextValue)
                {
                    after = thisListing;
                }
            }
            return new Tuple<Listing, Listing>(before, after);
        }        
    }
}
