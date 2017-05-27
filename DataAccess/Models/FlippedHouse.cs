using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Models
{

    [NotMapped]
    public class FlippedHouse : Listing
    {
        //public string Address { get; set; }

        //public string City { get; set; }

        //public string PostalCode { get; set; }

        [NotMapped]
        public string Subdivision { get { return this.LegalSubdivisionName; } set { this.LegalSubdivisionName = value; } }
        [NotMapped]
        public string PurchasePrice { get; set; }
        public double PurchasePriceValue { get { return GetNumericValue(this.PurchasePrice); } }
        [NotMapped]
        public string PurchaseDate { get; set; }
        [NotMapped]
        public string SoldPrice { get; set; }
        public double SoldPriceValue { get { return GetNumericValue(this.SoldPrice); } }
        [NotMapped]
        public string SoldDate { get; set; }
        [NotMapped]
        public string Profit { get; set; }
        public double ProfitValue { get { return GetNumericValue(this.Profit); } }
        [NotMapped]
        public double RoiPurchase { get; set; }
        [NotMapped]
        public double RoiSold { get; set; }

        //public string SqFtHeated { get; set; }

        //public string LPSqFt { get; set; }

        //public string SPSqFt { get; set; }

        //public string Beds { get; set; }

        //public string FullBaths { get; set; }

       // public string HalfBaths { get; set; }

        //public string Pool { get; set; }

        //public string YearBuilt { get; set; }

        [NotMapped]
        public string PPSqFt { get; set; }
        public double PPSqFtValue { get { return GetNumericValue(this.PPSqFt); } }

        [NotMapped]
        public string ROI { get; set; }

        [NotMapped]
        public string SPoverPP { get; set; }

        [NotMapped]
        public string Date { get; set; }

        //public string ADOM { get; set; }
    }
}
