using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Commands
{
    public class DeleteCommand : Command
    {
        public override string Description
        {
            get { return "Deletes the House Flipper database"; }
        }

        public override string Example
        {
            get { return string.Empty; }
        }

        public override string Format
        {
            get
            {
                return string.Empty;
            }
        }

        public override void Execute(params string[] args)
        {
            if(Database.Exists("MlsContext"))
            {
                Database.Delete("MlsContext");              
            }
        }
    }
}
