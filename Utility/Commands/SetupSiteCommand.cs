using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility
{
    public class SetupSiteCommand : ICommand
    {
        public virtual void Execute(params string[] args)
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
