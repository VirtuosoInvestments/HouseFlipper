using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WebSite.Controllers
{
    public class MlsRowComparer : IComparer<Listing>
    {
        private string sortOrder;
        public MlsRowComparer(string sortOrder)
        {
            this.sortOrder = sortOrder;
        }

        public int Compare(Listing x, Listing y)
        {
            var numericProps = new List<string>() { "CurrentPrice", "SqFtHeated" };

            var hasValueMethod = numericProps.Contains(sortOrder) || 
            x.GetType().FindMembers(
                MemberTypes.Method,
                BindingFlags.Public | BindingFlags.Instance,
                (m, criteria) => string.Equals(m.Name,criteria.ToString(), StringComparison.OrdinalIgnoreCase),
                sortOrder + "Value").Length>0;

            if (hasValueMethod)
            {
                var val = GetMethodValue(x);
                var val2 = GetMethodValue(y);
                if (val is Double)
                {
                    var dval = (Double)val;
                    var dval2 = (Double)val2;
                    return dval.CompareTo(dval2);
                }
                else if(val is DateTime)
                {
                    var dtval = (DateTime)val;
                    var dtval2 = (DateTime)val2;
                    return dtval.CompareTo(dtval2);
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Error: Unhandled type '{0}'",val!=null?val.GetType().Name:"null"));
                }

            }
            else
            {                
                var val = (string) GetPropValue(x);
                var val2 = (string)GetPropValue(y);
                return string.Compare(val, val2, true);
            }            
        }

        private object GetPropValue(Listing m)
        {
            var prop = m.GetType().GetProperty(sortOrder, BindingFlags.Instance | BindingFlags.Public);
            var val = prop.GetValue(m);
            return val;
        }

        private object GetMethodValue(Listing m)
        {
            var mth = m.GetType().GetMethod(sortOrder + "Value", BindingFlags.Instance | BindingFlags.Public);
            var val = mth.Invoke(m, null);
            return val;
        }

        private static int StringAscSort(string s1, string s2)
        {
            if (IsNumeric(s1) && IsNumeric(s2))
            {
                if (Convert.ToInt32(s1) > Convert.ToInt32(s2)) return 1;
                if (Convert.ToInt32(s1) < Convert.ToInt32(s2)) return -1;
                if (Convert.ToInt32(s1) == Convert.ToInt32(s2)) return 0;
            }

            if (IsNumeric(s1) && !IsNumeric(s2))
                return -1;

            if (!IsNumeric(s1) && IsNumeric(s2))
                return 1;

            return string.Compare(s1, s2, true);
        }

        public static bool IsNumeric(object value)
        {
            try
            {
                int i = Convert.ToInt32(value.ToString());
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public int Compare(double x, double y)
        {
            var result = Convert.ToDouble(x).CompareTo(Convert.ToDouble(y));
            //if(sortDirection == SortDirection.Ascending)
            //{
            return result;
            //}
            //return result - 1;
        }
    }
}