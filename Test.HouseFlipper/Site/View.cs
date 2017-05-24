using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.HouseFlipper
{
    public abstract class View
    {
        private MainSite site;
        protected IWebDriver Driver { get { return site.Driver; } }
        public View(MainSite mainSite)
        {
            this.site = mainSite;
        }

        public abstract void VerifyForm();
    }
}
