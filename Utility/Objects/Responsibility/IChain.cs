using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Responsibility
{
    public interface IChain
    {
        void Link(IChain next);
        void Unlink();
        void Process(object data);
        object Output { get; }
    }
}
