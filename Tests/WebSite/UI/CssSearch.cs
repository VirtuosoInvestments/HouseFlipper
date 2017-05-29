using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.HouseFlipper.WebSite
{
    public class CssSearch
    {
        private StringDictionary attributes = new StringDictionary();
        public string TagName { get; set; }
        public string Text { get; set; }
        public void Add(string attribute, string value)
        {
            attributes.Add(attribute, value);
        }

        public override string ToString()
        {
            var result = string.Format("{0}[", TagName);
            bool isFirst = true;
            if (!string.IsNullOrEmpty(Text)) { attributes.Add("text", Text); }
            foreach(var a in attributes.Keys)
            {
                if(isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    result += ",";
                }
                result += string.Format("{0}='{1}'", a, attributes[a]);
            }
            result += "]";
            return result;
        }
    }
}
