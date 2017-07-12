using HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Commands
{
    public class ClearCommand : ICommand
    {
        public void Execute(params string[] args)
        {
            using (var context = new MlsContext())
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE Listings");
            }
        }
    }
}
