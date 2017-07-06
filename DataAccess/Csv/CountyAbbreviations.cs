using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Csv
{
    public class CountyAbbreviations : Dictionary<string, string>
    {
        private static Dictionary<string, string> map = new Dictionary<string, string>()
        {
            { "oc", "Orange" },
            {"seminole","Seminole" },
            { "polk","Polk"},
            {"osceola","Osceola" }
        };

        private CountyAbbreviations()
        {
            foreach(var k in map.Keys)
            {                
                this.Add(k, map[k]);
            }
        }

        private static CountyAbbreviations _instance;
        public static CountyAbbreviations Instance
        {
            get
            {
                if(_instance==null)
                {
                    _instance = new CountyAbbreviations();
                }
                return _instance;
            }
        }
    }
}
