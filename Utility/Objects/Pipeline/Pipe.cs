using HouseFlipper.Common;
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
                var opName = op.GetType().Name;
                try
                {
                    Logger.Debug("{0}: Start", opName);
                    op.Execute(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }
                finally
                {
                    Logger.Debug("{0}: End", opName);
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
