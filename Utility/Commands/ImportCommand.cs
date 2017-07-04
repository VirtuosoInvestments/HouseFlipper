using HouseFlipper.DataAccess;
using HouseFlipper.DataAccess.Csv;
using HouseFlipper.DataAccess.DB;
using System.IO;
using System;

namespace HouseFlipper.Utility
{
    public class ImportCommand : ICommand
    {
        public void Execute(params string[] args)
        {
            var parallel = false;
            var dataFolder = string.Empty;
            if(args==null || args.Length==0)
            {
                throw new ArgumentNullException("args");
            }
            foreach (var a in args)
            {
                var tmp = a.ToString().ToLower().Trim();
                if (tmp == "parallel")
                {
                    parallel = true;
                }
                else
                {
                    dataFolder = a;
                }
            }

            if(string.IsNullOrWhiteSpace(dataFolder))
            {
                throw new ArgumentException("Data folder path not specified");
            }

            var reader = new MlsReader(dataFolder, "*.csv", SearchOption.AllDirectories);
            using (var context = new MlsContext())
            {
                new Importer(reader, context).Run(parallel);
            }
        }
    }    
}
