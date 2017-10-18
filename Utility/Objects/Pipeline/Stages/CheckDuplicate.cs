using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Pipeline.Stages
{
    public class CheckDuplicate : Stage
    {
        public override void Process(object data)
        {
            if (data is StringDictionary)
            {
                var dict = data as StringDictionary;
                var id = dict["MLSNumber"];

                //Check some global first if there is a collision
                //   then Check against db context for a collision
                //        otherwise process and pass on
                if(Globals.Listings.TryAdd(id,1))
                {
                    this.Pass(dict);
                }
            }
            else
            {
                throw new InvalidCastException("Error: data is expected to be StringDictionary");
            }
        }
    }
}
