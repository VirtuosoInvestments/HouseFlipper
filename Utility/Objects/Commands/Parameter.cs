using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Commands
{
    public class Parameter
    {
        public Parameter(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
        public string Name { get; private set; }
        public string Description { get; private set; }
    }
}
