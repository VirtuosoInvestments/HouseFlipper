using HouseFlipper.DataAccess.Csv;
using HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility
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
