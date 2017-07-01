using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.DB
{
    public enum MlsStatus
    {
        Unknown,
        Active,
        Sold
    }

    public enum PoolType
    {
        None = 0,
        Private = 1,
        Community = 2
    }

    public static class MlsStatusMapper
    {
        public static MlsStatus GetValue(string status)
        {
            var str = status;
            if (str == "sld")
            {
                return MlsStatus.Sold;
            }
            else if (str == "act")
            {
                return MlsStatus.Active;
            }
            else
            {
                throw new InvalidOperationException();
            }

        }
    }
}
