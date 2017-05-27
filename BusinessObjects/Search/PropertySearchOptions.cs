using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.BusinessObjects
{
    public class PropertySearchOptions
    {
        public bool Active { get; set; }        
        public bool Sold { get; set; }
        public double WithinRadius { get; set; }
    }
}
