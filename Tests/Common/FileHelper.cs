using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.HouseFlipper.Common
{
    public class FileHelper
    {
        public static List<List<string>> GetFiles(string path)
        {
            var files = Directory.GetFiles(path);
            var all = new List<List<string>>();
            foreach (var file in files)
            {
                List<string> lines = GetLines(file);
                all.Add(lines);
            }

            return all;
        }

        public static List<string> GetLines(string path)
        {
            List<string> lines = new List<string>();
            using (var sr = new StreamReader(path))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines;
        }
    }
}
