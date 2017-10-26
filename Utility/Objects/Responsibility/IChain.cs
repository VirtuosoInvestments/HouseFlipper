using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Responsibility
{
    public interface IChain
    {
        IChain Link(IChain next);
        void Unlink();
        void Process(object data);
        event Action<object> Exit;
    }
}
