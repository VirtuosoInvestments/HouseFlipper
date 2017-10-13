using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Pipeline
{
    public class Pipe<T> : IPipe where T : IPipeOperation
    {
        public void Next(IPipe pipe)
        {
            throw new NotImplementedException();
        }
    }
}
