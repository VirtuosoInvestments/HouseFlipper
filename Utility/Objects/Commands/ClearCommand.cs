using HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Commands
{
    public class ClearCommand : Command
    {
        public override string Description
        {
            get { return "Clears database of all data"; }
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
            Console.WriteLine("Are you sure you want to clear the database of all data? [y|n, default:n]");
            var key = Console.ReadKey().KeyChar.ToString().ToLower();
            Console.WriteLine();
            if (key == "y")
            {
                using (var context = new MlsContext())
                {
                    context.Database.ExecuteSqlCommand("TRUNCATE TABLE Listings");
                }
                Console.WriteLine("Database cleared of all data");
            }
            else
            {
                Console.WriteLine("Clear command not executed");
            }
        }
    }
}
