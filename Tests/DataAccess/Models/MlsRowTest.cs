
using System;
using Hack.HouseFlipper.DataAccess.Models;
using NUnit.Framework;
using Hack.HouseFlipper.DataAccess.Csv;

namespace Tests.DataAccess.Models
{
    [TestFixture]
    public class MlsRowTest
    {
        [Test]
        public void DefaultConstructor()
        {
            var target = new MlsRow();
            Assert.IsNull(target.Lookup);
            Assert.AreEqual(0,target.ID);
            Assert.IsNull(target.MLNumber);
            Assert.IsNull(target.Status);
            Assert.IsNull(target.Address);
            Assert.IsNull(target.City);
            Assert.IsNull(target.PostalCode);
            Assert.IsNull(target.LegalSubdivisionName);
            Assert.IsNull(target.SqFtHeated);
            Assert.IsNull(target.CurrentPrice);
            Assert.IsNull(target.Beds);
            Assert.IsNull(target.FullBaths);
            Assert.IsNull(target.HalfBaths);
            Assert.IsNull(target.YearBuilt);
            Assert.IsNull(target.Pool);
            Assert.IsNull(target.PropertyStyle);
            Assert.IsNull(target.Taxes);
            Assert.IsNull(target.CDOM);
            Assert.IsNull(target.ADOM);
            Assert.IsNull(target.DaystoContract);
            Assert.IsNull(target.SoldTerms);
            Assert.IsNull(target.CloseDate);
            Assert.IsNull(target.LPSqFt);
            Assert.IsNull(target.SPSqFt);
            Assert.IsNull(target.SPLP);
            Assert.IsNull(target.ListOfficeName);
            Assert.IsNull(target.ListAgentFullName);
            Assert.IsNull(target.ListAgentID);
            Assert.IsNull(target.SellingAgentName);
            Assert.IsNull(target.SellingOfficeID);
            Assert.IsNull(target.SellingAgentID);
            Assert.IsNull(target.LSCListSide);
            Assert.IsNull(target.OfficePrimaryBoardID);
        }

        [Test]
        public void ADOMValue()
        {
            var random = new Random();
            var val = "100,000,001";
            var target = new MlsRow();
            target.ADOM = val;

            var actual = target.ADOMValue();
            Assert.AreEqual(100000001, actual);
        }

        [Test]
        public void CDOMValue()
        {
            var random = new Random();
            var val = "-100,000,001.9";
            var target = new MlsRow();
            target.CDOM = val;

            var actual = target.CDOMValue();
            Assert.AreEqual(-100000001.9, actual);
        }

        [Test]
        public void LPSqFtValue()
        {
            var random = new Random();
            var val = "5123";
            var target = new MlsRow();
            target.LPSqFt = val;

            var actual = target.LPSqFtValue();
            Assert.AreEqual(5123, actual);
        }

        [Test]
        public void SPSqFtValue()
        {
            var random = new Random();
            var val = "5123";
            var target = new MlsRow();
            target.SPSqFt = val;

            var actual = target.SPSqFtValue();
            Assert.AreEqual(5123, actual);
        }

        [Test]
        public void SPLPValue()
        {
            var random = new Random();
            var val = "5123";
            var target = new MlsRow();
            target.SPLP = val;

            var actual = target.SPLPValue();
            Assert.AreEqual(5123, actual);
        }

        [Test]
        public void ListAgentIDValue()
        {
            var random = new Random();
            var val = "5123";
            var target = new MlsRow();
            target.ListAgentID = val;

            var actual = target.ListAgentIDValue();
            Assert.AreEqual(5123, actual);
        }

        [Test]
        public void SellingAgentIDValue()
        {
            var random = new Random();
            var val = "5123";
            var target = new MlsRow();
            target.SellingAgentID = val;

            var actual = target.SellingAgentIDValue();
            Assert.AreEqual(5123, actual);
        }

        [Test]
        public void CloseDateValue()
        {
            var random = new Random();
            var val = "07/25/2015";
            var target = new MlsRow();
            target.CloseDate = val;

            var actual = target.CloseDateValue();
            Assert.AreEqual(new DateTime(2015,07,25), actual);

            target.CloseDate = null;
            actual = target.CloseDateValue();
            Assert.AreEqual(null, actual);
        }

        [Test]
        public void PostalCodeValue()
        {
            var random = new Random();
            var val = "32835";
            var target = new MlsRow();
            target.PostalCode = val;

            var actual = target.PostalCodeValue();
            Assert.AreEqual(32835, actual);
        }

        [Test]
        public void CompareTo()
        {
            var random = new Random();
            var val = "07/25/2015";
            var target = new MlsRow();
            target.CloseDate = val;

            val = "08/25/2115";
            var target2 = new MlsRow();
            target2.CloseDate = val;

            var actual = target.CloseDateValue();
            Assert.AreEqual(new DateTime(2015, 07, 25), actual);

            var actual2 = target2.CloseDateValue();
            Assert.AreEqual(new DateTime(2115, 08, 25), actual2);

            var comp = target.CompareTo(target2);
            Assert.AreEqual(-1, comp);

            comp = target2.CompareTo(target);
            Assert.AreEqual(1, comp);

            Assert.AreEqual(0, target.CompareTo(target));
            Assert.AreEqual(0, target2.CompareTo(target2));

            Assert.Throws(
                typeof(ArgumentException),
                () => target.CompareTo(null));
            Assert.AreEqual(1, target.CompareTo(new MlsRow() { CloseDate = null }));

            Assert.Throws(
                typeof(ArgumentException), 
                ()=>
                target.CompareTo(new MlsDataLine(string.Empty,true)));

            Assert.AreEqual(-1, new MlsRow() { CloseDate = null }.CompareTo(target));

        }
    }
}
