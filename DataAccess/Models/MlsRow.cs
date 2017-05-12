using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Hack.HouseFlipper.DataAccess.Models
{
    public enum MlsStatus { Active, Sold }

    [Table("Listings")]
    public class MlsRow : IComparable
    {
        public MlsRow(StringDictionary data)
        {
            this.Lookup = data;
            var hash = new Dictionary<string, PropertyInfo>();
            foreach (var p in typeof(MlsRow).GetProperties())
            {
                var thisName = p.Name.ToLower();
                hash.Add(thisName, p);
            }

            foreach (string k in data.Keys)
            {
                var name = k.ToLower();
                if (name == "#") { continue; }
                var propName = name.Replace(" ", string.Empty).Replace("/", string.Empty);

                if (hash.ContainsKey(propName))
                {
                    var p = hash[propName];
                    p.SetMethod.Invoke(this, new object[] { data[k] });
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            /*
            foreach (var p in typeof(MlsRow).GetProperties())
            {
                var name = p.Name.ToLower();
                if (data.ContainsKey(name))
                {
                    var val = data[name];
                    p.SetMethod.Invoke(this, new object[] { val });
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }*/
        }

        public MlsRow() { }

        [JsonIgnore]
        [NotMapped]
        public StringDictionary Lookup { get; private set; }

        public double GetNumericValue(string val, int? decimalsToRound=null)
        {
            if (string.IsNullOrWhiteSpace(val)) { return double.MinValue; }
            val = val.Replace(",", string.Empty).Replace("$", string.Empty);
            if (decimalsToRound.HasValue)
            {
                return Math.Round(double.Parse(val), decimalsToRound.Value);
            }
            return double.Parse(val);
        }

        public int ID { get; set; }

        public string MLNumber { get; set; }
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
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string LegalSubdivisionName { get; set; }
        public string SqFtHeated { get; set; }

        //[NotMapped]
        public double SqFtHeatedValue()
        {
            /*
                var val = this.SqFtHeated.Replace(",", string.Empty);
                return double.Parse(val);*/
            return GetNumericValue(this.SqFtHeated, 0);


        }

        public string CurrentPrice { get; set; }

        //[NotMapped]
        public double CurrentPriceValue()
        {

            return GetNumericValue(this.CurrentPrice, 2);

        }
        public string Beds { get; set; }

        //[NotMapped]
        public double BedsValue()
        {

            return GetNumericValue(this.Beds, 0);

        }
        public string FullBaths { get; set; }

        //[NotMapped]
        public double FullBathsValue()
        {

            return GetNumericValue(this.FullBaths, 0);

        }
        public string HalfBaths { get; set; }

        //[NotMapped]
        public double HalfBathsValue()
        {

            return GetNumericValue(this.HalfBaths, 0);

        }

        public string YearBuilt { get; set; }

        //[NotMapped]
        public double YearBuiltValue()
        {

            return GetNumericValue(this.YearBuilt, 0);

        }
        public string Pool { get; set; }
        public string PropertyStyle { get; set; }
        public string Taxes { get; set; }
        public double TaxesValue()
        {
            return GetNumericValue(this.Taxes, 0);
        }
        public string CDOM { get; set; }
        public string ADOM { get; set; }
        public string DaystoContract { get; set; }
        public string SoldTerms { get; set; }
        public string CloseDate { get; set; }
        public string LPSqFt { get; set; }
        public string SPSqFt { get; set; }
        public string SPLP { get; set; }
        public string ListOfficeName { get; set; }
        public string ListAgentFullName { get; set; }
        public string ListAgentID { get; set; }
        public string SellingAgentName { get; set; }
        public string SellingOfficeID { get; set; }
        public string SellingAgentID { get; set; }
        public string LSCListSide { get; set; }
        public string OfficePrimaryBoardID { get; set; }
        //[NotMapped]
        public double? Latitude { get; set; }
        //[NotMapped]
        public double? Longitude { get; set; }

        //[NotMapped]
        public double PostalCodeValue() { return GetNumericValue(this.PostalCode, null); }
        /* public double SqFtHeatedValue { return GetNumericValue(this.SqFtHeated); } }
         public double CurrentPriceValue { return GetNumericValue(this.CurrentPrice); } }
         public double FullBathsValue { return GetNumericValue(this.FullBaths); } }
         public double HalfBathsValue { return GetNumericValue(this.HalfBaths); } }
         public double YearBuiltValue { return GetNumericValue(this.YearBuilt); } }*/

        //[NotMapped]
        public double CDOMValue() { return GetNumericValue(this.CDOM, null); }

        //[NotMapped]
        public double ADOMValue() { return GetNumericValue(this.ADOM, null); }
        public double ADOMNumValue { get { return ADOMValue(); } }

        //[NotMapped]
        public double DaystoContractValue() { return GetNumericValue(this.DaystoContract, 0); }

        //[NotMapped]
        public double LPSqFtValue() { return GetNumericValue(this.LPSqFt, null); }
        public double LPSqFtNumValue { get { return LPSqFtValue(); } }

        //[NotMapped]
        public double SPSqFtValue() { return GetNumericValue(this.SPSqFt, null); }
        public double SPSqFtNumValue { get { return SPSqFtValue(); } }

        //[NotMapped]
        public double SPLPValue() { return GetNumericValue(this.SPLP, null); }

        //[NotMapped]
        public double ListAgentIDValue() { return GetNumericValue(this.ListAgentID, null); }

        //[NotMapped]
        public double SellingAgentIDValue() { return GetNumericValue(this.SellingAgentID, null); }

        //[NotMapped]
        public DateTime? CloseDateValue()
        {
            if (string.IsNullOrWhiteSpace(this.CloseDate)) { return null; }
            return DateTime.Parse(this.CloseDate);
        }

        public int CompareTo(object obj)
        {
            var other = obj as MlsRow;
            if (other == null)
            {
                throw new ArgumentException(
                    string.Format("CompareTo cannot compare {0} against {1}", this.GetType().Name, obj == null ? "null" : obj.GetType().Name)
                    );
            }
            var date1 = this.CloseDateValue();
            if (!date1.HasValue)
            {
                return -1;
            }
            var date2 = other.CloseDateValue();
            if (!date2.HasValue)
            {
                return 1;
            }
            return date1.Value.CompareTo(date2.Value);
        }

        [NotMapped]
        public double? PreFlipAvgPrice { get; set; }

        [NotMapped]
        public double? WithinPreFlipPercent { get; set; }

        [NotMapped]
        public string PostFlipAvgSPSqFt { get; set; }
        [NotMapped]
        public double? AfterRepairValue {get;set;}
        public void SetAfterRepairValue(double val)
        {
            this.AfterRepairValue = val;
        }

        [NotMapped]
        public string AfterRepair
        {
            get
            {
                if (!AfterRepairValue.HasValue) { return String.Empty; }
                return ToDollars(AfterRepairValue.Value);
            }
        }

        [NotMapped]
        public string PotentialProfit
        {
            get
            {
                var profit = PotentialProfitValue;
                if (!profit.HasValue) { return null; }
                var val = profit.Value;
                return ToDollars(val);
            }
        }

        [NotMapped]
        public string PotentialROI
        {
            get
            {
                var profit = PotentialProfitValue;
                if (!profit.HasValue) { return null; }
                var roiPurchase = PotentialROIPurchaseValue;
                var roiSold = PotentialROISoldValue;
                return Math.Round(roiPurchase * 100) + "%/" + Math.Round(roiSold * 100) + "%";
            }
        }

        public double PotentialROISoldValue
        {
            get
            {
                var profit = PotentialProfitValue;
                var roiSold = profit.Value / this.AfterRepairValue.Value;
                return roiSold;
            }
        }

        public double PotentialROIPurchaseValue
        {
            get
            {
                var profit = PotentialProfitValue;
                var roiPurchase = profit.Value / this.CurrentPriceValue();
                return roiPurchase;
            }
        }

        private static string ToDollars(double val)
        {
            val = Math.Round(val, 2);
            if (val < 0)
            {
                return "-$" + ToString(Math.Abs(val));
            }
            return "$" + ToString(val);
        }

        [NotMapped]
        public double? PotentialProfitValue
        {
            get
            {
                if (!AfterRepairValue.HasValue) { return null; }

                var sell = AfterRepairValue.Value;
                var buy = CurrentPriceValue();
                var profit = sell - buy;
                return profit;
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

        [JsonIgnore]
        [NotMapped]
        public string PreFlipAvgLPSqFt { get; set; }
        public string County { get; set; }

        public int PoolValue()
        {
            return (int)Enum.Parse(typeof(PoolType), this.Pool);
        }
    }
}
