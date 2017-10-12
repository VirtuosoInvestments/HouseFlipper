using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.Utility.Objects.Commands
{
    public static class CommandFactory
    {
        private static Dictionary<string, Type> cmdLookup = new Dictionary<string, Type>();
        static CommandFactory()
        {
            foreach (var t in typeof(CommandFactory).Assembly.GetTypes())
            {
                if (!t.IsInterface && t.GetInterface("ICommand") != null && !t.IsAbstract)
                {
                    var className = t.Name.ToLower();
                    var end = className.IndexOf("command");
                    if (end < 1)
                    {
                        throw new InvalidOperationException("Error: Could not derive command alias from type: " + className);
                    }
                    var cmdAlias = className.Substring(0, end);
                    cmdLookup.Add(cmdAlias, t);
                }
            }
        }
        /*public static ICommand Create(string cmdName)
        {
            ICommand cmd = null;
            switch (cmdName)
            {
                case "import":
                    cmd = new ImportCommand();
                    break;
                case "clear":
                    cmd = new ClearCommand();
                    break;
                case "delete":
                    cmd = new DeleteCommand();
                    break;
                case "count":
                    cmd = new CountCommand();
                    break;
                case "setupsite":
                    cmd = new SetupSiteCommand();
                    break;
                case "setuptestsite":
                    cmd = new SetupTestSiteCommand();
                    break;
                default:
                    throw new InvalidOperationException("Unknown command " + cmdName);
            }
            return cmd;
        }*/

        public static ICommand Create(string cmdName)
        {
            var lookup = cmdName.ToLower().Trim();
            ICommand inst = null;
            if (cmdLookup.ContainsKey(lookup))
            {
                var t = cmdLookup[lookup];
                inst = (ICommand)Activator.CreateInstance(t);

            }
            else
            {
                throw new InvalidOperationException("Error: Unknown command " + cmdName);
            }
            return inst;
        }
    }
}
