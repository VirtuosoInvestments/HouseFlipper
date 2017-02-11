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
            Console.WriteLine("---------------------");
            Console.WriteLine("HouseFlipper Console ");
            Console.WriteLine("---------------------");
            Console.WriteLine("Welcome {0}\\{1}!\n", Environment.UserDomainName, Environment.UserName);
            Console.WriteLine("Choose an option from menu:");
            Console.WriteLine("1) Load data");
            Console.WriteLine("2) Query flips");
            Console.WriteLine("3) Query active");
            Console.WriteLine("4) Clear database");
            Console.Read();
        }
    }
}
