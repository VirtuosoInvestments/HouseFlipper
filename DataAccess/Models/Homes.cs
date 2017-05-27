using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Models
{
    public class Homes : List<Listing>//List<FlippedHouse>
    {
        public Homes() 
        {
            //this.Subdivisions = new Dictionary<string, Homes>(StringComparer.OrdinalIgnoreCase);
            this.Subdivisions = new Subdivisions();
        }
        public Homes(IEnumerable</*FlippedHouse*/Listing> list, string zip) : this()
        {
            this.AddRange(list);
            this.Zipcode = zip;
        }
        public string Zipcode { get; set; }
        public string Subdivision { get; set; }

        public string SortBy { get; set; }

        public SortDirection SortDirection { get; set; }

        public HouseCharacteristics AverageSold { get; set; }

        public HouseCharacteristics AverageBought { get; set; }

        public string AverageBeds { get; set; }
        public double AverageBedsValue { get { return AverageBought.Beds; } }

        public string AverageFullBaths { get; set; }
        public double AverageFullBathsValue { get { return AverageBought.FullBaths; } }

        public string AverageHalfBaths { get; set; }
        public double AverageHalfBathsValue { get { return AverageBought.HalfBaths; } }

        public double AverageYearBuilt { get; set; }

        public string AveragePurchase { get; set; }

        public double AveragePurchaseValue { get { return AverageBought.Price; } }

        public string AverageSoldPrice { get; set; }

        public double AverageSoldPriceValue { get { return AverageSold.Price; } }

        public string AverageSqFtHeated { get; set; }
        public double AverageSqFtHeatedValue { get { return AverageBought.SqFtHeated; } }

        public string AverageProfit { get; set; }
        public double AverageProfitValue { get { return AverageSold.Profit; } }

        public string AverageADOM { get; set; }
        public double AverageADOMValue { get { return AverageSold.ADOM; } }

        public string AveragePPSqFt { get; set; }
        public double AveragePPSqFtValue { get { return AverageBought.SPSqFT; } }

        public string AverageSPSqFT { get; set; }
        public double AverageSPSqFTValue { get { return AverageSold.SPSqFT; } }

        public string AverageROI { get; set; }
        public double AverageROIValue { get { return AverageSold.ROIOnSoldPrice; } }

        

        public Homes Active { get; set; }

        public int ActiveCount { get { if (Active != null) { return Active.Count; } return 0; } }

        public Subdivisions Subdivisions { get; set; }
        public int SubdivisionCount { get { if (Subdivisions != null) { return Subdivisions.Count; } return 0; } }
    }
}
