using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.DataAccess.Models
{
    public class FlippedCharacteristics : IComparable
    {
        public FlippedCharacteristics(string zip, HouseCharacteristics preFlip, HouseCharacteristics postFlip)
        {
            Zipcode = zip;
            PreFlip = preFlip;
            PostFlip = postFlip;
        }
        public FlippedCharacteristics() { }

        public string City { get; set; }
        public string Zipcode { get; set; }
        public string SubDivision { get; set; }

        //public string Profit { get; set; }
        
        public HouseCharacteristics PreFlip { get; set; }
        public HouseCharacteristics PostFlip { get; set; }

        [JsonIgnore]
        public List<FlippedHouse> Houses
        {
            get;
            set;
        }
        [JsonIgnore]
        public string Profit
        {
            get
            {
                if(PreFlip==null || PostFlip==null)
                {
                    return null;
                }
                var val = ProfitValue();
                return ToDollars(val);
            }
        }

        public static string ToDollars(double val)
        {
            if (val >= 0)
            {
                return "$" + ToString(val);
            }
            return "-$" + ToString(Math.Abs(val));
        }

        public static string ToString(double num)
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

        public double ProfitValue()
        {
            var firstPrice = PreFlip.Price;
            var lastPrice = PostFlip.Price;
            var profit = lastPrice - firstPrice;
            return profit;
        }
        
        [JsonIgnore]
        public List<FlippedCharacteristics> Aggregation { get; set; }

        [JsonIgnore]
        public int MinYear { get; set; }

        [JsonIgnore]
        public int MaxYear { get; set; }

        [JsonIgnore]
        public double MinSqft { get; set; }

        [JsonIgnore]
        public double MaxSqft { get; set; }

        public int CompareTo(object obj)
        {
            var other = obj as FlippedCharacteristics;
            if (other == null) { return 1; }
            return this.ProfitValue().CompareTo(other.ProfitValue());
        }

        public double RoiPurchase { get; set; }

        public double RoiSold { get; set; }
    }
}
