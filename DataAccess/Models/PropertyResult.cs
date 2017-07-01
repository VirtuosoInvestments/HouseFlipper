using HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Models
{
    public class PropertyResult
    {
        public PropertyResult(Guid searchId)
        {
            this.Flips = new List<Flip>();
            this.SearchId = searchId;
        }
        public Guid PropertyId { get; set; }
        public Guid SearchId { get; set; }
        public List<Flip> Flips { get; set; }

        // Can get it via PropertyId
        public MlsStatus Status { get; set; }

        public PropertyReference Reference { get; set; }
    }
}
