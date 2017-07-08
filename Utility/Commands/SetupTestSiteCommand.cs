using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Commands
{
    public class SetupTestSiteCommand : SetupSiteCommand
    {
        private static int port = 8020;
        private static string siteName = "HouseFlipperTest";
        private static string siteDir = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\WebSite";
        public override void Execute(string[] args)
        {
            base.Execute(port.ToString(),siteName,siteDir);
        }
    }
}
