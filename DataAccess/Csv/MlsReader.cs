using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.DataAccess.Csv
{
    public class MlsReader
    {
        private string[] files;

        public MlsReader(string dataFolder, string filesSearchPattern, SearchOption searchOption)
        {  
            if(string.IsNullOrWhiteSpace(dataFolder))
            {
                throw new ArgumentException(
                    "Error: dataFolder cannot be null, empty or blank");
            }
            if(!Directory.Exists(dataFolder))
            {
                throw new ArgumentException(
                    string.Format("Error: Directory path does not exist: '{0}'!", dataFolder));
            }

            if(string.IsNullOrWhiteSpace(filesSearchPattern))
            {
                throw new ArgumentException(
                    "Error: filesSearchPattern cannot be null, empty or blank");
            }

            files = Directory.GetFiles(dataFolder, filesSearchPattern, searchOption);
        }

        public MlsReader(string filePath)
        {            
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException(
                    "Error: filePath cannot be null, empty or blank");
            }
            if(!File.Exists(filePath))
            {
                throw new ArgumentException(
                    string.Format("Error: File path does not exist: '{0}'!", filePath));
            }
            files = new string[] { filePath };
        }

        public virtual IEnumerable<MlsRow> ReadLine()
        {
            foreach (var file in files)
            {
                var newFile = true;
                using (var sr = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.ReadWrite)))
                {
                    string line;                    
                    while ((line = sr.ReadLine()) != null)
                    {
                        yield return new MlsRow(line, newFile);
                        newFile = false;
                    }
                }
            }
        }
    }    
}
