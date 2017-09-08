using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.BusinessObjects.Aggregators
{
    public class FlipAggregator2
    {
        public PropertyListingsMap Execute(Listings dataSet)
        {            
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
            var results = (PropertyListingsMap)instance.Execute();
            return results;
        }
    }
}
