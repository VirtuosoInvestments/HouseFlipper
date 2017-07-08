using System;
using System.Diagnostics;
using NUnit.Framework;
using HouseFlipper.DataAccess.DB;
using System.Linq;
using System.Data.Entity;
using System.IO;
using HouseFlipper.DataAccess.Csv;
using System.Collections;

namespace Utility
{
    [TestFixture]
    public class UtilityTest
    {
        private const string SKIP_SETUP = "SkipSetup";
        private const string utilExe = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Utility\bin\Debug\util.exe";
        private readonly string appconfig = utilExe + ".config";
        private const int _defaultTimeoutMins = 10;

        [SetUp]
        public void OneTimeSetup()
        {
            if (CheckForSkipSetup())
            {
                if (!Database.Exists("MlsContext"))
                {
                    ImportTestData();
                }
            }
            else
            {
                ImportTestData();
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            UndoAppConfig();
        }

        [Test]
        public void Import()
        {
            using (var context = new MlsContext())
            {
                var count = (from i in context.Listings
                             select i).Count();
                Assert.IsTrue(count > 0);
                Assert.AreEqual(26587, count, "Error: Actual listings count in database incorrect!");
            }
        }

        [Test]
        [Category(SKIP_SETUP)]
        [Category("Regression")]
        public void Properties()
        {
            using (var context = new MlsContext())
            {
                var count2 = (from i in context.Properties
                              select i).Count();
                Assert.IsTrue(count2 > 0, "Error: No properties found in database!");
            }
        }

        [Test]
        [Category(SKIP_SETUP)]
        [Category("Regression")]
        public void PropertyListings()
        {
            using (var context = new MlsContext())
            {
                var count3 = (from i in context.PropertyListings
                              select i).Count();
                Assert.IsTrue(count3 > 0, "Error: No property listings found in database!");
            }
        }

        [Test]
        [Category(SKIP_SETUP)]
        [Category("Regression")]
        public void ImportParallel()
        {
            Database.Delete("MlsContext");
            /*FixAppConfig();

            var args = @"-import parallel ""E:\DocuSign\Backup\Laptop\My Documents\Visual Studio 2015\Projects\HouseFlipper\WebSite\Data\Listings""";
            var timeout = (int)TimeSpan.FromMinutes(timeoutMins).TotalMilliseconds;
            Process p = null;
            try
            {
                p=Run(args, timeout);
            }
            catch(Exception)
            {
                if (Database.Exists("MlsContext"))
                {
                    Database.Delete("MlsContext");
                }
                throw;
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
            }*/
            ImportTestData(TimeSpan.FromMinutes(10),true, false);
            Import();
        }

        [Test]
        [Category(SKIP_SETUP)]
        public void ImportBulk()
        {
            Database.Delete("MlsContext");
            ImportTestData(TimeSpan.FromMinutes(1),true, true);
            Import();
        }

        [Test]
        [Category(SKIP_SETUP)]
        [Category("Regression")]
        public void NoDuplicates()
        {
            using (var context = new MlsContext())
            {
                var count = (from i in context.Listings
                             select i).Count();

                var uniqueCount = (from i in context.Listings
                                   select i.MLNumber).Distinct().Count();

                Assert.AreEqual(uniqueCount, count);
            }
        }

        [Test]
        [Category(SKIP_SETUP)]
        public void ListingId()
        {
            using (var context = new MlsContext())
            {
                var newListing = new Listing();
                newListing.MLNumber = "foo" + DateTime.Now.ToString();
                context.Listings.Add(newListing);
                var id = newListing.Id;
                context.SaveChanges();
                try
                {
                    Assert.AreNotEqual(id, newListing.Id);
                }
                finally
                {
                    context.Listings.Remove(newListing);
                    context.SaveChanges();
                }
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

        [Test]
        [Category(SKIP_SETUP)]
        public void County()
        {
            using (var context = new MlsContext())
            {
                var query = (from i in context.Listings
                             where i.County != null && i.County.Length > 0
                             select i.County);
                var count = query.Count();
                var allListingsCount = (from i in context.Listings
                                        select i).Count();
                Assert.IsTrue(count > 0);
                Assert.AreEqual(allListingsCount, count);
                var distinct = query.Distinct().ToList();
                Assert.AreEqual(CountyAbbreviations.Instance.Keys.Count, distinct.Count);
                foreach (var county in CountyAbbreviations.Instance.Values)
                {
                    Assert.IsTrue(distinct.Contains(county));
                }
            }
        }

        [Test]
        [Category(SKIP_SETUP)]
        public void State()
        {
            using (var context = new MlsContext())
            {
                var query = (from i in context.Listings
                             where i.State != null && i.State.Length > 0
                             select i.State);
                var count = query.Count();
                var allListingsCount = (from i in context.Listings
                                        select i).Count();
                Assert.IsTrue(count > 0);
                Assert.AreEqual(allListingsCount, count);
                var distinct = query.Distinct().ToList();
                Assert.AreEqual(1, distinct.Count);
                Assert.AreEqual("FL", distinct[0]);
            }
        }

        private Process Run(string args, int timeout)
        {
            DateTime now, expected, actual;
            Output("Process start time: {0}", now = DateTime.Now);
            Output("Expected end time: {0}", expected = now.AddMilliseconds(timeout));
            Process p = Process.Start(utilExe, args);
            var done = p.WaitForExit(timeout);
            Output("Wait end time: {0}", actual = DateTime.Now);
            Assert.IsTrue(done, "Error: Process took longer than expected to complete! Expected end time:{0}, Current time:{1}", expected, actual);
            return p;

            /*
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo();
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;

            p.StartInfo.FileName = utilExe;
            p.StartInfo.Arguments = args;  

            p.Start();
            var done = p.WaitForExit(timeout);
            var output = p.StandardOutput.ReadToEnd();
            var error = p.StandardError.ReadToEnd();
            ProcessOutput(output);
            ProcessOutput(error);
            Assert.IsTrue(done);
            return p;
            */
        }

        private void Output(string message, params object[] args)
        {
            Console.WriteLine(message, args);
            Debug.WriteLine(message, args);
        }

        private void Output()
        {
            Output("");
        }

        private void ProcessOutput(string programOutput)
        {
            if (!string.IsNullOrWhiteSpace(programOutput))
            {
                Output("Program output:");
                Output(programOutput);
                Output();
                programOutput = programOutput.ToLower();
                if (programOutput.Contains("error code 1"))
                {
                    Output("Execution failed:");
                    throw new InvalidOperationException(programOutput);
                }
            }
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

        private void ImportTestData(TimeSpan? timeSpan=null, bool parallel = false, bool bulk = false)
        {
            Database.Delete("MlsContext");
            FixAppConfig();
            var parallelStr = string.Empty;
            if (parallel)
            {
                parallelStr = "parallel ";
            }

            var bulkStr = string.Empty;
            if (bulk)
            {
                bulkStr = "bulk ";
            }
            var args = @"-import " + parallelStr + bulkStr + @"""E:\DocuSign\Backup\Laptop\My Documents\Visual Studio 2015\Projects\HouseFlipper\WebSite\Data\Listings""";
            int timeout = -1;
            if (timeSpan.HasValue)
            {
                timeout = (int)timeSpan.Value.TotalMilliseconds;
            }
            else
            {
                timeout = (int)TimeSpan.FromMinutes(_defaultTimeoutMins).TotalMilliseconds;
            }
            Process p = null;
            try
            {
                p = Run(args, timeout);
            }
            catch (Exception)
            {
                if (Database.Exists("MlsContext"))
                {
                    Database.Delete("MlsContext");
                }
                throw;
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

        private bool CheckForSkipSetup()
        {
            var categories = TestContext.CurrentContext.Test.Properties["Category"] as IList;
            bool skipSetup = categories != null && categories.Contains(SKIP_SETUP);
            return skipSetup;
        }
    }
}
