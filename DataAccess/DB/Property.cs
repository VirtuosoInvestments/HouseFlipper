using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.DB
{
    public class Property
    {
        public int Id { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public string Status { get; set; }


        ////[NotMapped]
        public MlsStatus StatusValue()
        {

            var str = this.Status.ToLower();
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
