using HouseFlipper.BusinessObjects;
using HouseFlipper.DataAccess.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace Test.HouseFlipper.BusinessObjects
{
    [TestFixture]
    [Category("NotImplemented")]
    public class PropertySearchTest
    {
        [Test]
        public void Search()
        {
            var instance = new PropertySearch();
            var options = new PropertySearchOptions();
            List<Property> list = instance.Search(options);
        }
    }
}
