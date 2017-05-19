using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.DataAccess.Models
{
    public class PropertyReference
    {
        public Guid PropertyId { get; set; }
        public Guid FlipId { get; set; }
    }
}
