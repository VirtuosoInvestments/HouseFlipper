using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Models
{
    public class Subdivisions : List<Subdivision>
    {
        public Subdivisions() { this.Lookup = new Dictionary<string, Subdivision>(); }
        public Subdivisions(IEnumerable<Subdivision> list)
            : this()
        {
            this.AddRange(list);
            foreach(var s in list)
            {
                this.Lookup.Add(s.Subdivision, s);
            }
        }
        public string Zipcode { get; set; }
        public Dictionary<string, Subdivision> Lookup { get; set; }

        public string SortBy { get; set; }

        public SortDirection SortDirection { get; set; } 
    }
    
    public class Subdivision : Homes
    {
        public Subdivision(IEnumerable<Listing> list, string zip)
            : base(list, zip)
        {
        }

        public string Name { get { return this.Subdivision; } }
    }
}
