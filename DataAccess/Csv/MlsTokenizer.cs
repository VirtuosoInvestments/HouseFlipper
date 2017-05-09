using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.DataAccess.Csv
{
    public class MlsTokenizer
    {
        
        public List<string> GetValues(string line)
        {
            // This tokenizer class is a specialized tokenizer that expects
            // each line of the MLS.csv file to be of the format
            // "___","____"
            // where there is a quote-comma-quote delimter separating
            // internal fields
            // So we do 1 pass to split the line using quote-comma-quote delimeter
            // Then we iterate and remove a quote from the 1st and last tokens
            // since they are not internal.
            var result = new List<string>();
            var fields = GetFields(line);
            StripQuotes(result, fields);
            return result;
        }

        private static void StripQuotes(List<string> result, string[] fields)
        {
            for (var j = 0; j < fields.Length; j++)
            {
                var field = fields[j].Replace("\"", string.Empty);
                result.Add(field);
            }
        }

        private static string[] GetFields(string line)
        {
            return line.Split(new string[] { "\",\"" }, StringSplitOptions.None);
        }
    }
}
