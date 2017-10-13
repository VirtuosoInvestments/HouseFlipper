using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Pipeline
{
    public interface IPipeOperation
    {
        void Execute(object data);
    }
}
