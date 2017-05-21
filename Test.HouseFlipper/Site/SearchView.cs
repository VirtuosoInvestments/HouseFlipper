using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.HouseFlipper
{
    public class SearchView : View
    {
        // Section for list of Elements by id or css path found on page/view

        public SearchView(HouseFlipperSite mainSite) : base(mainSite) { }
        protected override void HandleNavigateToView()
        {
            // Click on <li> with value "Search"
            throw new NotImplementedException();
        }
    }
}
