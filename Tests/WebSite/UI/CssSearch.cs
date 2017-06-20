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


            
            // FIRST WAY
            var result = string.Format("{0}[", TagName);
            string expr = string.Empty;
            bool isFirst = true;
            foreach (string a in attributes.Keys)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    expr += ",";
                }
                expr += string.Format("{0}='{1}'", a, attributes[a]);
            }


            var sep = string.Empty;
            if (!string.IsNullOrWhiteSpace(expr))
            {
                sep = ",";
            }

            if (!string.IsNullOrEmpty(this.Text))
            {
                expr += string.Format("text()='{0}'", Text);
            }

            result += expr + "]";
            return result;

            /*
            string result = string.Empty;
            // SECOND WAY
            if (attributes.Keys.Count > 0)
            {
                result = string.Format(".{0}[", TagName);
                string expr = string.Empty;
                bool isFirst = true;
                foreach (string a in attributes.Keys)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        expr += ",";
                    }
                    expr += string.Format("{0}='{1}'", a, attributes[a]);
                }
            }
            else if (!string.IsNullOrEmpty(this.Text))
            {
                result = string.Format(".{0}:contains('{1}')", TagName, Text);                                
            }
            return result;*/
        }
    }
}
