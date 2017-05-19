using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility
{    
    public class SoldAggregator : Aggregator
    {
        public SoldAggregator(MlsSet initialSet=null) : base((set, row) => row.IsSold(), initialSet)
        { }
    }    
}
