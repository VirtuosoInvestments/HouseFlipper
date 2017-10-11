using HouseFlipper.DataAccess;
using HouseFlipper.DataAccess.Csv;
using HouseFlipper.DataAccess.DB;
using System.IO;
using System;

namespace HouseFlipper.Utility.Objects.Commands
{
    public class ImportCommand : ICommand
    {
        public string Description
        {
            get { return "Imports csv format listing data into database"; }
        }

        public string Example
        {
            get { return string.Empty; }
        }

        public string Format
        {
            get
            {
                return "[parallel | bulk] <dataFolder>" ;
            }
        }

        public void Execute(params string[] args)
        {
            var parallel = false;
            var bulk = false;
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
                else if(tmp=="bulk")
                {
                    bulk = true;
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
            new Importer(reader, parallel).Run(bulk);            
        }
    }    
}
