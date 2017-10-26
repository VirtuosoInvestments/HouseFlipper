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

        public event Action<object> Exit;

        public IChain Link(IChain next)
        {
            this.next = next;
            return this.next;
        }

        public abstract void Process(object data);

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
            else
            {
                if(this.Exit!=null)
                {
                    this.Exit.BeginInvoke(data, null, null);
                }
            }
        }
    }
}
