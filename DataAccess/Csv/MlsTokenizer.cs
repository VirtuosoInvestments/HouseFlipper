using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Csv
{
    // This tokenizer class is a specialized tokenizer that expects
    // each line of the MLS.csv file to be of the format
    // "___","____"
    // where there is a quote-comma-quote delimter/separator separating
    // internal fields
    // So we do 1 pass to split the line using quote-comma-quote delimeter
    // Then we iterate and remove a quote from the 1st and last tokens
    // since they are not internal.
    public static class MlsTokenizer
    {
        public static string[] Split(string line)
        {
            return StripExtraQuotes(GetFields(line));
        }
        private static string[] GetFields(string line)
        {
            var separator = "\",\"";
            return line.Split(new string[] { separator }, StringSplitOptions.None);
        }

        private static string[] StripExtraQuotes(string[] fields)
        {
            var colNames = new string[fields.Length];
            for (var j = 0; j < fields.Length; j++)
            {
                var field = fields[j].Replace("\"", string.Empty);
                colNames[j] = field;
            }
            return colNames;
        }
    }
}
