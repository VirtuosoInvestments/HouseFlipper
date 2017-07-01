using HouseFlipper.DataAccess;
using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseFlipper.WebSite.Controllers
{
    public class NumericComparer : IComparer<double>
    {
        private string sortOrder;
        private SortDirection sortDirection;

        public NumericComparer(string sortOrder, SortDirection sortDirection)
        {
            // TODO: Complete member initialization
            this.sortOrder = sortOrder;
            this.sortDirection = sortDirection;
        }

        public int Compare(string s1, string s2)
        {
            var result = StringAscSort(s1, s2);
            //if(sortDirection== SortDirection.Ascending)
            //{
                return result;
            //}
            //return result - 1;
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