using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.DB
{
    public class Property
    {
        public Property() { }      
        public Property(Listing record)
        {
            this.Address = record.Address;
            this.City = record.City;
            this.PostalCode = record.PostalCode;
            this.Status = record.Status;
        }

        public int Id { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public string Status { get; set; }


        public MlsStatus StatusValue()
        {
            return MlsStatusMapper.GetValue(this.Status);
        }

    }    
}
