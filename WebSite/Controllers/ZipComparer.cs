using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WebSite.Controllers
{
    public class ZipComparer : IComparer<Homes>
    {
        private string sortOrder;
        public ZipComparer(string sortOrder)
        {
            this.sortOrder = sortOrder;
        }
        public int Compare(Homes x, Homes y)
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
            else
            {
                var val = (string)GetPropValue(x,prop);
                var val2 = (string)GetPropValue(y,prop);
                return string.Compare(val, val2, true);
            }
            
        }

        private object GetPropValue(Homes m, PropertyInfo prop)
        {
            var val = prop.GetValue(m);
            //if(!(val is string))
            //{
            //    return val.ToString();
            //}
            return val;
        }

        private PropertyInfo FindProperty(Homes m)
        {
            var prop = m.GetType().GetProperty(sortOrder, BindingFlags.Instance | BindingFlags.Public);
            return prop;
        }
    }
}