using HouseFlipper.DataAccess;
using HouseFlipper.DataAccess.Csv;
using HouseFlipper.DataAccess.DB;
using System.IO;
using System;
using System.Collections.Generic;
using HouseFlipper.Utility.Objects.Pipeline;

namespace HouseFlipper.Utility.Objects.Commands
{
    public class ImportCommand : Command
    {
        public override string Description
        {
            get { return "Imports csv format listing data into database"; }
        }

        public override string Example
        {
            get { return string.Empty; }
        }

        public override string Format
        {
            get
            {
                return "<dataFolder>";
            }
        }

        public override List<Parameter> Parameters
        {
            get
            {
                return new List<Parameter>() { new Parameter("dataFolder", "Top-level directory containing listing data in csv format") };
            }
        }

        public override void Execute(params string[] args)
        {            
            if (args == null || args.Length != 1)
            {
                Usage();
                return;
            }

            var dataFolder = args[0];
            //var parallel = true;
            //var bulk = true;

            if (string.IsNullOrWhiteSpace(dataFolder))
            {
                throw new ArgumentException("Data folder path not specified");
            }

            var reader = new MlsReader(dataFolder, "*.csv", SearchOption.AllDirectories);

            var p1 = new Pipe<Stage>();
            var chain = new Pipeline.Stages.ConvertData();
            chain.Link(new Pipeline.Stages.CheckDuplicate());
            chain.Link(new Pipeline.Stages.CreateListing());
            p1.Add(chain);

            var p2 = new Pipe<ParallelTask>();
            p2.Add(new Pipeline.Tasks.ZipFlipsTask());
            p2.Add(new Pipeline.Tasks.SubdivFlipsTask());
            p2.Add(new Pipeline.Tasks.CountyFlipsTask());

            p1.Next(p2);

            new ImporterV2(reader, p1).Run();
        }
    }    
}
