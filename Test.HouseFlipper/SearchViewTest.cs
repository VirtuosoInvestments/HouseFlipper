using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using NUnit.Framework;

namespace Test.HouseFlipper
{
    [TestFixture]
    public class SearchViewTest
    {
        /*private static int port = 8020;
        private static string siteName = "HouseFlipper";
        private static string siteDir = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\WebSite";
        private static string siteUrl;*/
        private static HouseFlipperSite site;// = new HouseFlipperSite();

        [OneTimeSetUp]
        public static void Setup()
        {
            //siteUrl = WebSiteSetup.NewSite(siteName, siteDir,port);
            site = new HouseFlipperSite();
        }

        [OneTimeTearDown]
        public static void TearDown()
        {
            site.Driver.Close();
        }

        [Test]
        public void Visit()
        {
            //var siteUrl = Settings.Get(Setting.SiteUrl);
            //var driver = new ChromeDriver(@"C:\docusign_source\Core\External\Selenium\ChromeDriver");
            //driver.Navigate().GoToUrl(site.Url);
            //string title = string.Empty;
            //Assert.AreEqual(title, driver.Title);
            //var input = string.Empty;
            //driver.FindElement(By.Id(input));
            var searchView = site.GoTo(SiteNavigation.Search);
            Assert.IsNotNull(searchView);
        }

        [Test]
        public void PerformSearch()
        {
            var searchView = site.GoTo(SiteNavigation.Search);
            Assert.IsNotNull(searchView);
            throw new NotImplementedException();
        }
    }
}
