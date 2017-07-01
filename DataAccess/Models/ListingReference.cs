using HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Models
{
    public class ListingReference
    {
        public ListingReference(Listing listing)
        {
            this.Id = listing.MLNumber;
        }

        public string Id { get; private set; }
    }

    public class ListingReferences : List<ListingReference>
    {

    }
}
