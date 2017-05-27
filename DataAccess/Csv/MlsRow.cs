using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Csv
{
    public class MlsRow
    {
        public MlsRow(string text, bool newFile)
        {
            this.Text = text;
            this.IsHeader = newFile;
        }
        public string Text { get; set; }
        public bool IsHeader { get; set; }
    }
}
