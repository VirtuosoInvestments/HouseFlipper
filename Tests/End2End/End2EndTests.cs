using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.HouseFlipper.Framework;

namespace Test.HouseFlipper.End2End
{
    [TestFixture]
    public class End2EndTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            LoadDb();
        }

        private void LoadDb()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void ListingsView()
        {
            var dbCount = new MlsDatabase().Listings.Count; // uses app.config to know which database to connect to
            var listingsView = new ListingsView();  // uses app.config to know which site url to use
            var results = listingsView.Results;
            Assert.AreEqual(dbCount, results.Count);
        }
    }
}
