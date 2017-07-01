using HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Models
{
    public class Flip
    {

        public Flip(Listing before, Listing after)
        {
            Contract.Requires(before.PropertyId().Equals(after.PropertyId()));
            this.Before = before;
            this.After = after;
            this.PropertyId = before.PropertyId();
        }

        public string PropertyId { get; set; }
        public Listing After { get; set; }
        public Listing Before { get; set; }
    }

    public class Flips : List<Flip>
    {
        public static Flips Convert(Listings listings)
        {
            listings.Sort((x, y) => x.CloseDate.CompareTo(y.CloseDate));
            var flips = new Flips();
            for (var i = 1; i < listings.Count; i++)
            {
                var before = listings[i - 1];
                var after = listings[i];
                flips.Add(new Flip(before, after));
            }
            return flips;
        }
    }
}
