using HouseFlipper.DataAccess.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Models
{
    public class HouseCharacteristics
    {
        public HouseCharacteristics() { this.Price = 0; this.Sum = 0; this.NumHouses = 0; this.Beds = 0; this.FullBaths = 0; this.HalfBaths = 0; this.SqFtHeated = 0; this.YearBuilt = 0; }

        [JsonIgnore]
        public double Sum { get; set; }
        public double NumHouses { get; set; }
        public double Price { get; set; }

        [JsonIgnore]
        public string PriceStr
        {
            get
            {
                return "$" + ToString(Price);
            }
        }

        private static string ToString(double num)
        {
            var wholeNumber = num.ToString();
            var separatorIndex = wholeNumber.IndexOf('.');
            var fraction = "";
            if (separatorIndex >= 0)
            {
                fraction = wholeNumber.Substring(separatorIndex);
                wholeNumber = wholeNumber.Substring(0, separatorIndex);
                if (fraction.Length == 2)
                {
                    fraction += "0";
                }
            }
            var k = 0;
            var beforeDecimal = "";
            for (int j = wholeNumber.Length - 1; j >= 0; j--)
            {
                if (k > 0 && k % 3 == 0)
                {
                    beforeDecimal = "," + beforeDecimal;
                }
                var digit = wholeNumber[j];
                beforeDecimal = digit + beforeDecimal;
                ++k;
            }
            return beforeDecimal + fraction;
        }

        public double Beds { get; set; }
        public double FullBaths { get; set; }
        public double HalfBaths { get; set; }
        public double SqFtHeated { get; set; }
        public double YearBuilt { get; set; }

        [JsonIgnore]
        public string PriceSqFt
        {
            get {
                var val = PriceSqFtValue(); 
                if(val<0)
                {
                    return "-$" + ToString(Math.Abs(val));
                }
                return "$" + ToString(val);
            }
        }

        [JsonIgnore]
        public string ProfitStr
        {
            get
            {
                var val = Profit;
                if (val < 0)
                {
                    return "-$" + ToString(Math.Abs(val));
                }
                return "$" + ToString(val);
            }
        }

        public double PriceSqFtValue()
        {
            var val = Math.Round(this.Price / this.SqFtHeated, 2);
            return val;
        }

        public double PPSqFt { get; set; }

        public double PurchasePrice { get; set; }

        public double PoolValue { get; set; }

        public string Pool
        {
            get
            {
                var index = (int)Math.Round(PoolValue);
                return ((PoolType)index).ToString();
            }
        }

        public double Profit { get; set; }

        public double ROIOnPurchasePrice { get; set; }

        public double ROIOnSoldPrice { get; set; }

        public double ADOM { get; set; }

        public double SPSqFT { get; set; }
    }
}
