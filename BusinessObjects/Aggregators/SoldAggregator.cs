using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.BusinessObjects
{    
    public class SoldAggregator : Aggregator
    {
        public SoldAggregator(MlsSet initialSet=null) : base((set, row) => row.IsSold(), initialSet)
        { }
    }    
}
