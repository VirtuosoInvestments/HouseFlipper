using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Models
{
    public class PropertyReference
    {
        public string /*Guid*/ PropertyId { get; set; }
        public Guid PropertyGuid { get; set; }
        public string /*Guid*/ FlipId { get; set; }
        public Guid FlipGuid { get; set; }

        public static PropertyReference Create(Listing listing)
        {
            return new PropertyReference() { PropertyId = listing.PropertyId() };
        }
    }
}
