using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Commands
{
    public class DeleteCommand : ICommand
    {
        public void Execute(params string[] args)
        {
            if(Database.Exists("MlsContext"))
            {
                Database.Delete("MlsContext");              
            }
        }
    }
}
