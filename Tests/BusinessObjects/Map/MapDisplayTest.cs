using HouseFlipper.BusinessObjects;
using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Test.HouseFlipper.BusinessObjects
{
    [TestFixture]
    [Category("NotImplemented")]
    public class MapDisplayTest
    {
        [Test]
        public void GetMarkers()
        {
            var instance = new MapDisplay();
            var list = new List<Property>();
            List<Marker> markers = instance.GetMarkers(list);
        }

        [Test]
        [Category("NotImplemented")]
        public void GeoLocation()
        {
            throw new NotImplementedException();
        }
    }
}
