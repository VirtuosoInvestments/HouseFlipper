using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.HouseFlipper
{
    public abstract class MainSite : IDisposable
    {
        public virtual string Url {get;}
        private static Lazy<IWebDriver> driver = new Lazy<IWebDriver>(
           () =>
           {
               var instance = new ChromeDriver(@"C:\docusign_source\Core\External\Selenium\ChromeDriver");
               return instance;
           }
           );
        public IWebDriver Driver
        {
            get
            {
                return driver.Value;
            }
        }

        public void GoToSite()
        {
            Driver.Navigate().GoToUrl(this.Url);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && driver!=null)
            {
                driver.Value.Close();
                driver = null;
            }
        }

        ~MainSite()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
