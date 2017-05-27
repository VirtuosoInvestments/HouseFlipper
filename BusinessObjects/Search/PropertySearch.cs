using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HouseFlipper.DataAccess.Models;

namespace HouseFlipper.BusinessObjects
{
    public class PropertySearch
    {
        public Guid Id { get; set; }
        public List<Property> Search(PropertySearchOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
