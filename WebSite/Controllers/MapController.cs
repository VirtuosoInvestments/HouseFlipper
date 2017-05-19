using Hack.HouseFlipper.DataAccess.Models;
using Hack.HouseFlipper.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Controllers
{
    public class MapController : Controller
    {
        // GET: Map
        public ActionResult Index()
        {
            //<TESTING> MapDisplay
            /*var mapDisplay = new MapDisplay();
            var searchId = (Guid)TempData[Constants.PropertySearchId.ToString()];
             var results =
            (from t in dbContext.SearchResults
            where t.SearchId == searchId
            select { new Property() { Id = t.PropertyId, Status = }}).ToList();
            //var list = new List<Property>();
            var markers = mapDisplay.GetMarkers(results);*/
            return View(/*markers*/);
        }
    }
}