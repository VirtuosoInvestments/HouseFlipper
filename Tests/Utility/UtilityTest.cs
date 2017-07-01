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
        [Test]
        public void Import()
        {
            Database.Delete("MlsContext");

            var exe = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Utility\bin\Debug\util.exe";
            var appconfig = exe + ".config";
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

            try
            {
                var args = @"-import ""E:\DocuSign\Backup\Laptop\My Documents\Visual Studio 2015\Projects\HouseFlipper\WebSite\Data\Listings""";

                var p = Process.Start(exe, args);
                p.WaitForExit((int)TimeSpan.FromMinutes(5).TotalMilliseconds);

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
            finally
            {
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

        [Test]
        [Category("NotImplemented")]
        public void ImportParallel()
        {
            // TODO: Delete DB
            throw new NotImplementedException();
        }
    }
}
