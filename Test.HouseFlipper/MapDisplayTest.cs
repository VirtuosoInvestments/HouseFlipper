using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hack.HouseFlipper.Utility;
using Hack.HouseFlipper.DataAccess.Models;
using System.Collections.Generic;

namespace Test.HouseFlipper
{
    [TestClass]
    public class MapDisplayTest
    {
        [TestMethod]
        public void GetMarkers()
        {
            var instance = new MapDisplay();
            var list = new List<Property>();
            List<Marker> markers = instance.GetMarkers(list);
        }
    }
}
