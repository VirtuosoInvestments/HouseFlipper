using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Models
{
    public class Property
    {
    }

    public class PropertyId
    {
        public PropertyId(Listing listing)
        {
            this.Value = listing.PropertyId();
        }

        public string Value { get; private set; }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var pId = obj as PropertyId;
            if(pId == null)
            {
                return false;
            }
            return pId.Value.Equals(this.Value);
        }
    }
}
