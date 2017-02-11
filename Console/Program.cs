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
                try
                {
                    CmdHandler(args);
                }
                catch(UnrecognizedCommandException uc)
                {
                    Console.WriteLine(uc.Message);
                    Console.WriteLine();
                    Help();
                }
                catch(UnhandledCommandException uc)
                {
                    Console.WriteLine(uc.Message);
                }
            }
            Console.Read();
        }

        private static void CmdHandler(string[] args)
        {
            var cmdStr = args[0];
            Command command;
            if(!Enum.TryParse(cmdStr, true, out command))
            {
                throw new UnrecognizedCommandException(cmdStr);
            }

            switch(command)
            {
                case Command.Load:                    
                default:
                    throw new UnhandledCommandException(cmdStr);                    
            }
        }

        private static void Help()
        {            
            Console.WriteLine(string.Format(
@"{0} [options]",System.Reflection.Assembly.GetExecutingAssembly().GetName().Name) + 
@"

options:

load     [directory <directory path> | files <list of file paths comma separated>]

");
        }
    }

    enum Command
    {
        Load
    }

    class UnrecognizedCommandException : Exception
    {
        public UnrecognizedCommandException(string cmd) : base("Unrecognized command: " + cmd)
        {
        }
    }

    class UnhandledCommandException : Exception
    {
        public UnhandledCommandException(string cmd) : base("No handler has been written for command: " + cmd)
        {
        }
    }
}
