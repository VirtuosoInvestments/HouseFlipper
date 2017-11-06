using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Pipeline.Stages
{
    public class AddFlip : Stage
    {
        public override void Process(object data)
        {
            if (data is List<Flip>)
            {
                var oldFlips = new List<Flip>();
                var flips = data as List<Flip>;
                foreach(var newFlip in flips)
                {
                    var propertyId = newFlip.PropertyId;
                    /*Globals.Flips.AddOrUpdate(
                        propertyId,
                        (x) => new List<Flip>() { flip },
                        (k, currentList) =>
                        {
                            Update(currentList, flip);
                            return currentList;
                        });*/

                    Globals.PropertyFlips.AddOrUpdate(
                            propertyId,
                            (x) =>
                            {
                                var flipHash = new Dictionary<DateTime, Flip>();
                                flipHash.Add(newFlip.Before.CloseDateValue().Value, newFlip);
                                return flipHash;
                            },
                            (k, currentFlipHash) =>
                            {
                                var date = newFlip.Before.CloseDateValue().Value;
                                if(currentFlipHash.ContainsKey(date))
                                {
                                    var prevFlip = currentFlipHash[date];
                                    if(newFlip.After.CloseDateValue()<prevFlip.After.CloseDateValue())
                                    {
                                        currentFlipHash[date] = newFlip;
                                        oldFlips.Add(prevFlip);
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("Error: New flip end date is >= to current flip with same start date!");
                                    }
                                }
                                else
                                {
                                    currentFlipHash.Add(newFlip.Before.CloseDateValue().Value, newFlip);
                                }
                                return currentFlipHash;
                            });                    
                }
                this.Pass(new Tuple<List<Flip>,List<Flip>>(flips,oldFlips));
            }
            else
            {
                throw new InvalidCastException("Error: data is expected to be List<Flip>");
            }
        }
        /*
        private void Update(List<Flip> currentList, Flip newFlip)
        {
            //TODO: Replace with interval tree implemenation Insert/Update
            var newStart = newFlip.Before.CloseDateValue();
            var newEnd = newFlip.After.CloseDateValue();

            if(newStart>newEnd)
            {
                throw new InvalidOperationException("Error: Invalid flip record");
            }

            for (int i=currentList.Count-1; i>=0; i--)
            {
                var item = currentList[i];
                var start = item.Before.CloseDateValue();
                var end = item.After.CloseDateValue();

                if(newStart==start && newEnd==end)
                {
                     throw new InvalidOperationException("Error: Duplicate flip record being entered");
                }

                if(start<=newStart && newStart<end &&
                   start<newEnd && newEnd<=end)
                {
                    if (i < (currentList.Count - 1))
                    {
                        currentList.Insert(i + 1, newFlip);
                    }
                    else
                    {
                        currentList.Add(newFlip);
                    }
                    currentList.RemoveAt(i);
                    break;
                }

                if(start>newStart && 
                   start<newEnd && newEnd<=end)
                {
                    InsertAfterAndRemove(currentList, newFlip, i);

                    if(i>0)
                    {
                        var prev = i - 1;
                        var prevItem = currentList[i - 1];
                        currentList.RemoveAt(prev);                      
                    }
                    break;
                }

                if(newEnd<=start)
                {
                    if(i>0)
                    {
                        var prev = i - 1;
                        var prevItem = currentList[i - 1];
                        var prevEnd = prevItem.After.CloseDateValue();

                        if(prevEnd<=newStart)
                        {
                            currentList.Insert(i, newFlip);
                            break;
                        }
                    }
                    else if(i==0)
                    {
                        currentList.Insert(i, newFlip);
                        break;
                    }
                }
            }

            throw new InvalidOperationException("Error: new flip not placed in current list!");
        }

        private static void InsertAfterAndRemove(List<Flip> currentList, Flip newFlip, int i)
        {
            if (i < (currentList.Count - 1))
            {
                currentList.Insert(i + 1, newFlip);
            }
            else
            {
                currentList.Add(newFlip);
            }
            currentList.RemoveAt(i);
        }
        */
    }
}
