using HouseFlipper.Common.Pipeline;
using HouseFlipper.Utility.Objects.Responsibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Pipeline
{
    public abstract class Stage : Chain, IPipeOperation
    {
        public void Execute(object data)
        {
            this.Process(data);
        }
    }
}
