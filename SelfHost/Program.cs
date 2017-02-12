using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using System.Web.Http;

namespace Hack.HouseFlipper.WebSite.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string baseAddress = "http://localhost:9090";

            var configuration = new HttpSelfHostConfiguration(baseAddress);

            configuration.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });           

            using (HttpSelfHostServer server = new HttpSelfHostServer(configuration))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Service started ... Press Enter to quit.");
                Console.ReadLine();
            }
        }
    }
}
