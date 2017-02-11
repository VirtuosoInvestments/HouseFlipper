using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args==null || args.Length==0)
            {
                Help();
            }
            else
            {
                CmdHandler(args);
            }
            Console.Read();
        }

        private static void CmdHandler(string[] args)
        {
            throw new NotImplementedException();
        }

        private static void Help()
        {
            Console.WriteLine("Help - coming soon!");
        }
    }
}
