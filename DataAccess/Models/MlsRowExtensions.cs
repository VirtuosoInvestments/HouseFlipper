using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.DataAccess.Models
{
    public static class MlsRowExtensions
    {
        public static bool IsSold(this MlsRow record)
        {
            return record.StatusValue() == MlsStatus.Sold;
        }

        public static bool IsActive(this MlsRow record)
        {
            return record.StatusValue() == MlsStatus.Active;
        }

        public static string PropertyId(this MlsRow record)
        {
            string fullAddress = record.FullAddress();
            var houseID = fullAddress.ToLower();
            return houseID;
        }

        public static string FullAddress(this MlsRow record)
        {
            return record.Address.Trim() + record.City.Trim() + record.PostalCode.Trim();
        }
    }
}
