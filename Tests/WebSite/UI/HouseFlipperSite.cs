using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Test.HouseFlipper.Common;

namespace Test.HouseFlipper
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
            var nav = this.Navigation;
            switch (view)
            {
                case Views.Search:
                    var link = nav.FindElement(By.LinkText(SearchView.LinkText));
                    link.Click();
                    var searchView = new SearchView(this);
                    return searchView;
                default:
                    throw new InvalidOperationException("Unhandled navigation: " + view);
            }
        }

        public void CheckExists(string id)
        {
            if (Find(id) == null)
            {
                throw new InvalidOperationException("Error: Could not locate element with id='" + id + "'!");
            }
        }

        private IWebElement Find(string id)
        {
            var by = By.Id(id);
            return Find(by);
        }

        private IWebElement Find(By by)
        {
            return this.Driver.FindElement(by);
        }
    }
}
