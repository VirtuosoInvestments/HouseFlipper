using HouseFlipper.DataAccess.Models;
using HouseFlipper.Utility.Objects.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Pipeline.Tasks
{
    public class CountyFlipsTask : ParallelTask
    {
        public override void Execute(object data)
        {
            if (data is Tuple<List<Flip>, List<Flip>>)
            {
                var flipData = data as Tuple<List<Flip>, List<Flip>>;
                var flips = flipData.Item1;
                var oldFlips = flipData.Item2;
                var propNameList = Globals.FlipProperties;

                var table = Globals.CountyFlipTotals;
                var table2 = Globals.CountyFlips;

                foreach (var oldFlip in oldFlips)
                {
                    var county = oldFlip.Before.County;
                    table.AddOrUpdate(
                        county,
                        (x) =>
                        {
                            throw new NotImplementedException();
                        },
                        (k, currentProperties) =>
                        {
                            foreach (var propName in propNameList)
                            {
                                var prop = currentProperties[propName];
                                var beforeCount = prop.Item1;
                                var afterCount = prop.Item2;
                                var methodName = propName + "Value";
                                beforeCount.Subtract((double)oldFlip.Before.InvokeMethod(methodName));
                                afterCount.Subtract((double)oldFlip.After.InvokeMethod(methodName));
                            }
                            return currentProperties;
                        }
                    );

                    table2.AddOrUpdate(
                        county,
                        (x) =>
                        {
                            throw new NotImplementedException();
                        },
                        (k, currentProperties) =>
                        {
                            currentProperties.Remove(oldFlip.PropertyId);
                            return currentProperties;
                        });
                }


                foreach (var newFlip in flips)
                {
                    var county = newFlip.Before.County;
                    table.AddOrUpdate(
                        county,
                        (x) =>
                        {
                            var propsTable = new Dictionary<string, Tuple<SumCount, SumCount>>();
                            foreach (var propName in propNameList)
                            {
                                var counts = new Tuple<SumCount, SumCount>(new SumCount(), new SumCount());
                                propsTable.Add(propName, counts);
                                var beforeCount = counts.Item1;
                                var afterCount = counts.Item2;
                                var methodName = propName + "Value";
                                var beforeVal = newFlip.Before.InvokeMethod(methodName);
                                beforeCount.Add(Convert.ToDouble(beforeVal));
                                var afterVal = newFlip.After.InvokeMethod(methodName);
                                afterCount.Add(Convert.ToDouble(afterVal));
                            }
                            return propsTable;
                        },
                        (k, currentProperties) =>
                        {
                            foreach (var propName in propNameList)
                            {
                                var prop = currentProperties[propName];
                                var beforeCount = prop.Item1;
                                var afterCount = prop.Item2;
                                var methodName = propName + "Value";
                                beforeCount.Add((double)newFlip.Before.InvokeMethod(methodName));
                                afterCount.Add((double)newFlip.After.InvokeMethod(methodName));
                            }
                            return currentProperties;
                        }
                    );

                    table2.AddOrUpdate(
                        county,
                        (x) =>
                        {
                            throw new NotImplementedException();
                        },
                        (k, currentProperties) =>
                        {
                            currentProperties.Add(newFlip.PropertyId);
                            return currentProperties;
                        });
                }
            }
            else
            {
                throw new InvalidCastException("Error: data is expected to be Tuple<List<Flip>, List<Flip>>");
            }
        }
    }
}
