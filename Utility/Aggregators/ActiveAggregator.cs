﻿using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility
{
    public class ActiveAggregator : Aggregator2
    {
        public ActiveAggregator() : base((set, row) => row.IsActive())
        {
            var zipTable = new ZipTable();
            this.AddEvent += zipTable.HandleAdd;
        }
    }
}
