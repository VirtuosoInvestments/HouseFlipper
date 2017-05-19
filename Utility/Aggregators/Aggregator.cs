using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility
{
    public class Aggregator
    {
        private Func<MlsSet, Listing, bool> ShouldAdd;
        protected MlsSet set = new MlsSet();
        public Aggregator(Func<MlsSet, Listing, bool> rule, MlsSet initialSet = null)
        {
            this.set = initialSet;
            this.ShouldAdd = rule;
        }

        public event Action<Listing> AddEvent;

        public virtual bool Add(Listing record)
        {
            if (ShouldAdd(set, record))
            {
                set.Add(record);
                AddEvent?.BeginInvoke(record, null, null);
                return true;
            }
            return false;
        }
    }
}
