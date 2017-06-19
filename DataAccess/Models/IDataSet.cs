using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Models
{
    public interface IDataSet
    {
        bool Peek();
        object Next();
        void Seek(int v);
    }
}
