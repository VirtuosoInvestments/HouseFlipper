using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Commands
{
    public interface ICommand
    {
        string Format { get; }
        string Description { get; }
        string Example { get; }
        void Execute(string[] args);
    }
}
