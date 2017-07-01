using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using NUnit.Framework;

namespace Utility
{
    [TestFixture]
    public class UtilityTest
    {
        [Test]
        [Category("Incomplete")]
        public void Import()
        {
            var exe = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Utility\bin\Debug\util.exe";
            var args = @"-import ""E:\DocuSign\Backup\Laptop\My Documents\Visual Studio 2015\Projects\HouseFlipper\WebSite\Data\Listings""";

            var p = Process.Start(exe, args);
            throw new NotImplementedException();
        }
    }
}
