using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Test.HouseFlipper.DataAccess
{
    [TestFixture]
    public class MlsRowTest
    {
        class SomeObject
        {

        }
        [Test]
        public void Constructor()
        {
            var data = new StringDictionary();
            var list = new List<dynamic>
            {
                //{ "#", "a" },
                new {Field="ML Number",Value="a", Invoke= (Func<Listing,object>)((Listing t) => { return t.MLNumber; }) },
                new {Field="Status",Value="b" , Invoke= (Func<Listing,object>)((Listing t) => { return t.Status; }) },
                new {Field="Address",Value="c" , Invoke= (Func<Listing,object>)((Listing t) => { return t.Address; }) },
                new {Field="City",Value="d" , Invoke= (Func<Listing,object>)((Listing t) => { return t.City; }) },
                new {Field="Postal Code",Value="e" , Invoke= (Func<Listing,object>)((Listing t) => { return t.PostalCode; }) },
                new {Field="Legal Subdivision Name",Value="f" , Invoke= (Func<Listing,object>)((Listing t) => { return t.LegalSubdivisionName; }) },
                new {Field="Sq Ft Heated",Value="3" , Invoke= (Func<Listing,object>)((Listing t) => { return t.SqFtHeated; }) },
                new {Field="Current Price",Value="$12.99" , Invoke= (Func<Listing,object>)((Listing t) => { return t.CurrentPrice; }) },
                new {Field="Beds",Value="4" , Invoke= (Func<Listing,object>)((Listing t) => { return t.Beds; }) },
                new {Field= "Full Baths",Value="15" , Invoke= (Func<Listing,object>)((Listing t) => { return t.FullBaths; }) },
                new {Field="Half Baths",Value="3" , Invoke= (Func<Listing,object>)((Listing t) => { return t.HalfBaths; }) },
                new {Field="Year Built",Value="1981" , Invoke= (Func<Listing,object>)((Listing t) => { return t.YearBuilt; }) },
                new {Field="Pool",Value="No" , Invoke= (Func<Listing,object>)((Listing t) => { return t.Pool; }) },
                new {Field="Property Style",Value="Ranch" , Invoke= (Func<Listing,object>)((Listing t) => { return t.PropertyStyle; }) },
                new {Field="Taxes",Value="$1,299.43" , Invoke= (Func<Listing,object>)((Listing t) => { return t.Taxes; }) },
                new {Field= "CDOM",Value="223" , Invoke= (Func<Listing,object>)((Listing t) => { return t.CDOM; }) },
                new {Field="ADOM",Value="15" , Invoke= (Func<Listing,object>)((Listing t) => { return t.ADOM; }) },
                new {Field="Days to Contract",Value="3" , Invoke= (Func<Listing,object>)((Listing t) => { return t.DaystoContract; }) },
                new {Field="Sold Terms",Value="hello" , Invoke= (Func<Listing,object>)((Listing t) => { return t.SoldTerms; }) },
                new {Field="Close Date",Value="11/23/2081" , Invoke= (Func<Listing,object>)((Listing t) => { return t.CloseDate; }) },
                new {Field="LP/SqFt",Value="$17.21" , Invoke= (Func<Listing,object>)((Listing t) => { return t.LPSqFt; }) },
                new {Field="SP/SqFt",Value="$15.22" , Invoke= (Func<Listing,object>)((Listing t) => { return t.SPSqFt; }) },
                new {Field="SP/LP",Value=".81" , Invoke= (Func<Listing,object>)((Listing t) => { return t.SPLP; }) },
                new {Field="List Office Name",Value="haha" , Invoke= (Func<Listing,object>)((Listing t) => { return t.ListOfficeName; }) },
                new {Field="List Agent Full Name",Value="meknow" , Invoke= (Func<Listing,object>)((Listing t) => { return t.ListAgentFullName; }) },
                new {Field="List Agent ID",Value="43" , Invoke= (Func<Listing,object>)((Listing t) => { return t.ListAgentID; }) },
                new {Field="Selling Agent Name",Value="jajaja" , Invoke= (Func<Listing,object>)((Listing t) => { return t.SellingAgentName; }) },
                new {Field="Selling Office ID",Value="3" , Invoke= (Func<Listing,object>)((Listing t) => { return t.SellingOfficeID; }) },
                new {Field="Selling Agent ID",Value="433" , Invoke= (Func<Listing,object>)((Listing t) => { return t.SellingAgentID; }) },
                new {Field="LSC List Side",Value="cool" , Invoke= (Func<Listing,object>)((Listing t) => { return t.LSCListSide; }) },
                new {Field="Office Primary Board ID",Value="83838" , Invoke= (Func<Listing,object>)((Listing t) => { return t.OfficePrimaryBoardID; }) },
                new {Field="County", Value="Suffolk", Invoke= (Func<Listing,object>)((Listing t) => { return t.County; }) }
            };
            foreach(dynamic d in list)
            {                
                data.Add(d.Field, d.Value);
            }
            Listing m = new Listing(data);
            foreach (var o in list)
            {
                Assert.AreEqual(
                    o.Value,
                    o.Invoke(m));
            }
        }
    }
}
