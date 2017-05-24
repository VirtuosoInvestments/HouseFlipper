using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using System.IO;

namespace Test.HouseFlipper
{
    public enum Views { Search }

    public class HouseFlipperSite : MainSite
    {
        private static string siteUrl = Settings.Get(Setting.SiteUrl);

        public static string NavigationId = "navigation";
        public static string SearchViewLinkCss = "";

        public HouseFlipperSite()
        {
            Console.WriteLine("HouseFlipper site url: " + siteUrl);
            RemoveVsFolder();
        }

        private void RemoveVsFolder()
        {
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\.vs";
            if(Directory.Exists(path))
            {
                foreach(var sub in Directory.GetDirectories(path))
                {
                    Directory.Delete(sub, true);
                }
            }
        }

        public override string Url { get { return siteUrl; } }       

        public View GoTo(Views view)
        {
            var nav = this.Driver.FindElement(By.Id(NavigationId));
            switch(view)
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

        public void CheckCss(object searchViewLinkCss)
        {
            throw new NotImplementedException();
        }

        public void CheckExists(string id)
        {
            if(Find(id)==null)
            {
                throw new InvalidOperationException("Error: Could not locate element with id='"+id+"'!");
            }
        }

        private IWebElement Find(string id)
        {
            return this.Driver.FindElement(By.Id(id));
        }        
    }
}
