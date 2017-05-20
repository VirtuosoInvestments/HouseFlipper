using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility
{
    public class SoldRule : IRule
    {
        private MlsSet soldSet;

        public SoldRule(MlsSet soldSet)
        {
            this.soldSet = soldSet;
        }

        public object Results
        {
            get
            {
                return soldSet;
            }
        }

        public bool IsSatisfied(params object[] variables)
        {
            Contract.Requires(variables != null);
            Contract.Requires(variables.Length > 0);
            var listing = variables[0] as Listing;
            var isSatisfied = listing.IsSold();
            if (isSatisfied)
            {
                soldSet.Add(listing);
            }
            return isSatisfied;
        }
    }
}
