using System;
using System.Diagnostics;
using NUnit.Framework;
using HouseFlipper.DataAccess.DB;
using System.Linq;
using System.Data.Entity;

namespace Utility
{
    [TestFixture]
    public class UtilityTest
    {
        [Test]
        [Category("Incomplete")]
        public void Import()
        {
            // TODO: Delete DB: either drop tables, or use sqlexec.cmd to delete MLS database or set app.config to point to a TestMls DB that can be dropped
            /*using (var context = new MlsContext())
            {
                context.Database.Delete();
                context.SaveChanges();
            }*/

            Database.Delete("MlsContext");

            var exe = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Utility\bin\Debug\util.exe";
            var args = @"-import ""E:\DocuSign\Backup\Laptop\My Documents\Visual Studio 2015\Projects\HouseFlipper\WebSite\Data\Listings""";

            var p = Process.Start(exe, args);
            p.WaitForExit();

            using (var context = new MlsContext())
            {
                var count = (from i in context.Listings
                 select i).Count();
                Assert.IsTrue(count > 0);
                Assert.AreEqual(26587,count);

                var count2 = (from i in context.Properties
                             select i).Count();
                Assert.IsTrue(count2 > 0);

                var count3 = (from i in context.PropertyListings
                              select i).Count();
                Assert.IsTrue(count3 > 0);
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
