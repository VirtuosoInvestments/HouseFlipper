using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.BusinessObjects
{
    public class Aggregator
    {
        private Func<MlsSet, Listing, bool> ShouldAdd;
        protected MlsSet set = new MlsSet();
        private IRule rule;

        public Aggregator()
        {
            this.DataSet = new Dictionary<string, List<Flip>>();
        }

        public Aggregator(Func<MlsSet, Listing, bool> rule, MlsSet initialSet = null):this()
        {           
            this.set = initialSet;
            this.ShouldAdd = rule;
        }

        public Aggregator(IRule rule):this()
        {
            this.rule = rule;
        }

        public Dictionary<string, List<Flip>> DataSet { get; set; }

        public event Action<Listing> AddEvent;

        public virtual bool Add(Listing record)
        {
            if (ShouldAdd != null)
            {
                return OldWay(record);
            }
            return NewWay(record);
        }

        private bool NewWay(Listing record)
        {
            if(rule!=null)
            {
                rule.IsSatisfied(record);
                AddEvent?.BeginInvoke(record, null, null);
                return true;
            }
            return false;
        }

        private bool OldWay(Listing record)
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
