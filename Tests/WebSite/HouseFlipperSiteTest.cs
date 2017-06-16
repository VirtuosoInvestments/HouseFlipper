using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using NUnit.Framework;
using OpenQA.Selenium.Firefox;
using System.Net;
using Test.HouseFlipper.Common;

namespace Test.HouseFlipper.WebSite
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

        [OneTimeTearDown]
        [TestFixtureTearDown]
        public static void TearDown()
        {
            site.Dispose();
        }


        [Test]
        public void Visit()
        {
            var req = HttpWebRequest.Create(site.Url);
            var resp = req.GetResponse() as HttpWebResponse;
            Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
        }

        [Test]
        [Category("PathIssue")]
        public void Title()
        {
            //var site = new HouseFlipperSite();
            site.GoToSite();
            Assert.AreEqual("House Flipper", site.Title);
        }

        [Test]
        [Category("PathIssue")]
        public void Navigation()
        {
            site.GoToSite();
            var navMenu = site.Navigation;
            Assert.IsNotNull(navMenu);
            var navMenuItems = site.NavMenuItems;
            Assert.IsNotNull(navMenuItems);
            Assert.AreEqual(1, navMenuItems.Count);
        }   
        
        [Test]
        [Category("PathIssue")]
        public void Search()
        {
            site.GoToSite();
            var searchMenu = site.SearchViewMenu;
            Assert.IsNotNull(searchMenu);
            Assert.AreEqual("Search", searchMenu.Text);

            var searchLink = site.SearchViewLink;
            Assert.IsNotNull(searchLink);
            Assert.AreEqual(Settings.Get(Setting.SiteUrl) + "/Search", searchLink);
        }  
    }
}
