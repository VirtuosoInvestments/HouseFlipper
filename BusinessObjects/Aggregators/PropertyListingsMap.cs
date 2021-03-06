﻿using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace HouseFlipper.BusinessObjects
{
    public class PropertyListingsMap : Dictionary<string, List<Listing>>
    {        
        public void Add(Listing record)
        {
            //var exists = false;
            string houseID = record.PropertyId();
            List<Listing> list;
            if (this.ContainsKey(houseID))
            {
                list = this[houseID];
            }
            else
            {                
                this.Add(houseID, list = new List<Listing>());
            }
            //list.Add(record);
            // sort chronologically by sold date
            var recordDate = record.CloseDateValue();
            var insertAt = -1;
            for(int i = 0; i<list.Count; i++)
            {
                var item = list[i];
                var thisDate = item.CloseDateValue();
                if (thisDate > recordDate)
                {
                    insertAt = i;
                    break;
                }
                else if(thisDate==recordDate)
                {
                    if(record.MLNumber==item.MLNumber)
                    {
                        return;
                    }
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
            return this.ContainsKey(houseID);
        }
    }
}