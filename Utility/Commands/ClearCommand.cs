using Hack.HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility
{
    public class ClearCommand : ICommand
    {
        public void Execute(string[] args)
        {
            using (var context = new MlsContext())
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE Listings");
            }
        }
    }
}
