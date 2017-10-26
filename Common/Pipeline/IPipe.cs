using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Common.Pipeline
{
    public interface IPipe
    {
        void Enter(object data);
        void Next(IPipe pipe);
        void HandleExit(object data);
    }
}
