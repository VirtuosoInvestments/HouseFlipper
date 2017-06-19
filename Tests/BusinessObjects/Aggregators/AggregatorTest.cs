using HouseFlipper.BusinessObjects;
using HouseFlipper.DataAccess.Csv;
using HouseFlipper.DataAccess.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Test.HouseFlipper.BusinessObjects
{
    [TestFixture]    
    public class AggregatorTest
    {
        [Test]
        [Category("NotImplemented")]
        public void Count()
        {
            throw new NotImplementedException();
        }
        [Test]
        [Category("NotImplemented")]
        public void Active()
        {
            // [ARRANGE]
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Tests\WebSite\data\flips.csv";
            var reader = new MlsReader(path);
            var listingToPropId = new Dictionary<string, string>();
            var soldSet = new PropertyListingsMap();                        

            var exclude = new List<string>()
            {
                "O5366758","L4705767","L4707116",
                "P4705621","P4704119","K4700271"
            };
            var include = new List<List<string>>()
            {
                new List<string>(){ "L4711733", "L4711734" },
                new List<string>() { "L4707116", "L4707117" },
                new List<string>(){ "L4703920", "L4703921", "L4703922" }
            };

            var dataSet = new Listings();
            var converter = new Converter();
            foreach (var row in reader.ReadLine())
            {
                var listing = converter.Convert(row);

                if (listing != null)
                {
                    dataSet.Add(listing);
                }
            }

            var keys = new List<string>(){ "MLSNumber" };
            var results = new Listings();
            Func<object,bool> where = (listing) => ((Listing)listing).IsActive();
            Action<object> collect = (listing) => results.Add(listing as Listing);                
            var instance = new Aggregator(dataSet);

            // [ACT]
            instance.Execute(where,collect);


            // [ASSERT]
            foreach (var id in exclude)
            {
                Assert.IsFalse(results.Where((listing)=>(string)listing[keys[0]]==id).Count()>0);
            }

            foreach (var id in include)
            {
                Assert.IsTrue(results.Where((listing) => listing[keys[0]] == id).Count() == 1);
            }
        }

        [Test]
        [Category("NotImplemented")]
        public void Sold()
        {
            // [ARRANGE]
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Tests\WebSite\data\flips.csv";
            var reader = new MlsReader(path);
            var listingToPropId = new Dictionary<string, string>();
            var soldSet = new PropertyListingsMap();
            var instance = new OldAggregator(new RuleSet(RuleCondition.AND, new FlipRule(soldSet), new SoldRule(soldSet)));
            var converter = new Converter();

            // Expect instance Add method to .....
            // 1n. O5366758 -> not a flip (not added)
            // 2a. L4711733 -> flip < 1 year (added)
            // 3n. L4707116 -> flip > 1 year (not added)
            // 4a. L4707116 -> flip == 1 year (added)
            // 5n. L4705767 -> act (not added)
            // 6a. L4703920 -> flip < 1 year, multiple times (added)

            var notAdded = new List<string>()
            {
                "O5366758","L4705767","L4707116",
                "P4705621","P4704119","K4700271"
            };
            var added = new List<List<string>>()
            {
                new List<string>(){ "L4711733", "L4711734" },
                new List<string>() { "L4707116", "L4707117" },
                new List<string>(){ "L4703920", "L4703921", "L4703922" }
            };


            // [ACT]
            foreach (var row in reader.ReadLine())
            {
                var listing = converter.Convert(row);

                if (listing != null)
                {
                    instance.Add(listing);

                    var mlsNumber = listing.MLNumber;
                    var id = listing.PropertyId();
                    listingToPropId.Add(mlsNumber, id);
                }
            }

            // [ASSERT]
            foreach (var not in notAdded)
            {
                Assert.IsFalse(instance.DataSet.ContainsKey(listingToPropId[not]));
            }

            foreach (var add in added)
            {
                int i = 0;
                var propId = listingToPropId[add[i++]];
                Assert.IsTrue(instance.DataSet.ContainsKey(propId));

                foreach (var flip in instance.DataSet[propId])
                {
                    var sold1 = add[i - 1];
                    var sold2 = add[i];
                    Assert.AreEqual(propId, flip.PropertyId);
                    Assert.AreEqual(sold1, flip.Before.MLNumber);
                    Assert.AreEqual(sold2, flip.After.MLNumber);
                    ++i;
                }
            }
        }


        [Test]
        [Category("Regression")]
        public void Flips()
        {
            // [ARRANGE]
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Tests\WebSite\data\flips.csv";
            var reader = new MlsReader(path);
            var listingToPropId = new Dictionary<string, string>();            
            var soldSet = new PropertyListingsMap();
            var instance = new OldAggregator(new RuleSet(RuleCondition.AND, new FlipRule(soldSet), new SoldRule(soldSet)));
            var converter = new Converter();

            // Expect instance Add method to .....
            // 1n. O5366758 -> not a flip (not added)
            // 2a. L4711733 -> flip < 1 year (added)
            // 3n. L4707116 -> flip > 1 year (not added)
            // 4a. L4707116 -> flip == 1 year (added)
            // 5n. L4705767 -> act (not added)
            // 6a. L4703920 -> flip < 1 year, multiple times (added)

            var notAdded = new List<string>()
            {
                "O5366758","L4705767","L4707116",
                "P4705621","P4704119","K4700271"
            };
            var added = new List<List<string>>()
            {
                new List<string>(){ "L4711733", "L4711734" },
                new List<string>() { "L4707116", "L4707117" },
                new List<string>(){ "L4703920", "L4703921", "L4703922" }
            };


            // [ACT]
            foreach (var row in reader.ReadLine())
            {
                var listing = converter.Convert(row);
                
                if (listing != null)
                {                    
                    instance.Add(listing);

                    var mlsNumber = listing.MLNumber;
                    var id = listing.PropertyId();
                    listingToPropId.Add(mlsNumber, id);
                }
            }
            
            // [ASSERT]
            foreach (var not in notAdded)
            {
                Assert.IsFalse(instance.DataSet.ContainsKey(listingToPropId[not]));
            }

            foreach (var add in added)
            {
                int i = 0;
                var propId = listingToPropId[add[i++]];
                Assert.IsTrue(instance.DataSet.ContainsKey(propId));
                
                foreach(var flip in instance.DataSet[propId])
                {
                    var sold1 = add[i-1];
                    var sold2 = add[i];
                    Assert.AreEqual(propId, flip.PropertyId);
                    Assert.AreEqual(sold1, flip.Before.MLNumber);
                    Assert.AreEqual(sold2, flip.After.MLNumber);
                    ++i;
                }
            }
        }
    

        public void DimensionsTest()
        { }

        public void WithinTest()
        { }
    }

    public class Converter
    {
        private string[] headers = null;

        public Listing Convert(MlsRow row)
        {
            if (row.IsHeader)
            {
                headers = MlsTokenizer.Split(row.Text);
                return null;
            }

            if (headers == null)
            {
                throw new InvalidOperationException();
            }
            var values = MlsTokenizer.Split(row.Text);
            return new Listing(ToDictionary(headers, values));
        }

        private static StringDictionary ToDictionary(string[] colNames, string[] fields)
        {
            var data = new StringDictionary();
            for (var j = 0; j < fields.Length; j++)
            {
                var field = fields[j];
                data.Add(colNames[j], field);
            }

            return data;
        }
    }
}
