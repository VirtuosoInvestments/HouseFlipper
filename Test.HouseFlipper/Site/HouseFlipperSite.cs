using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Test.HouseFlipper
{
    public enum SiteNavigation { Search }

    public class HouseFlipperSite : MainSite
    {
        private static int port = 8020;
        private static string siteName = "HouseFlipperTest";
        private static string siteDir = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\WebSite";

        private static Lazy<string> siteUrl = new Lazy<string>(
            ()=>
            {                
                return WebSiteSetup.NewSite(siteName, siteDir, port);
            }
            );

        public override string Url { get { return siteUrl.Value; } }

        private static Lazy<IWebDriver> driver = new Lazy<IWebDriver>(
            () => 
            {
                var instance = new ChromeDriver(@"C:\docusign_source\Core\External\Selenium\ChromeDriver");
                /*instance.Navigate().GoToUrl(site.Url);
                string title = string.Empty;
                Assert.AreEqual(title, instance.Title);
                var input = string.Empty;
                instance.FindElement(By.Id(input));*/
                return instance;
            }
            );
        public override IWebDriver Driver
        {
            get
            {
                return driver.Value;
            }
        }

        public View GoTo(SiteNavigation navigation)
        {
            switch(navigation)
            {
                case SiteNavigation.Search:
                    var view = new SearchView(this);
                    view.GoToView();
                    return view;
                default:
                    throw new InvalidOperationException("Unhandled navigation: " + navigation);
            }
        }

        public void Remove()
        {
            WebSiteSetup.RemoveSite(siteName);
        }
    }
}
