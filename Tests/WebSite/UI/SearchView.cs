using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.HouseFlipper
{
    public class SearchView : View
    {
        public static string LinkText { get { return "Search"; } }

        public static string SoldCheckBoxId = "soldCheckBox";
        public static string ActiveCheckBoxId = "activeCheckBox";
        public static string WithinTextFieldId = "withinTextBox";

        // Section for list of Elements by id or css path found on page/view

        public SearchView(HouseFlipperSite mainSite) : base(mainSite) { }

        public IWebElement GetCheckBox(string id)
        {
            return this.Driver.FindElement(By.Id(id));
        }

        public IWebElement GetTextField(string id)
        {
            return this.Driver.FindElement(By.Id(id));
        }

        public override void VerifyForm()
        {
            if (GetCheckBox(SoldCheckBoxId) == null)
            {
                throw new InvalidOperationException("Error: Could not locate sold checkbox!");
            }

            if (GetCheckBox(ActiveCheckBoxId) == null)
            {
                throw new InvalidOperationException("Error: Could not locate active checkbox!");
            }

            if(GetTextField(WithinTextFieldId) == null)
            {
                throw new InvalidOperationException("Error: Could not locate within text field!");
            }
        }


    }
}
