using HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Pipeline.Stages
{
    public class CreateListing : Stage
    {
        public override void Process(object data)
        {
            if (data is StringDictionary)
            {
                var args = data as StringDictionary;
                this.Pass(NewListing(args));
            }
            else
            {
                throw new InvalidCastException("Error: data is expected to be object[]");
            }
        }

        private Listing NewListing(StringDictionary data)
        {
            var record = new Listing(data);
            return record;
        }
    }
}
