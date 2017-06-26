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
        public void Active()
        {
            // [ARRANGE]
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Tests\WebSite\data\flips.csv";
            var reader = new MlsReader(path);
            var listingToPropId = new Dictionary<string, string>();
            var soldSet = new PropertyListingsMap();

            var exclude = new List<string>()
            {
                "O5366758","L4711733","L4711734",
                "L4706089","L4706090","L4707116",
                "L4707117","L4703920","L4703921",
                "L4703922"
            };
            var include = new List<string>()
            {
                "L4705767","L4705768"
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

            var keys = new List<string>() { "MLNumber" };
            var results = new Listings();
            Func<object, bool> filter = (listing) => ((Listing)listing).IsActive();
            //Func<IDataSet,object> collect = (data) => data;                
            var instance = new Aggregator(dataSet, filter);

            // [ACT]
            results = (Listings)instance.Execute();


            // [ASSERT]
            foreach (var id in exclude)
            {
                Assert.AreEqual(0, results.Where((listing) => (string)listing[keys[0]] == id).Count(), "Error: {0} should be excluded", id);
            }

            foreach (var id in include)
            {
                Assert.AreEqual(1, results.Where((listing) => (string)listing[keys[0]] == id).Count(), "Error: {0} should be included", id);
            }
        }

        [Test]
        public void Sold()
        {
            // [ARRANGE]
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Tests\WebSite\data\flips.csv";
            var reader = new MlsReader(path);
            var listingToPropId = new Dictionary<string, string>();
            var soldSet = new PropertyListingsMap();

            var include = new List<string>()
            {
                "O5366758","L4711733","L4711734",
                "L4706089","L4706090","L4707116",
                "L4707117","L4703920","L4703921",
                "L4703922"
            };
            var exclude = new List<string>()
            {
                "L4705767","L4705768"
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

            var keys = new List<string>() { "MLNumber" };
            var results = new Listings();
            Func<object, bool> filter = (listing) => ((Listing)listing).IsSold();
            //Func<IDataSet,object> collect = (data) => data;                
            var instance = new Aggregator(dataSet, filter);

            // [ACT]
            results = (Listings)instance.Execute();


            // [ASSERT]
            foreach (var id in exclude)
            {
                Assert.AreEqual(0, results.Where((listing) => (string)listing[keys[0]] == id).Count(), "Error: {0} should be excluded", id);
            }

            foreach (var id in include)
            {
                Assert.AreEqual(1, results.Where((listing) => (string)listing[keys[0]] == id).Count(), "Error: {0} should be included", id);
            }
        }


        [Test]
        public void Flips()
        {
            // [ARRANGE]
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Tests\WebSite\data\flips.csv";
            var reader = new MlsReader(path);
            var listingToPropId = new Dictionary<string, string>();
            var soldSet = new PropertyListingsMap();

            // Expect instance Add method to .....
            // 1e. O5366758 -> not a flip (exclude)
            // 3e. L4706089 -> flip > 1 year (exclude)
            // 5e. L4705767 -> act (exclude)
            // 2i. L4711733 -> flip < 1 year (include)
            // 4i. L4707116 -> flip == 1 year (include)
            // 6i. L4703920 -> flip < 1 year, multiple times (include)

            var exclude = new List<string>()
            {
                "O5366758","L4706089","L4706090",
                "L4705767","L4705768",
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

            var keys = new List<string>() { "MLNumber" };
            Func<object, bool> filter = (listing) => ((Listing)listing).IsSold();

            Func<IDataSet, object> operation =
                (data) =>
                {
                    var map = new PropertyListingsMap();
                    var listings = (Listings)data;

                    foreach (var listing in listings)
                    {
                        var query = listings.Where((item) => item.PropertyId() == listing.PropertyId());
                        var count = query.Count();
                        if (count > 1)
                        {
                            foreach (var item in query)
                            {
                                map.Add(item);
                            }
                        }
                    }

                    var removeKeys = new List<string>();
                    foreach (var key in map.Keys)
                    {
                        var l = map[key];

                        for (int i = l.Count - 1; i > 0; i--)
                        {
                            var closeDate1 = l[i - 1].CloseDateValue();
                            var closeDate2 = l[i].CloseDateValue();

                            if (closeDate2.Value.Subtract(closeDate1.Value).TotalDays > 365)
                            {
                                l.RemoveAt(i);
                            }
                        }

                        if (l.Count < 2)
                        {
                            removeKeys.Add(key);
                        }
                    }

                    foreach (var key in removeKeys)
                    {
                        map.Remove(key);                        
                    }

                    return map;
                };

            var instance = new Aggregator(dataSet, filter, operation);

            // [ACT]
            var results = (PropertyListingsMap)instance.Execute();


            // [ASSERT]
            foreach (var id in exclude)
            {
                var query = dataSet.Where((listing) => listing.MLNumber.ToLower().Trim() == id.ToLower().Trim()).FirstOrDefault();
                var exPropId = query.PropertyId();
                Assert.IsFalse(results.ContainsKey(exPropId), "Error: {0} should be excluded", id);
            }

            foreach (var set in include)
            {
                var query = dataSet.Where((listing) => listing.MLNumber.ToLower().Trim() == set[0].ToLower().Trim()).FirstOrDefault();
                var incPropId = query.PropertyId();
                Assert.IsTrue(results.ContainsKey(incPropId));
                var list = results[incPropId];
                Assert.IsTrue(list != null && list.Count() == set.Count);

                foreach (var expected in set)
                {
                    Assert.IsTrue(list.Where((l) => l.MLNumber.Trim().ToLower() == expected.ToLower().Trim()).Count() == 1);
                }
            }
        }

        [Test]
        public void FlipsDateRange()
        {
            // [ARRANGE]
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Tests\WebSite\data\flips.csv";
            var reader = new MlsReader(path);
            var listingToPropId = new Dictionary<string, string>();
            var soldSet = new PropertyListingsMap();

            // Expect instance Add method to .....
            // 1e. O5366758 -> not a flip (exclude)
            // 3e. L4706089 -> flip > 1 year (exclude)
            // 5e. L4705767 -> act (exclude)
            // 2i. L4711733 -> flip < 1 year (include)
            // 4i. L4707116 -> flip == 1 year (include)
            // 6i. L4703920 -> flip < 1 year, multiple times (include)

            var exclude = new List<string>()
            {
                "O5366758","L4706089","L4706090","L4707116", "L4707117",
                "L4705767","L4705768",
                "P4705621","P4704119","K4700271"
            };
            var include = new List<List<string>>()
            {
                new List<string>(){ "L4711733", "L4711734" },
                new List<string>(){ "L4703920", "L4703921" }
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

            DateTime endDate = new DateTime(2016, 5, 11);
            DateTime startDate = endDate.Subtract(TimeSpan.FromDays(365));
            var keys = new List<string>() { "MLNumber" };
            Func<object, bool> filter = (listing) => ((Listing)listing).IsSold() && ((Listing)listing).CloseDateValue() >= startDate && ((Listing)listing).CloseDateValue() <= endDate;

            Func<IDataSet, object> operation =
                (data) =>
                {
                    var map = new PropertyListingsMap();
                    var listings = (Listings)data;

                    foreach (var listing in listings)
                    {
                        var query = listings.Where((item) => item.PropertyId() == listing.PropertyId());
                        var count = query.Count();
                        if (count > 1)
                        {
                            foreach (var item in query)
                            {
                                map.Add(item);
                            }
                        }
                    }

                    var removeKeys = new List<string>();
                    foreach (var key in map.Keys)
                    {
                        var l = map[key];

                        for (int i = l.Count - 1; i > 0; i--)
                        {
                            var closeDate1 = l[i - 1].CloseDateValue();
                            var closeDate2 = l[i].CloseDateValue();

                            if (closeDate2.Value.Subtract(closeDate1.Value).TotalDays > 365)
                            {
                                l.RemoveAt(i);
                            }
                        }

                        if (l.Count < 2)
                        {
                            removeKeys.Add(key);
                        }
                    }

                    foreach (var key in removeKeys)
                    {
                        map.Remove(key);
                    }

                    return map;
                };

            var instance = new Aggregator(dataSet, filter, operation);

            // [ACT]
            var results = (PropertyListingsMap)instance.Execute();


            // [ASSERT]
            foreach (var id in exclude)
            {
                var query = dataSet.Where((listing) => listing.MLNumber.ToLower().Trim() == id.ToLower().Trim()).FirstOrDefault();
                var exPropId = query.PropertyId();
                Assert.IsFalse(results.ContainsKey(exPropId), "Error: {0} should be excluded", id);
            }

            foreach (var set in include)
            {
                var query = dataSet.Where((listing) => listing.MLNumber.ToLower().Trim() == set[0].ToLower().Trim()).FirstOrDefault();
                var incPropId = query.PropertyId();
                Assert.IsTrue(results.ContainsKey(incPropId));
                var list = results[incPropId];
                Assert.IsTrue(list != null && list.Count() == set.Count);

                foreach (var expected in set)
                {
                    Assert.IsTrue(list.Where((l) => l.MLNumber.Trim().ToLower() == expected.ToLower().Trim()).Count() == 1);
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
