using Hack.HouseFlipper.DataAccess.Csv;
using Hack.HouseFlipper.DataAccess.DB;
using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility
{
    public class Importer
    {
        private MlsContext context;
        private MlsDataReader reader;

        public Importer(MlsDataReader reader, MlsContext context)
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
                if (line.NewFile)
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

        public virtual MlsRow AddRecord(
            string[] colNames,
            string[] fields)
        {
            var data = new StringDictionary();
            for (var j = 0; j < fields.Length; j++)
            {
                var field = fields[j].Replace("\"", string.Empty);
                data.Add(colNames[j], field);
            }
            var record = new MlsRow(data);
            context.Listings.Add(record);
            return record;
        }        
    }
}
