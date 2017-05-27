using HouseFlipper.DataAccess.Csv;
using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Specialized;

namespace HouseFlipper.DataAccess
{
    public class Importer
    {
        private MlsContext context;
        private MlsReader reader;

        public Importer(MlsReader reader, MlsContext context)
        {
            this.reader = reader;
            this.context = context;
        }
        public void Run()
        {
            string[] fieldNames = null;
            var rowNum = 0;
            foreach (var line in reader.ReadLine())
            {
                ++rowNum;
                Console.WriteLine("{0}: {1}", rowNum, line.Text);
                var values = MlsTokenizer.Split(line.Text);
                if (line.IsHeader)
                {
                    fieldNames = values;
                }
                else
                {
                    AddRecord(fieldNames, values);
                }
            }
            context.SaveChanges();
        }

        public virtual Listing AddRecord(
            string[] colNames,
            string[] fields)
        {
            StringDictionary data = ToDictionary(colNames, fields);
            var record = new Listing(data);
            context.Listings.Add(record);
            return record;
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
}
