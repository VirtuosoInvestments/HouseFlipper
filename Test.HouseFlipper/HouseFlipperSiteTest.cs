using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using NUnit.Framework;
using OpenQA.Selenium.Firefox;
using System.Net;

namespace Test.HouseFlipper
{
    [TestFixture]
    public class HouseFlipperSiteTest
    {
        private static HouseFlipperSite site;

        [OneTimeSetUp]
        [TestFixtureSetUp]
        public static void Setup()
        {
            site = new HouseFlipperSite();
        }

        [Test]
        public void Visit()
        {
            var req = HttpWebRequest.Create(site.Url);
            var resp = req.GetResponse() as HttpWebResponse;
            Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
        }

        [Test]
        public void Title()
        {
            //var site = new HouseFlipperSite();
            site.GoToSite();
            Assert.AreEqual("House Flipper", site.Title);
        }

        [Test]
        public void Navigation()
        {
            site.GoToSite();
            site.CheckExists(HouseFlipperSite.NavigationId);
        }   
        
        [Test]
        public void Search()
        {
            site.GoToSite();
            site.CheckCss(HouseFlipperSite.SearchViewLinkCss);
        }  
    }
}
