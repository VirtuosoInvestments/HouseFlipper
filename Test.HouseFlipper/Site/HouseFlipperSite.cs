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
    public enum SiteNavigation { Search }

    public class HouseFlipperSite : MainSite
    {
        private static string siteUrl = Settings.Get(Setting.SiteUrl);

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
    }
}
