using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace HouseFlipper.WebSite.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult Index()
        {
            //<TESTING> PropertySearch
            /*var propSearch = new PropertySearch();
            var options = new PropertySearchOptions();
            propSearch.Id = Guid.NewGuid();
            TempData[Constants.PropertySearchId.ToString()] = propSearch.Id;
            propSearch.Search(options);*/
            return View(/*propSearch*/);
        }
    }
}
