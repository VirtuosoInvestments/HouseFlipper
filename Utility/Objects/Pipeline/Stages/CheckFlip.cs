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
        private TimeSpan withinTimeSpan;
        public CheckFlip(TimeSpan time)
        {
            this.withinTimeSpan = time;
        }
        public override void Process(object data)
        {
            if (data is Listing)
            {
                var listing = data as Listing;
                var propertyId = listing.PropertyId().ToLower();
                if (!listing.IsSold())
                {
                    return;
                }

                var inBetween = Globals.Sold.AddOrUpdate(propertyId, listing);

                if (inBetween != null)
                {
                    var thisDate = listing.CloseDateValue();
                    List<Flip> flips = new List<Flip>();

                    var before = inBetween.Item1;
                    var after = inBetween.Item2;

                    if (before != null)
                    {
                        var span = thisDate - before.CloseDateValue();
                        if (span <= withinTimeSpan)
                        {
                            var flip = new Flip(before, listing);
                            flips.Add(flip);
                        }
                    }

                    if (after != null)
                    {
                        var span = after.CloseDateValue() - thisDate;
                        if (span <= withinTimeSpan)
                        {
                            var flip = new Flip(listing, after);
                            flips.Add(flip);
                        }
                    }

                    if (flips.Count > 0)
                    {
                        this.Pass(flips);
                    }
                }
            }
            else
            {
                throw new InvalidCastException("Error: data is expected to be Listing");
            }
        }
    }
}
