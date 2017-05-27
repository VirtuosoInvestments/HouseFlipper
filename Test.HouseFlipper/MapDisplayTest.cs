using HouseFlipper.BusinessObjects;
using HouseFlipper.DataAccess.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace Test.HouseFlipper
{
    [TestFixture]
    public class MapDisplayTest
    {
        [Test]
        public void GetMarkers()
        {
            var instance = new MapDisplay();
            var list = new List<Property>();
            List<Marker> markers = instance.GetMarkers(list);
        }
    }
}
