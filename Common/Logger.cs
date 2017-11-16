using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Common
{
    public static class Logger
    {
        private static bool debugEnabled = GetConfig("Debug",true);
        private static bool errorEnabled = GetConfig("Error", true);

        public static void Debug(string message,params object[] args)
        {
            if(debugEnabled)
            {
                var tag = "DEBUG";
                HandleWrite(message, args, tag);
            }
        }

        public static void Error(string message, params object[] args)
        {
            if (errorEnabled)
            {
                var tag = "ERROR";
                HandleWrite(message, args, tag);
            }
        }

        private static void HandleWrite(string message, object[] args, string tag)
        {
            var msg = string.Format(message, args);
            Console.WriteLine("{0}: {1}", tag, msg);
        }


        private static bool GetConfig(string name, bool defaultValue)
        {
            var val = ConfigurationManager.AppSettings[name];
            if(!string.IsNullOrWhiteSpace(val))
            {
                return bool.Parse(val);
            }
            return defaultValue;
        }
    }
}
