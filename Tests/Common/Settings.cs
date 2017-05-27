using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.HouseFlipper.Common
{
    public enum Setting
    {
        SiteUrl,
        ChromeDriver
    }

    public class Settings
    {
        public static string Get(Setting setting)
        {
            return ConfigurationManager.AppSettings[setting.ToString()];
        }
    }
}
