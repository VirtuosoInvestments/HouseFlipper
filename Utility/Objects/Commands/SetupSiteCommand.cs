using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Commands
{
    public class SetupSiteCommand : ICommand
    {
        public virtual string Description
        {
            get { return "Sets up House Flipper website"; }
        }

        public string Example
        {
            get { return string.Empty; }
        }

        public virtual string Format
        {
            get
            {
                return "<port> <siteName> <siteDirPath>";
            }
        }

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
