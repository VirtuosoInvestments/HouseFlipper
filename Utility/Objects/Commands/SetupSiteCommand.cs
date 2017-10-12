using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Commands
{
    public class SetupSiteCommand : Command
    {
        public override string Description
        {
            get { return "Sets up House Flipper website"; }
        }

        public override string Example
        {
            get { return string.Empty; }
        }

        public override string Format
        {
            get
            {
                return "<port> <siteName> <siteDirPath>";
            }
        }

        public override void Execute(params string[] args)
        {
            int port;
            string siteName, siteDir;
            port = int.Parse(args[0]);
            siteName = args[1];
            siteDir = args[2];
            WebSiteSetup.NewSite(siteName, siteDir, port);
        }
    }
}
