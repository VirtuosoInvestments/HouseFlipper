using HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Models
{
    public class Listings : List<Listing>, IDataSet
    {
        int current = 0;

        public object Selected
        {
            get
            {
                var selected = new Listings();
                foreach (var i in chosen)
                {
                    selected.Add(this[i]);
                }
                return selected;
            }
        }

        public object Current
        {
            get
            {
                return this[current];
            }
        }

        public object Next()
        {
            return this[current++];
        }

        public bool Peek()
        {
            return current < this.Count;
        }

        public void Seek(int v)
        {
            current = v;
        }

        private List<int> chosen = new List<int>();
        public void Select()
        {
            chosen.Add(current);
        }

        public void MoveMext()
        {
            ++current;
        }
    }
}
