using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.HouseFlipper.WebSite
{
    public class WebElement
    {
        public static IWebDriver Driver { get; set; }
        public static void CheckExists(string id)
        {
            if (Find(By.Id(id)) == null)
            {
                throw new InvalidOperationException("Error: Could not locate element with id='" + id + "'!");
            }
        }

        public static void CheckExists(By by)
        {
            if (Find(by) == null)
            {
                throw new InvalidOperationException("Error: Could not locate element with by='" + by.ToString() + "'!");
            }
        }

        internal static IWebElement FindByXPath(string search)
        {
            IWebElement elem = null;
            if ((elem=Find(By.XPath(search))) == null)
            {
                throw new InvalidOperationException("Error: Could not locate element with xpath='" + search + "'!");
            }
            return elem;
        }

        public static void CheckExists(CssSearch css)
        {
            var search = css.ToString();
            if (Find(By.CssSelector(search)) == null)
            {
                throw new InvalidOperationException("Error: Could not locate element with css='" + search + "'!");
            }
        }

        private static IWebElement Find(By by)
        {
            return Driver.FindElement(by);
        }
    }
}
