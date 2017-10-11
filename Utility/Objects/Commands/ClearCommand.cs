using HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Commands
{
    public class ClearCommand : ICommand
    {
        public string Description
        {
            get { return "Clears database of all data"; }
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
            using (var context = new MlsContext())
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE Listings");
            }
        }
    }
}
