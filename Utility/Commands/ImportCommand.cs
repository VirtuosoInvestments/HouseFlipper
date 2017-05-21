using Hack.HouseFlipper.DataAccess.Csv;
using Hack.HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility
{   
    public class ImportCommand : ICommand
    {
        public void Execute(params string[] args)
        {
            var dataFolder = args[0];
            var reader = new MlsReader(dataFolder, "*.csv", SearchOption.AllDirectories);
            using (var context = new MlsContext())
            {
                new Importer(reader, context).Run();
            }
        }
    }    
}
