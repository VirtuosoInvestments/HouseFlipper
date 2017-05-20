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
        private const string defaultPool = "DefaultAppPool";
        public static void NewSite(string siteName, string siteDir)
        {
            var iisMgr = new ServerManager();
            RemoveSite(iisMgr, siteName);
            AddSite(iisMgr, siteName, siteDir);
        }

        private static void AddSite(ServerManager iisMgr, string siteName, string siteDir)
        {
            Console.WriteLine("Adding new IIS site: {0} {1}", siteName, siteDir);
            var newSite = iisMgr.Sites.Add(siteName, siteDir, 2020);
            //newSite.ApplicationDefaults.ApplicationPoolName = defaultPool;
            newSite.TraceFailedRequestsLogging.Enabled = true;
            //newSite.TraceFailedRequestsLogging.Directory = "C:\\inetpub\\customfolder\\site";
            newSite.ServerAutoStart = true;
            AddReadACL("NetworkService", siteDir);
            iisMgr.CommitChanges();
        }

        private static void RemoveSite(ServerManager iisMgr, string siteName)
        {
            foreach (var site in iisMgr.Sites)
            {
                var existingSite = site.ToString();
                if (site.ToString().Equals(siteName))
                {
                    Console.WriteLine("Removing old IIS app: {0}", existingSite);
                    iisMgr.Sites.Remove(site);
                    iisMgr.CommitChanges();
                    break;
                }
            }
        }

        public static void NewApplication(string siteName, string appName, string appDir)
        {
            AddReadACL("NetworkService", appDir);
            var iisMgr = new ServerManager();
            string appname = siteName + appName;
            RemoveApplication(iisMgr, siteName, appname);
            AddApplication(iisMgr, siteName, appName, appDir);
        }

        private static void AddApplication(ServerManager iisMgr, string siteName, string appName, string appDir)
        {
            Console.WriteLine("Adding new IIS app: {0} {1}", appName, appDir);
            var newapp = iisMgr.Sites[siteName].Applications.Add(appName, appDir);
            newapp.ApplicationPoolName = siteName;
            iisMgr.CommitChanges();
        }

        private static void RemoveApplication(ServerManager iisMgr, string siteName, string appname)
        {
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
