using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Test.HouseFlipper.Common;

namespace Test.HouseFlipper.WebSite
{
    public enum Views { Search }

    public class HouseFlipperSite : MainSite
    {
        private static string siteUrl = Settings.Get(Setting.SiteUrl);

        public IWebElement Navigation
        {
            get { return this.Driver.FindElement(By.Id("navigation")); }
        }


        public ReadOnlyCollection<IWebElement> NavMenuItems

        {
            get { return this.Navigation.FindElements(By.XPath(".//ul/li")); }

        }
        public IWebElement SearchViewMenu
        {
            get
            {
                return this.Navigation.FindElement(By.XPath(".//ul/li[1]"));
            }
        }

        public string SearchViewLink
        {
            get
            {
                var searchMenuItem = this.SearchViewMenu;
                return searchMenuItem.FindElement(By.TagName("a")).GetAttribute("href");
            }
        }

        public HouseFlipperSite()
        {
            Console.WriteLine("HouseFlipper site url: " + siteUrl);
            RemoveVsFolder();
        }

        private void RemoveVsFolder()
        {
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\.vs";
            if (Directory.Exists(path))
            {
                foreach (var sub in Directory.GetDirectories(path))
                {
                    Directory.Delete(sub, true);
                }
            }
        }

        public override string Url { get { return siteUrl; } }


        public View GoTo(Views view)
        {
            this.GoToSite();
            var nav = this.Navigation;
            switch (view)
            {
                case Views.Search:
                    this.SearchViewMenu.Click();
                    var searchView = new SearchView(this);
                    return searchView;
                default:
                    throw new InvalidOperationException("Unhandled navigation: " + view);
            }
        }        
    }
}
