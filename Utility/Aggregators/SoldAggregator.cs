using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility.Aggregators
{
    public class Aggregator2
    {
        private Func<MlsSet, MlsRow, bool> ShouldAdd;
        protected MlsSet set = new MlsSet();
        public Aggregator2(Func<MlsSet, MlsRow, bool> rule, MlsSet initialSet = null)
        {
            this.set = initialSet;
            this.ShouldAdd = rule;
        }

        public virtual bool Add(MlsRow record)
        {
            if (ShouldAdd(set, record))
            {
                set.Add(record);
                return true;
            }
            return false;
        }
    }
    public class SoldAggregator : Aggregator2
    {
        public SoldAggregator(MlsSet initialSet=null) : base((set, row) => row.IsSold(), initialSet)
        { }
    }    
}
