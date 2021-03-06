﻿using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using NUnit.Framework;

namespace Test.HouseFlipper.WebSite
{
    [TestFixture]
    public class SearchViewTest
    {
        private static HouseFlipperSite site;
        private SearchView searchView;       

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
        [Category("NotImplemented")]
        public void PerformSearch()
        {
            var searchView = site.GoTo(Views.Search);
            Assert.IsNotNull(searchView);
            throw new NotImplementedException();
        }

        [Test]
        public void Visit()
        {
            var searchView = site.GoTo(Views.Search);            
            searchView.VerifyForm();
        }               
    }
}
