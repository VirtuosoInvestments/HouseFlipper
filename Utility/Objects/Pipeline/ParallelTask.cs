using HouseFlipper.Common.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Pipeline
{
    public abstract class ParallelTask : IPipeOperation
    {
        public abstract void Execute(object data);
    }
}
