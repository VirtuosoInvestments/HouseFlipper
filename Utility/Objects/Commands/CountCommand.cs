using HouseFlipper.DataAccess.Csv;
using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Commands
{    
    public class CountCommand : Command
    {
        public override string Description
        {
            get { return "Counts the number of times each property appears as sold or active within CSV files"; }
        }

        public override string Example
        {
            get { return string.Empty; }
        }

        public override string Format
        {
            get
            {
                return "<csvDataFolder>";
            }
        }

        public override void Execute(params string[] args)
        {
            var dataFolder = args[0];
            var reader = new MlsReader(dataFolder, "*.csv", SearchOption.AllDirectories);
            string[] fieldNames = null;
            var listingsCount = new Dictionary<string, CountData>();
            foreach (var line in reader.ReadLine())
            {
                var values = MlsTokenizer.Split(line.Text);
                if (line.IsHeader)
                {
                    fieldNames = values;
                }
                else
                {
                    var record = new Listing(ToDictionary(fieldNames, values));
                    var id = record.PropertyId();
                    CountData data = null;
                    if (listingsCount.ContainsKey(id))
                    {
                        data = listingsCount[id];
                    }
                    else
                    {
                        data = new CountData();
                        listingsCount.Add(id, data);
                    }
                    data.TotalCount++;
                    if (record.IsSold())
                    {
                        data.SoldCount++;
                    }
                    else if (record.IsActive())
                    {
                        data.ActiveCount++;
                    }
                }
            }
        }

        private static StringDictionary ToDictionary(string[] colNames, string[] fields)
        {
            var data = new StringDictionary();
            for (var j = 0; j < fields.Length; j++)
            {
                var field = fields[j];
                data.Add(colNames[j], field);
            }

            return data;
        }
    }

    public class CountData
    {
        public CountData()
        {
            TotalCount = 0;
            SoldCount = 0;
            ActiveCount = 0;
        }
        public int TotalCount { get; set; }
        public int SoldCount { get; set; }
        public int ActiveCount { get; set; }
    }
}
