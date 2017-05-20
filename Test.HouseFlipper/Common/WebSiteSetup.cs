using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using System.IO;

namespace Test.HouseFlipper
{
    public class WebSiteSetup
    {
        public static void NewApplication(string siteName, string appName, string siteDir)
        {
            AddReadACL("NetworkService", siteDir);
            var iisMgr = new ServerManager();
            string appname = siteName + appName;
            foreach (var app in iisMgr.Sites[siteName].Applications)
            {
                var existingApp = app.ToString();
                if (app.ToString().Equals(appname))
                {
                    Console.WriteLine("Removing old IIS app: {0}", existingApp);
                    iisMgr.Sites[siteName].Applications.Remove(app);
                    iisMgr.CommitChanges();
                    break;
                }
            }
            Console.WriteLine("Adding new IIS app: {0} {1}", appName, siteDir);
            var newapp = iisMgr.Sites[siteName].Applications.Add(appName, siteDir);
            newapp.ApplicationPoolName = siteName;
            iisMgr.CommitChanges();
        }

        private static void AddReadACL(string account, string dirname)
        {
            var rights = FileSystemRights.ReadAndExecute;
            var inheritanceFlag = InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit;
            var propagationFlag = PropagationFlags.InheritOnly;
            var allow = AccessControlType.Allow;
            var user = new NTAccount(account);
            var ace = new FileSystemAccessRule(user, rights, inheritanceFlag, propagationFlag, allow);
            var dirInfo = new DirectoryInfo(dirname);
            var acl = dirInfo.GetAccessControl();
            acl.AddAccessRule(ace);
            dirInfo.SetAccessControl(acl);
        }
    }
}
