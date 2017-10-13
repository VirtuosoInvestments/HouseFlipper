using HouseFlipper.Common.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Pipeline
{
    public class Pipe<T> : IPipe where T : IPipeOperation
    {
        public void Enter(object data)
        {
            throw new NotImplementedException();
        }

        public void Add(T operation)
        {
            throw new NotImplementedException();
        }

        public void Next(IPipe pipe)
        {
            throw new NotImplementedException();
        }
    }
}
