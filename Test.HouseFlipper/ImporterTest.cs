using System;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hack.HouseFlipper.Utility;
using Hack.HouseFlipper.DataAccess.DB;
using System.Collections.Specialized;
using System.Collections.Generic;
using Hack.HouseFlipper.DataAccess.Models;
using System.Data.Entity;
using Moq;
using Hack.HouseFlipper.DataAccess.Csv;

namespace Test.HouseFlipper
{
    [TestClass]
    public class ImporterTest
    {
        [TestMethod]
        public void Import()
        {
            // Arrange
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Test.HouseFlipper\data\listing.csv";
            var readerMock = new Mock<MlsDataReader>(path);
            readerMock.Setup(x => x.ReadLine()).Returns(
                  new List<MlsDataLine>()
                  {
                      new MlsDataLine(
                          "\"#\",\"ML Number\",\"Status\",\"Address\",\"City\",\"Postal Code\",\"Legal Subdivision Name\",\"Sq Ft Heated\",\"Current Price\",\"Beds\",\"Full Baths\",\"Half Baths\",\"Year Built\",\"Pool\",\"Property Style\",\"Taxes\",\"CDOM\",\"ADOM\",\"Days to Contract\",\"Sold Terms\",\"Close Date\",\"LP/SqFt\",\"SP/SqFt\",\"SP/LP\",\"List Office Name\",\"List Agent Full Name\",\"List Agent ID\",\"Selling Agent Name\",\"Selling Office ID\",\"Selling Agent ID\",\"LSC List Side\",\"Office Primary Board ID\"",
                          true),

                      new MlsDataLine(
                          "\"1\",\"O5440207\",\"ACT\",\"141 E 12TH ST\",\"APOPKA\",\"32703\",\"CLARKSVILLE\",\"1,144\",\"$37,000\",\"3\",\"1\",\"0\",\"1959\",\"None\",\"Single Family Home\",\"$1,298\",\"1\",\"1\",\"\",\"\",\"\",\"$32.34\",\"\",\"\",\"CHARLES RUTENBERG REALTY\",\"Sara Nicholas\",\"269503895\",\"\",\"\",\"\",\"O\",\"Orlando Regional\"",
                          false)
                  }
                );

            var contextMock = new Mock<MlsContext>();
            var listingsMock = new Mock<DbSet<MlsRow>>();
            

            listingsMock.Setup(x => x.Add(It.IsAny<MlsRow>())).Returns((MlsRow u) => u);
            contextMock.Setup(x => x.Listings).Returns(listingsMock.Object);

            var importerMock = new Mock<Importer>(readerMock.Object, contextMock.Object);
            importerMock.Setup(x => x.AddRecord(
                It.Is<string[]>(h => EvaluateHeaders(h)),
                It.Is<string[]>(v => EvaluateValues(v))));

            // Act

            var _instance = importerMock.Object; //new Importer(readerMock.Object, contextMock.Object);
            _instance.Run();

            // Assert

            contextMock.Verify(x => x.SaveChanges());

            importerMock.Verify(x => x.AddRecord(It.IsAny<string[]>(), It.IsAny<string[]>()), Times.Once);
        }

        private bool EvaluateValues(string[] values)
        {
            var expected = new string[] {
                "1","O5440207","ACT","141 E 12TH ST","APOPKA","32703","CLARKSVILLE","1,144","$37,000","3","1","0","1959","None","Single Family Home","$1,298","1","1","","","","$32.34","","","CHARLES RUTENBERG REALTY","Sara Nicholas","269503895","","","","O","Orlando Regional"
            };

            Assert.AreEqual(expected.Length, values.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], values[i]);
            }
            return true;
        }

        private bool EvaluateHeaders(string[] headers)
        {
            var expected = new string[] {
                "#","ML Number","Status","Address","City","Postal Code","Legal Subdivision Name","Sq Ft Heated","Current Price","Beds","Full Baths","Half Baths","Year Built","Pool","Property Style","Taxes","CDOM","ADOM","Days to Contract","Sold Terms","Close Date","LP/SqFt","SP/SqFt","SP/LP","List Office Name","List Agent Full Name","List Agent ID","Selling Agent Name","Selling Office ID","Selling Agent ID","LSC List Side","Office Primary Board ID"
            };

            Assert.AreEqual(expected.Length, headers.Length);
            for(var i=0; i<expected.Length; i++)
            {
                Assert.AreEqual(expected[i], headers[i]);
            }
            return true;
        }

        [TestMethod]
        public void AddRecord()
        {
            var data = new StringDictionary();
            var list = new List<dynamic>
            {
                //{ "#", "a" },
                new {Field="ML Number",Value="a", Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.MLNumber; }) },
                new {Field="Status",Value="b" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.Status; }) },
                new {Field="Address",Value="c" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.Address; }) },
                new {Field="City",Value="d" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.City; }) },
                new {Field="Postal Code",Value="e" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.PostalCode; }) },
                new {Field="Legal Subdivision Name",Value="f" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.LegalSubdivisionName; }) },
                new {Field="Sq Ft Heated",Value="3" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.SqFtHeated; }) },
                new {Field="Current Price",Value="$12.99" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.CurrentPrice; }) },
                new {Field="Beds",Value="4" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.Beds; }) },
                new {Field= "Full Baths",Value="15" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.FullBaths; }) },
                new {Field="Half Baths",Value="3" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.HalfBaths; }) },
                new {Field="Year Built",Value="1981" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.YearBuilt; }) },
                new {Field="Pool",Value="No" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.Pool; }) },
                new {Field="Property Style",Value="Ranch" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.PropertyStyle; }) },
                new {Field="Taxes",Value="$1,299.43" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.Taxes; }) },
                new {Field= "CDOM",Value="223" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.CDOM; }) },
                new {Field="ADOM",Value="15" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.ADOM; }) },
                new {Field="Days to Contract",Value="3" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.DaystoContract; }) },
                new {Field="Sold Terms",Value="hello" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.SoldTerms; }) },
                new {Field="Close Date",Value="11/23/2081" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.CloseDate; }) },
                new {Field="LP/SqFt",Value="$17.21" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.LPSqFt; }) },
                new {Field="SP/SqFt",Value="$15.22" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.SPSqFt; }) },
                new {Field="SP/LP",Value=".81" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.SPLP; }) },
                new {Field="List Office Name",Value="haha" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.ListOfficeName; }) },
                new {Field="List Agent Full Name",Value="meknow" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.ListAgentFullName; }) },
                new {Field="List Agent ID",Value="43" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.ListAgentID; }) },
                new {Field="Selling Agent Name",Value="jajaja" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.SellingAgentName; }) },
                new {Field="Selling Office ID",Value="3" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.SellingOfficeID; }) },
                new {Field="Selling Agent ID",Value="433" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.SellingAgentID; }) },
                new {Field="LSC List Side",Value="cool" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.LSCListSide; }) },
                new {Field="Office Primary Board ID",Value="83838" , Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.OfficePrimaryBoardID; }) },
                new {Field="County", Value="Suffolk", Invoke= (Func<MlsRow,object>)((MlsRow t) => { return t.County; }) }
            };
            string[] headers = new string[list.Count];
            string[] values = new string[list.Count];
            var i = 0;
            foreach (dynamic d in list)
            {
                data.Add(headers[i] = d.Field, values[i++] = d.Value);
            }


            // Arrange

            var contextMock = new Mock<MlsContext>();
            var listingsMock = new Mock<DbSet<MlsRow>>();
            listingsMock.Setup(x => x.Add(It.IsAny<MlsRow>())).Returns((MlsRow u) => u);
            contextMock.Setup(x => x.Listings).Returns(listingsMock.Object);

            // Act

            var row = new Importer(null, contextMock.Object).AddRecord(
                headers,
                values);

            // Assert

            Assert.IsNotNull(row);

            foreach (dynamic o in list)
            {
                Assert.AreEqual(
                    o.Value,
                    o.Invoke(row));
            }

            listingsMock.Verify(
              x =>
                x.Add(
                  It.Is<MlsRow>(
                    u => Evaluate(u,list)
                  )),
                
                Times.Once);            
        }

        private bool Evaluate(MlsRow u, List<dynamic> list)
        { 
            foreach (var o in list)
            {
                Assert.AreEqual(
                    o.Value,
                    o.Invoke(u));
            }
            return true;
        }
    }

}
