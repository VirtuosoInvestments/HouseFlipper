using HouseFlipper.DataAccess;
using HouseFlipper.DataAccess.Csv;
using HouseFlipper.DataAccess.DB;
using System.IO;

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
