using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Models
{
    public enum SortDirection { None = -1, Ascending = 0, Descending = 1 }
    public enum PoolType
    {
        None = 0,
        Private = 1,
        Community = 2
    }
}
