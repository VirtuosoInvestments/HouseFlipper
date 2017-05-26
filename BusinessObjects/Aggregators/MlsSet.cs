using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace Hack.HouseFlipper.BusinessObjects
{
    public class MlsSet
    {
        private Dictionary<string, List<Listing>> set = new Dictionary<string, List<Listing>>();

        public void Add(Listing record)
        {
            //var exists = false;
            string houseID = record.PropertyId();
            List<Listing> list;
            if (set.ContainsKey(houseID))
            {
                list = set[houseID];
            }
            else
            {                
                set.Add(houseID, list = new List<Listing>());
            }
            //list.Add(record);
            // sort chronologically by sold date
            var insertAt = -1;
            for(int i = 0; i<list.Count; i++)
            {
                var item = list[i];
                if(item.CloseDateValue() > record.CloseDateValue())
                {
                    insertAt = i;
                    break;
                }
            }
            if(insertAt<0)
            {
                list.Add(record);
            }
            else
            {
                list.Add(record);
            }
        }

        public bool ContainsKey(Listing record)
        {
            string houseID = record.PropertyId();
            return set.ContainsKey(houseID);
        }
    }
}