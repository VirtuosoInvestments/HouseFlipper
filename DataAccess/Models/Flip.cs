using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.DataAccess.Models
{
    public class Flip
    {
        public string PropertyId { get; set; }
        public Listing After { get; set; }
        public Listing Before { get; set; }
    }
}
