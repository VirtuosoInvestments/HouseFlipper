using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects
{
    public class SumCount
    {        
        public SumCount(double sum, int count)
        {
            this.Sum = sum;
            this.Count = count;
        }
        public Double Sum { get; set; }
        public int Count { get; set; }
        public double Average()
        {
            return Sum / ((double)Count);
        }

        public void Add(double value)
        {
            this.Sum += value;
            this.Count++;
        }

        public void Subtract(double value)
        {
            this.Sum -= value;
            this.Count--;
        }
    }
}
