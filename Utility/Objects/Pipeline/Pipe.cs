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
        private List<IPipeOperation> operations = new List<IPipeOperation>();
        private IPipe next;
        public void Enter(object data)
        {
            Parallel.ForEach(operations, (op) => 
            {
                try
                {
                    op.Execute(data);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            });

            if(this.next!=null)
            {
                this.next.Enter(this.Output);
            }
        }

        public void Add(T operation)
        {
            this.operations.Add(operation);
        }

        public void Next(IPipe pipe)
        {
            this.next = pipe;
        }
    }
}
