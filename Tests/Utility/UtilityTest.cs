using System;
using System.Diagnostics;
using NUnit.Framework;
using HouseFlipper.DataAccess.DB;
using System.Linq;
using System.Data.Entity;
using System.IO;

namespace Utility
{
    [TestFixture]
    public class UtilityTest
    {
        private const string utilExe = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Utility\bin\Debug\util.exe";
        private readonly string appconfig = utilExe + ".config";
        private const int timeoutMins = 8;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Database.Delete("MlsContext");
            FixAppConfig();

            var args = @"-import ""E:\DocuSign\Backup\Laptop\My Documents\Visual Studio 2015\Projects\HouseFlipper\WebSite\Data\Listings""";
            var timeout = (int)TimeSpan.FromMinutes(timeoutMins).TotalMilliseconds;
            Process p = null;
            try
            {
                Run(args, timeout, out p);
            }
            finally
            {
                if (p != null)
                {
                    if (!p.HasExited)
                    {
                        p.Kill();
                    }
                }
            }
        }
        
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            UndoAppConfig();
        }
        
        [Test]
        [Category("Regression")]
        public void Import()
        {
            using (var context = new MlsContext())
            {
                var count = (from i in context.Listings
                             select i).Count();
                Assert.IsTrue(count > 0);
                Assert.AreEqual(26587, count);

                var count2 = (from i in context.Properties
                              select i).Count();
                Assert.IsTrue(count2 > 0);

                var count3 = (from i in context.PropertyListings
                              select i).Count();
                Assert.IsTrue(count3 > 0);
            }
        }

        [Test]
        [Category("Regression")]
        public void ImportParallel()
        {
            Database.Delete("MlsContext");
            FixAppConfig();

            var args = @"-import parallel ""E:\DocuSign\Backup\Laptop\My Documents\Visual Studio 2015\Projects\HouseFlipper\WebSite\Data\Listings""";
            var timeout = (int)TimeSpan.FromMinutes(timeoutMins).TotalMilliseconds;
            Process p = null;
            try
            {
                Run(args, timeout, out p);
            }
            finally
            {
                if(p!=null)
                {
                    if(!p.HasExited)
                    {
                        p.Kill();
                    }
                }
            }
            Import();
        }

        [Test]
        public void NoDuplicates()
        {
            using (var context = new MlsContext())
            {
                var count = (from i in context.Listings
                             select i).Count();

                var count2 = (from i in context.Listings
                             select i.MLNumber).Distinct().Count();

                Assert.AreEqual(count, count2);
            }
        }

        [Test]
        public void ListingId()
        {
            using (var context = new MlsContext())
            {
                var newListing = new Listing();
                newListing.MLNumber = "foo" + DateTime.Now.ToString();
                context.Listings.Add(newListing);
                var id = newListing.Id;
                context.SaveChanges();
                Assert.AreNotEqual(id, newListing.Id);
            }
        }

        [Test]
        [Category("NotImplemented")]
        public void AddDates()
        {
            throw new NotImplementedException();
        }

        

        [Test]
        [Category("NotImplemented")]
        public void DeleteDbCmd()
        {
            //TODO: Be sure to ask user if they are sure they want to drop the database, but also provide an override -force flag to skip confirmationn or -confirm
            throw new NotImplementedException();
        }

        private static void Run(string args, int timeout, out Process p)
        {
            p = Process.Start(utilExe, args);
            var done = p.WaitForExit(timeout);
            Assert.IsTrue(done);
        }

        private void FixAppConfig()
        {
            var content = string.Empty;
            using (var sr = new StreamReader(appconfig))
            {
                content = sr.ReadToEnd();
            }
            content = content.Replace("Initial Catalog=MLS", "Initial Catalog=TestMLS");
            using (var sw = new StreamWriter(appconfig))
            {
                sw.Write(content);
                sw.Flush();
            }
        }

        private void UndoAppConfig()
        {
            var content = string.Empty;
            using (var sr = new StreamReader(appconfig))
            {
                content = sr.ReadToEnd();
            }
            content = content.Replace("Initial Catalog=TestMLS", "Initial Catalog=MLS");
            using (var sw = new StreamWriter(appconfig))
            {
                sw.Write(content);
                sw.Flush();
            }
        }
    }
}
