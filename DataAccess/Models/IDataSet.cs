using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Models
{
    public interface IDataSet
    {
        object Selected { get; }
        object Current { get; }

        bool Peek();
        object Next();
        void Seek(int v);
        void Select();
        void MoveMext();
    }
}
