using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dispatcher;
using WebSite.Controllers;

namespace Hack.HouseFlipper.WebSite.Host
{
    /* Loads the assemblies required for hosting the House Flipper site
     */
    public class ControllersLoader : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            ICollection<Assembly> defaultAssemblies = base.GetAssemblies();
            List<Assembly> assemblies = new List<Assembly>(defaultAssemblies);
            Type t = typeof(ListingsController);
            Assembly a = t.Assembly;
            defaultAssemblies.Add(a);
            return assemblies;
        }
    }
}
