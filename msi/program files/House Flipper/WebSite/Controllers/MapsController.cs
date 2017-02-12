using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace WebSite.Controllers
{
    public class MapsController : Controller
    {
        // GET = Maps
        public ActionResult ViewMaps()
        {
            // Add action logic here
            return View();
        }

        public ActionResult MapAddress()
        {
            return View();
        }

        public ActionResult GoogleMap()
        {
            return View();
        }

        public ActionResult GoogleMap2()
        {
            return View();
        }

        public ActionResult GoogleMap3()
        {
            return View();
        }

        public ActionResult GoogleMap4()
        {
            return View();
        }

        public ActionResult GoogleMap5()
        {
            return View();
        }

        public ActionResult GoogleMap6()
        {
            var markers = new List<Marker>() {
                new Marker()
                {
                    title = "Aksa Beach",
                    lat = "19.1759668",
                    lng = "72.79504659999998",
                    description = "Aksa Beach is a popular beach and a vacation spot in Aksa village at Malad, Mumbai.",
                    icon = "http://maps.google.com/mapfiles/ms/icons/yellow-dot.png"
                },
                new Marker()
                {
                    title = "Juhu Beach",
                    lat = "19.0883595",
                    lng = "72.82652380000002",
                    description = "Juhu Beach is one of favourite tourist attractions situated in Mumbai.",
                    icon = "http://maps.google.com/mapfiles/ms/icons/orange-dot.png"
                },
                new Marker()
                {
                    title = "Girgaum Beach",
                    lat = "18.9542149",
                    lng = "72.81203529999993",
                    description = "Girgaum Beach commonly known as just Chaupati is one of the most famous public beaches in Mumbai.",
                    icon = "http://maps.google.com/mapfiles/ms/icons/green-dot.png"
                },
                new Marker()
                {
                    title = "Jijamata Udyan",
                    lat = "18.979006",
                    lng = "72.83388300000001",
                    description = "Jijamata Udyan is situated near Byculla station is famous as Mumbai (Bombay) Zoo.",
                    icon = "http://maps.google.com/mapfiles/ms/icons/red-dot.png"
                },
                new Marker()
                {
                    title = "Sanjay Gandhi National Park",
                    lat = "19.2147067",
                    lng = "72.91062020000004",
                    description = "Sanjay Gandhi National Park is a large protected area in the northern part of Mumbai city.",
                    icon = "http://maps.google.com/mapfiles/ms/icons/blue-dot.png"
                }
            };            
            return View(markers);
        }
    }
}