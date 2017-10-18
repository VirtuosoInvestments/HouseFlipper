using HouseFlipper.DataAccess.Csv;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Pipeline.Stages
{
    public class ConvertData : Stage
    {
        public override void Process(object data)
        {
            /*if(data is MlsRow)
            {
                var mlsRow = data as MlsRow;
                var values = MlsTokenizer.Split(mlsRow.Text);
                if (mlsRow.IsHeader)
                {
                    colNames = values;
                }
                else
                {
                    //Task.Run(() => pipe.Enter(mlsRow));
                    pipe.Enter(new object[] { colNames, values, file });
                }
            }*/

            if (data is object[])
            {
                var args = data as object[];
                var colNames = args[0] as string[];
                var values = args[1] as string[];
                var file = args[2] as string;

                var dict = ToDictionary(colNames, values);
                if(!dict.ContainsKey("County"))
                {
                    dict.Add("County", GetCounty(file));
                }

                if (!dict.ContainsKey("State"))
                {
                    dict.Add("State", "FL");
                }

                this.Pass(dict);

            }
            else
            {
                throw new InvalidCastException("Error: data is expected to be object[]");
            }
        }

        private StringDictionary ToDictionary(string[] colNames, string[] values)
        {
            var data = new StringDictionary();
            for (var j = 0; j < values.Length; j++)
            {
                var field = values[j];
                data.Add(colNames[j], field);
            }

            return data;
        }

        private string GetCounty(string file)
        {
            var fileName = Path.GetFileName(file).ToLower().Trim();
            foreach (var k in CountyAbbreviations.Instance.Keys)
            {
                var t = k.ToLower().Trim();
                if (fileName.StartsWith(t))
                {
                    return CountyAbbreviations.Instance[k];
                }
            }
            throw new InvalidOperationException("Error: Could not find county that maps to file name '" + fileName + "'");
        }
    }
}
