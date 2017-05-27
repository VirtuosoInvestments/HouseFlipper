using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.BusinessObjects
{
    public class ActiveAggregator : Aggregator
    {
        public ActiveAggregator() : base((set, row) => row.IsActive())
        {
            var zipTable = new ZipTable();
            this.AddEvent += zipTable.HandleAdd;
        }
    }
}
