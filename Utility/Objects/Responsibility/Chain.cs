using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Responsibility
{
    public abstract class Chain : IChain
    {
        private IChain next;
        public object Output
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Link(IChain next)
        {
            this.next = next;
        }

        public virtual void Process(object data)
        {
            throw new NotImplementedException();
        }

        public void Unlink()
        {
            this.next = null;
        }

        internal void Pass(object data)
        {
            if(next!=null)
            {
                next.Process(data);
            }
        }
    }
}
