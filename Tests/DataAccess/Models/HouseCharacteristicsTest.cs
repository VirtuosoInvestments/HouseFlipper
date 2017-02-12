using System;
using NUnit.Framework;
using Hack.HouseFlipper.DataAccess.Models;

namespace Tests.DataAccess.Models
{
    [TestFixture]
    public class HouseCharacteristicsTest
    {
        [Test]
        public void PriceStr()
        {
            var target = new HouseCharacteristics();
            target.Price = 4.99;
            Assert.AreEqual("$4.99", target.PriceStr);
        }

        [Test]
        public void PriceFractionAddZeroStr()
        {
            var target = new HouseCharacteristics();
            target.Price = 4.9;
            Assert.AreEqual("$4.90", target.PriceStr);
        }

        [Test]
        public void PriceWithCommaStr()
        {
            var target = new HouseCharacteristics();
            target.Price = 4000.9;
            Assert.AreEqual("$4,000.90", target.PriceStr);
        }
    }
}
