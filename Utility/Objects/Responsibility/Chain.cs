using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Responsibility
{
    public abstract class Chain : IChain
    {
        public object Output
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Link(IChain next)
        {
            throw new NotImplementedException();
        }

        public void Pass(object data)
        {
            throw new NotImplementedException();
        }

        public void Unlink()
        {
            throw new NotImplementedException();
        }
    }
}
