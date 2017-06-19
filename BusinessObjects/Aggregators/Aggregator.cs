using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.BusinessObjects
{
    public class Aggregator
    {
        
        private IDataSet dataSet;        

        public Aggregator(IDataSet dataSet)
        {
            this.dataSet = dataSet;
        }

        public void Execute(Action<object> callback)
        {
            if (dataSet.Peek())
            {
                callback(dataSet.Next());
            }
        }

        public void Execute(Func<object, bool> where, Action<object> collect)
        {
            dataSet.Seek(0);
            while (dataSet.Peek())
            {
                object item;
                if(where(item=dataSet.Next()))
                {
                    collect(item);
                }
            }
        }
    }

    public class OldAggregator
    {
        private Func<PropertyListingsMap, Listing, bool> ShouldAdd;
        protected PropertyListingsMap set = new PropertyListingsMap();
        private IRule rule;

        public OldAggregator()
        {
            this.DataSet = new Dictionary<string, List<Flip>>();
        }

        public OldAggregator(Func<PropertyListingsMap, Listing, bool> rule, PropertyListingsMap initialSet = null):this()
        {           
            this.set = initialSet;
            this.ShouldAdd = rule;
        }

        public OldAggregator(IRule rule):this()
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
