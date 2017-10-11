using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Commands
{
    public class DeleteCommand : ICommand
    {
        public string Description
        {
            get { return "Deletes the House Flipper database"; }
        }

        public string Example
        {
            get { return string.Empty; }
        }

        public string Format
        {
            get
            {
                return string.Empty;
            }
        }

        public void Execute(params string[] args)
        {
            if(Database.Exists("MlsContext"))
            {
                Database.Delete("MlsContext");              
            }
        }
    }
}
