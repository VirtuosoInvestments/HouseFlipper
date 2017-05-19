using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.DataAccess.DB
{
    public class MlsContext : DbContext
    {

        public MlsContext()
            : base("MlsContext")
        {
        }

        public virtual DbSet<Listing> Listings { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //}
    }
}
