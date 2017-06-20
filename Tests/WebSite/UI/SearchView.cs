using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.HouseFlipper.WebSite
{
    public class SearchView : View
    {
        public static string LinkText { get { return "Search"; } }

        public static string SoldCheckBoxId = "soldCheckBox";
        public static string ActiveCheckBoxId = "activeCheckBox";
        public static string WithinTextFieldId = "withinTextBox";

        // Section for list of Elements by id or css path found on page/view

        public SearchView(HouseFlipperSite mainSite) : base(mainSite) { }

        public override void VerifyForm()
        {

            WebElement.FindByXPath(".//label[text()='Sold']");//WebElement.CheckExists(new CssSearch() { Text = "Sold", TagName = "label" });
            WebElement.CheckExists(SoldCheckBoxId);
            WebElement.FindByXPath(".//label[text()='Active']");//WebElement.CheckExists(new CssSearch() { Text = "Active", TagName = "label" });
            WebElement.CheckExists(ActiveCheckBoxId);
            WebElement.FindByXPath(".//label[text()='Within (miles)']");// WebElement.CheckExists(new CssSearch() { Text = "Within (miles)", TagName = "label" });
            WebElement.CheckExists(WithinTextFieldId);            
        }
    }
}
