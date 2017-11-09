using HouseFlipper.DataAccess.Models;
using HouseFlipper.Utility.Objects.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Pipeline.Tasks
{
    public class ZipFlipsTask : ParallelTask
    {
        public override void Execute(object data)
        {
            if (data is Tuple<List<Flip>, List<Flip>>)
            {
                var flipData = data as Tuple<List<Flip>, List<Flip>>;
                var flips = flipData.Item1;
                var oldFlips = flipData.Item2;

                var propNameList = new List<string>()
                {
                    //property names of listing we want to keep track
                    //of averages of
                    "CurrentPrice",
                    "Beds",
                    "FullBaths",
                    "HalfBaths",
                    "SqFtHeated",
                    "YearBuilt",
                    "Pool",
                    "Taxes",
                    "CDOM",
                    "ADOM",
                    "LPSqFt",
                    "SPSqFt",
                    "SPLP"
                };

                foreach (var oldFlip in oldFlips)
                {
                    var zip = oldFlip.Before.PostalCode;
                    Globals.ZipFlips.AddOrUpdate(
                        zip,
                        (x)=>
                        {
                            throw new NotImplementedException();
                        },
                        (k, currentProperties)=>
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
                }


                foreach (var newFlip in flips)
                {
                    var zip = newFlip.Before.PostalCode;
                    Globals.ZipFlips.AddOrUpdate(
                        zip,
                        (x) =>
                        {
                            var table = new Dictionary<string, Tuple<SumCount, SumCount>>();
                            foreach (var propName in propNameList)
                            {
                                var counts = new Tuple<SumCount, SumCount>(new SumCount(), new SumCount());
                                table.Add(propName,counts);
                                var beforeCount = counts.Item1;
                                var afterCount = counts.Item2;
                                var methodName = propName + "Value";
                                var beforeVal = newFlip.Before.InvokeMethod(methodName);
                                beforeCount.Add(Convert.ToDouble(beforeVal));
                                var afterVal = newFlip.After.InvokeMethod(methodName);
                                afterCount.Add(Convert.ToDouble(afterVal));
                            }
                            return table;
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
                }
            }
            else
            {
                throw new InvalidCastException("Error: data is expected to be Tuple<List<Flip>, List<Flip>>");
            }
        }
    }
}
