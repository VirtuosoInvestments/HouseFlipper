using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace Test.HouseFlipper
{
    [TestClass]
    public class SearchViewTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            WebSiteSetup.NewApplication();
        }        

        [TestMethod]
        public void Load()
        {
            var siteUrl = Settings.Get(Setting.SiteUrl);
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl(siteUrl);
            string title = string.Empty;
            Assert.AreEqual(title, driver.Title);
            var input = string.Empty;
            driver.FindElement(By.Id(input));
        }

        public void Results() { }
        public void Map() { }
        
    }
}
