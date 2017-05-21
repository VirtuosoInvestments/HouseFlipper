using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Test.HouseFlipper
{
    public enum SiteNavigation { Search }

    public class HouseFlipperSite : MainSite
    {
        private static string siteUrl = Settings.Get(Setting.SiteUrl);

        public HouseFlipperSite()
        {
            Console.WriteLine("HouseFlipper site url: " + siteUrl);
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
