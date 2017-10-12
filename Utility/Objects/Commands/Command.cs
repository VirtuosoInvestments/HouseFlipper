using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Commands
{
    public abstract class Command : ICommand
    {
        private string exe;
        private string alias;
        public Command()
        {
            exe = System.Reflection.Assembly.GetEntryAssembly().GetName().Name.ToLower();
            alias = this.GetType().Name.ToLower().Replace("command","");            
        }       

        public abstract string Description
        {
            get;
        }

        public abstract string Example
        {
            get;
        }

        public abstract string Format
        {
            get;
        }

        public virtual List<Parameter> Parameters
        {
            get { return new List<Parameter>(); }
        }

        public abstract void Execute(params string[] args);

        public void Usage()
        {
            //Console.WriteLine("Usage:");
            Console.WriteLine("{0} -{1} {2}", exe, alias, Format);            
            Console.WriteLine(Description);
            Console.WriteLine();
            var parameters = Parameters;
            if (parameters != null && parameters.Count > 0)
            {
                Console.WriteLine("Parameters:");
                foreach (var p in Parameters)
                {
                    Console.WriteLine("<{0}> {1}", p.Name, p.Description);
                }
                Console.WriteLine();
            }
        }
    }
}
