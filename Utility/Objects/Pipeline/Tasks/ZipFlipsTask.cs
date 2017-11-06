using HouseFlipper.DataAccess.Models;
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
                };

                foreach (var oldFlip in oldFlips)
                {
                    var zip = oldFlip.Before.PostalCode;
                    Globals.ZipFlips.AddOrUpdate(
                        zip,
                        (x)=>
                        {
                            return null;
                        },
                        (k, currentProperties)=>
                        {                            
                            foreach (var propName in propNameList)
                            {
                                var prop = currentProperties[propName];
                                var beforeCount = prop.Item1;
                                var afterCount = prop.Item2;
                                //before: property list (to subtract values for)
                                beforeCount.Subtract(oldFlip.Before.GetValue(propName));
                                beforeCount.Subtract(oldFlip.After.GetValue(propName));

                                //after: property list (to subtract values for)
                                
                            }
                            throw new NotImplementedException();
                            //return currentProperties;
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
                            throw new NotImplementedException();
                        },
                        (k, currentProperties) =>
                        {
                            foreach (var propName in propNameList)
                            {
                                var prop = currentProperties[propName];
                                var beforeCount = prop.Item1;
                                var afterCount = prop.Item2;
                                //before: property list (to add values for)
                                beforeCount.Add(newFlip.Before.GetValue(propName));
                                beforeCount.Add(newFlip.After.GetValue(propName));

                                //after: property list (to add values for)
                                
                            }
                            throw new NotImplementedException();
                            //return currentProperties;
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
