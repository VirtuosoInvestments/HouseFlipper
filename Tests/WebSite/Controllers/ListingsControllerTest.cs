using Hack.HouseFlipper.DataAccess.DB.Fakes;
using Hack.HouseFlipper.DataAccess.Models;
using Microsoft.QualityTools.Testing.Fakes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Fakes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSite.Controllers;

namespace Tests
{
    [TestFixture]
    public class ListingsControllerTest
    {
        [Test]
        public void IndexSortNull()
        {
            using (ShimsContext.Create())
            {
                
                var stub = new StubDbSet<MlsRow>();
                ShimMlsContext.AllInstances.ListingsGet =
                    (context) =>
                    {
                        return stub;
                    };
                var target = new ListingsController();
                target.Index(null, null, null, null, null, null);
            }
        }
    }
}
