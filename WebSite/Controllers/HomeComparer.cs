using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace HouseFlipper.WebSite.Controllers
{
    public class HomeComparer : IComparer<Listing>
    {
        private string sortOrder;
        public HomeComparer(string sortOrder)
        {
            this.sortOrder = sortOrder;
        }
        public int Compare(Listing x, Listing y)
        {
            var prop = FindProperty(x);
            if (prop.PropertyType == typeof(Int32))
            {
                var val = (Int32)GetPropValue(x, prop);
                var val2 = (Int32)GetPropValue(y, prop);
                return val.CompareTo(val2);
            }
            else if (prop.PropertyType == typeof(Int64))
            {
                var val = (Int64)GetPropValue(x, prop);
                var val2 = (Int64)GetPropValue(y, prop);
                return val.CompareTo(val2);
            }
            else if (prop.PropertyType == typeof(Int16))
            {
                var val = (Int16)GetPropValue(x, prop);
                var val2 = (Int16)GetPropValue(y, prop);
                return val.CompareTo(val2);
            }
            else if (prop.PropertyType == typeof(Double))
            {
                var val = (Double)GetPropValue(x, prop);
                var val2 = (Double)GetPropValue(y, prop);
                return val.CompareTo(val2);
            }
            else if (prop.PropertyType == typeof(Double?))
            {
                var val = (Double?)GetPropValue(x, prop);
                var val2 = (Double?)GetPropValue(y, prop);
                if (!val.HasValue && !val.HasValue) { return 0; }
                if (!val.HasValue) { return -1; }
                if (!val2.HasValue) { return 1; }
                return val.Value.CompareTo(val2.Value);
            }
            else
            {
                var val = (string)GetPropValue(x, prop);
                var val2 = (string)GetPropValue(y, prop);
                return string.Compare(val, val2, true);
            }
        }

        private object GetPropValue(Listing m, PropertyInfo prop)
        {
            var val = prop.GetValue(m);
            return val;
        }

        private PropertyInfo FindProperty(Listing m)
        {
            var prop = m.GetType().GetProperty(sortOrder, BindingFlags.Instance | BindingFlags.Public);
            return prop;
        }
    }
}