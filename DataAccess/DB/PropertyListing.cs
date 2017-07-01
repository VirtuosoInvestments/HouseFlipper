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
        public int ID { get; set; }
        public int PropertyId { get; set; }
        public string ListingId { get; set; }
    }
}
