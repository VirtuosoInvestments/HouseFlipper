using Hack.HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace WebSite.Controllers
{
    public class ResultsController : Controller
    {
        // GET: Results
        public ActionResult Index()
        {
            //<TESTING> PropertySearch
            /*
            var dbContext = new MlsContext();
            var searchId = (Guid)TempData[Constants.PropertySearchId.ToString()];
            var results =
            (from t in dbContext.SearchResults
            where t.SearchId == searchId
            select t).ToList();
            */
            
            return View(/*results*/);
        }
    }
}