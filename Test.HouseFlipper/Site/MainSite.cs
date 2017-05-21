using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.HouseFlipper
{
    public abstract class MainSite
    {
        public virtual string Url {get;}
        public virtual IWebDriver Driver { get; }

        public void GoToSite()
        {
            Driver.Navigate().GoToUrl(this.Url);
        }
    }
}
