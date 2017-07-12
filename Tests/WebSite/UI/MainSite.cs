using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.HouseFlipper.Common;

namespace Test.HouseFlipper.WebSite
{
    public abstract class MainSite //: IDisposable
    {
        public virtual string Url {get;}

        private IWebDriver driver = null;
        protected IWebDriver Driver
        {
            get
            {
                //return driver.Value;
                if(driver==null)
                {
                    var driverPath = Settings.Get(Setting.ChromeDriver);
                    driver = new ChromeDriver(driverPath);
                    WebElement.Driver = driver;
                }
                return driver;
            }
        }

        public string Title
        {
            get
            {
                return Driver.Title;
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
                driver.Close();
                driver = null;
            }
        }
        /*
        ~MainSite()
        {
            Dispose(true);
        }*/


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
