using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility
{
    public class MainProgram
    {
        public static void Main(string[] args)
        {
            var version = GetVersion();
            if (version == "v2")
            {
                Program.Execute(args);
            }
            else
            {
                throw new InvalidOperationException("Unsupported version: " + version);
            }
        }
        private static string GetVersion()
        {
            return ConfigurationManager.AppSettings["Version"].ToLower();
        }
    }
}