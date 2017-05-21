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
        public static string NewSite(string siteName, string siteDir, int port)
        {
            var iisMgr = new ServerManager();
            RemoveSite(iisMgr, siteName);
            var siteUrl = AddSite(iisMgr, siteName, siteDir, port);
            return siteUrl;
        }

        private static string AddSite(ServerManager serverManager, string siteName, string siteDir, int port)
        {            
            Console.WriteLine("Adding new IIS site: {0} {1}", siteName, siteDir);
            
            var newSite = serverManager.Sites.Add(siteName, siteDir, port);            
            newSite.TraceFailedRequestsLogging.Enabled = true;            
            newSite.ServerAutoStart = true;
            AddReadACL("NetworkService", siteDir);
            RemoveAppPool(serverManager, siteName);
            var appPoool = CreateAppPool(serverManager, siteName);
            serverManager.CommitChanges();
            appPoool.Recycle();
            return string.Format("http://{0}:{1}", "localhost", port);
        }

        public static void RemoveSite(string siteName)
        {
            var iisMgr = new ServerManager();
            RemoveSite(iisMgr, siteName);
            GetAppPoolName(siteName);
            RemoveAppPool(iisMgr, siteName);
        }

        private static void RemoveAppPool(ServerManager serverManager, string siteName)
        {
            string poolName = GetAppPoolName(siteName);
            foreach(var appPool in serverManager.ApplicationPools)
            {
                if(appPool.Name.Equals(poolName))
                {
                    serverManager.ApplicationPools.Remove(appPool);
                    Console.WriteLine("Removed existing app pool: {0}", poolName);
                    break;
                }
            }
        }

        private static ApplicationPool CreateAppPool(ServerManager serverManager, string siteName)
        {
            string poolName = GetAppPoolName(siteName);
            serverManager.ApplicationPools.Add(poolName);
            serverManager.Sites[siteName].Applications[0].ApplicationPoolName = poolName;
            ApplicationPool appPool = serverManager.ApplicationPools[poolName];
            appPool.ProcessModel.IdentityType = ProcessModelIdentityType.NetworkService;
            appPool.ProcessModel.LoadUserProfile = false;
            appPool.ProcessModel.PingingEnabled = false;
            appPool.Failure.RapidFailProtection = false;
            appPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
            appPool.ManagedRuntimeVersion = "v4.0.30319";
            return appPool;
        }

        private static string GetAppPoolName(string siteName)
        {
            return siteName.Replace(" ", string.Empty) + "Pool";
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
