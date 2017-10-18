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
                op.Execute(data);
            });
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
