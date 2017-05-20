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
        private const string siteName = "HouseFlipper";
        private const string siteDir = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\WebSite";
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            WebSiteSetup.NewApplication(siteName, "", siteDir);
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
