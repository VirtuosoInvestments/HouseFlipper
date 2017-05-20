using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hack.HouseFlipper.Utility;
using System.Collections.Generic;
using Hack.HouseFlipper.DataAccess.Models;

namespace Test.HouseFlipper
{
    [TestClass]
    public class PropertySearchTest
    {
        [TestMethod]
        public void Search()
        {
            var instance = new PropertySearch();
            var options = new PropertySearchOptions();
            List<Property> list = instance.Search(options);
        }
    }
}
