using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace Hack.HouseFlipper.Utility
{
    public class MlsSet
    {
        private Dictionary<string, List<MlsRow>> set;

        public void Add(MlsRow record)
        {
            //var exists = false;
            string houseID = record.PropertyId();
            List<MlsRow> list;
            if (set.ContainsKey(houseID))
            {
                set.Add(houseID, list=new List<MlsRow>());
            }
            else
            {
                list = set[houseID];
            }
            list.Add(record);
        }

        public bool ContainsKey(MlsRow record)
        {
            string houseID = record.PropertyId();
            return set.ContainsKey(houseID);
        }
    }
}