using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.DB
{
    [Table("PropertyListings")]
    public class PropertyListing
    {
        public PropertyListing() { }
        public PropertyListing(Listing record, Property property)
        {
            this.PropertyId = property.Id;
            this.ListingId = record.Id;
        }

        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int ListingId { get; set; }
    }
}
