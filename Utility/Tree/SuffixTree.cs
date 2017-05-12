using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility
{
    public class SuffixTree<T>
    {
        private SuffixTreeNode<T> root;
    }

    public class SuffixTreeNode<T>
    {
        public string Key { get; set; }
        public T Data { get; set; }
    }
}
