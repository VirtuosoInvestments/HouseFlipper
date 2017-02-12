using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.DataAccess.Models
{
    public class Zips : List<Homes>
    {
        public Zips() { this.Flips = new Dictionary<string, Homes>(); }
        public Zips(IEnumerable<Homes> list)
            : this()
        {
            this.AddRange(list);
        }
        public Dictionary<string, Homes> Flips { get; set; }

        public string SortBy { get; set; }

        public SortDirection SortDirection { get; set; }        
    }
}
