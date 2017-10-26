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
        private List<IPipeOperation> parallelOperations = new List<IPipeOperation>();
        private IPipe next;

        public object Output
        {
            get;
            private set;
        }

        public void Enter(object data)
        {
            Parallel.ForEach(parallelOperations, (op) =>
            {
                try
                {
                    op.Execute(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            });
        }

        public void Add(T operation)
        {
            this.parallelOperations.Add(operation);
        }

        public void Next(IPipe pipe)
        {
            this.next = pipe;
        }

        public void HandleExit(object data)
        {
            if(this.next!=null)
            {
                this.next.Enter(data);
            }
            else
            {
                this.Output = data;
            }
        }
    }
}
