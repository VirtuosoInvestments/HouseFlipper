using Hack.HouseFlipper.DataAccess.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace WebSite.Controllers
{
    public class ActiveController : Controller
    {
        private static string dataFolder = @"~/App_Data";
        private static string byZipFile = "activezip.json";
        private static string byZipNoPool = "activezip.json";
        private static string byDivFile = "activediv.json";
        private static string byCommDivFile = "activecommdiv.json";
        private static Lazy<List<MlsRow>> activeHouses =
           new Lazy<List<MlsRow>>(
               () =>
               {
                   return InitActiveHouses();                   
               });
        private static Lazy<List<MlsRow>> activeHousesNoPool =
           new Lazy<List<MlsRow>>(
               () =>
               {
                   return InitActiveHousesNoPool();
               });
        private static Lazy<List<MlsRow>> activeByDiv =
           new Lazy<List<MlsRow>>(
               () =>
               {
                   return InitActiveByDiv();
               });

        private static Lazy<List<MlsRow>> activeByCommDiv =
           new Lazy<List<MlsRow>>(
               () =>
               {
                   return InitActiveByCommonDiv();
               });

        private static Lazy<Dictionary<string, List<MlsRow>>> zipActive =
            new Lazy<Dictionary<string,List<MlsRow>>>(
                ()=>{
                    var lookup = new Dictionary<string, List<MlsRow>>(StringComparer.OrdinalIgnoreCase);
                    InitActiveHouses(lookup);   
                    return lookup;
                });

        public static List<MlsRow> ActiveHomes(string zip, double maxPrice)
        {
            //var all =
            //(from el in activeHouses.Value
            //where el.PostalCode==zipcode
            //select el).ToList();

            var list = new List<MlsRow>();
            if (zipActive.Value.ContainsKey(zip))
            {
                var all = zipActive.Value[zip];
                foreach (var item in all)
                {
                    if (item.CurrentPriceValue() <= maxPrice)
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public static List<MlsRow> ActiveHomes(string zip, double maxPrice, double avgBed, double avgFullBath, double avgHalfBath, double avgSqft)
        {
            var list = new List<MlsRow>();
            if (zipActive.Value.ContainsKey(zip))
            {
                var all = zipActive.Value[zip];
                foreach (var item in all)
                {
                    if (item.CurrentPriceValue() <= maxPrice &&
                        item.BedsValue()==avgBed &&
                        item.FullBathsValue()==avgFullBath &&
                        item.HalfBathsValue()==avgHalfBath &&
                        item.SqFtHeatedValue()==avgSqft)
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public static List<MlsRow> ActiveHomes(
            string zip, 
            double maxPrice, 
            double avgBed, 
            //double avgFullBath, 
            //double avgHalfBath, 
            double minSqFt, 
            double maxSqFt, 
            int minYear, 
            int maxYear)
        {
            var list = new List<MlsRow>();
            if (zipActive.Value.ContainsKey(zip))
            {
                var all = zipActive.Value[zip];
                foreach (var house in all)
                {
                    if (house.CurrentPriceValue() <= maxPrice) //criteria #1: price
                    {
                        var houseYear = house.YearBuiltValue();
                        if (minYear <= houseYear && houseYear <= maxYear) //criteria #2: year built
                        {
                            var houseSqft = house.SqFtHeatedValue();
                            if (minSqFt <= houseSqft && houseSqft <= maxSqFt) //criteria #3: sq ft
                            {
                                var minBeds = avgBed;
                                if (minBeds <= house.BedsValue()) //criteria #4: same beds or more
                                {
                                    list.Add(house);
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }

        private static Lazy<List<MlsRow>> allActive =
            new Lazy<List<MlsRow>>(
                () => {
                    var target = new ListingsController();
                    var list = target.GetActive();
                    if (list == null)
                    {
                        list = new List<MlsRow>();
                    }
                    return list;
                });

        public ActionResult Index()
        {            
            return View(allActive.Value);
        }

        // GET: Active
        public ActionResult ByZip()
        {
            string withinStr = ViewBag.WithinPercent;
            double within = .20;
            if (!string.IsNullOrWhiteSpace(withinStr))
            {
                within = double.Parse(withinStr)/100;
            }

            if (within == 20)
            {
                foreach (var item in activeHouses.Value)
                {
                    var data = FlipsController.FlipByZip;
                    CalculateNeededValues(item, data, "zip");
                }
                return View(activeHouses.Value.OrderBy(x => x.PostalCode).ThenBy(x=>x.LegalSubdivisionName));
            }
            else
            {
                var customList = new List<MlsRow>();
                var data = FlipsController.FlipByZip;
                foreach(var flipChar in data)
                {
                    var zip = flipChar.Zipcode;
                    var preflip = flipChar.PreFlip;
                    var min = -1.0 * within * preflip.Price;
                    var max = (1.0 + within) * preflip.Price;

                    foreach(var active in activeHouses.Value)
                    {
                        var myPrice = active.CurrentPriceValue();
                        if(myPrice>=min && myPrice<=max)
                        {
                            customList.Add(active);
                            CalculateNeededValues(active, data, "zip");
                        }
                    }                    
                }
                return View(customList.OrderBy(x => x.PostalCode).ThenBy(x => x.LegalSubdivisionName));
            }            
        }

        public ActionResult ByZipNoPool()
        {
            string withinStr = ViewBag.WithinPercent;
            double within = .20;
            if (!string.IsNullOrWhiteSpace(withinStr))
            {
                within = double.Parse(withinStr) / 100;
            }

            if (within == 20)
            {
                foreach (var item in activeHousesNoPool.Value)
                {
                    var data = FlipsController.FlipByZip;
                    CalculateNeededValues(item, data, "zip");
                }
                return View(activeHouses.Value.OrderBy(x => x.PostalCode).ThenBy(x => x.LegalSubdivisionName));
            }
            else
            {
                var customList = new List<MlsRow>();
                var data = FlipsController.FlipByZipNoPool;
                foreach (var flipChar in data)
                {
                    var zip = flipChar.Zipcode;
                    var preflip = flipChar.PreFlip;
                    var min = -1.0 * within * preflip.Price;
                    var max = (1.0 + within) * preflip.Price;

                    foreach (var active in activeHousesNoPool.Value)
                    {
                        var myPrice = active.CurrentPriceValue();
                        if (myPrice >= min && myPrice <= max)
                        {
                            customList.Add(active);
                            CalculateNeededValues(active, data, "zip");
                        }
                    }
                }
                return View(customList.OrderBy(x => x.PostalCode).ThenBy(x => x.LegalSubdivisionName));
            }
        }
        
        public ActionResult BySubDivision()
        {
            foreach (var item in activeByDiv.Value)
            {
                var data = FlipsController.FlipBySubDivision;
                CalculateNeededValues(item, data, "subdiv");
            }
            return View(activeByDiv.Value.OrderBy(x => x.LegalSubdivisionName));
        }

        public ActionResult ByCommonSubDivision()
        {
            foreach (var item in activeByCommDiv.Value)
            {
                var data = FlipsController.FlipByCommonSubDivision;
                CalculateNeededValues(item, data, "subdiv");
            }
            return View(activeByDiv.Value.OrderBy(x => x.LegalSubdivisionName));
        }
        
        private static List<MlsRow> InitActiveHouses(Dictionary<string,List<MlsRow>> lookup=null)
        {
            //string path = HostingEnvironment.MapPath(
            //    string.Format("{0}/{1}", dataFolder, byZipFile));

            //string json = null;
            //using (var sr = new StreamReader(path))
            //{
            //    json = sr.ReadToEnd();
            //}
            //var array = JsonConvert.DeserializeObject<List<MlsRow>[]>(json);
            //var list = new List<MlsRow>();
            //foreach (var a in array)
            //{
                //list.AddRange(a);
            var a = allActive.Value;
                foreach(var h in a)
                {
                    var zip = h.PostalCode;
                    if (lookup != null)
                    {
                        if (lookup.ContainsKey(zip))
                        {
                            lookup[zip].Add(h);
                        }
                        else
                        {
                            lookup.Add(zip, new List<MlsRow>() { h });
                        }
                    }
                }
            //}
            return a;
        }

        private static List<MlsRow> InitActiveHousesNoPool()
        {
            string path = HostingEnvironment.MapPath(
                string.Format("{0}/{1}", dataFolder, byZipNoPool));

            string json = null;
            using (var sr = new StreamReader(path))
            {
                json = sr.ReadToEnd();
            }
            var array = JsonConvert.DeserializeObject<List<MlsRow>[]>(json);
            var list = new List<MlsRow>();
            foreach (var a in array)
            {
                list.AddRange(a);
            }
            return list;
        }

        private static List<MlsRow> InitActiveByDiv()
        {
            string path = HostingEnvironment.MapPath(
                string.Format("{0}/{1}", dataFolder, byDivFile));

            string json = null;
            using (var sr = new StreamReader(path))
            {
                json = sr.ReadToEnd();
            }
            var array = JsonConvert.DeserializeObject<List<MlsRow>[]>(json);            
            var list = new List<MlsRow>();
            foreach (var a in array)
            {
                list.AddRange(a);
            }
            return list;
        }

        private static List<MlsRow> InitActiveByCommonDiv()
        {
            string path = HostingEnvironment.MapPath(
                string.Format("{0}/{1}", dataFolder, byCommDivFile));

            string json = null;
            using (var sr = new StreamReader(path))
            {
                json = sr.ReadToEnd();
            }
            var array = JsonConvert.DeserializeObject<List<MlsRow>[]>(json);            
            var list = new List<MlsRow>();
            foreach (var a in array)
            {
                list.AddRange(a);
            }
            return list;
        }

        private static void CalculateNeededValues(
            MlsRow item, 
            List<FlippedCharacteristics> data,
            string by)
        {
            List<FlippedCharacteristics> list = null;

            if (by.ToLower().Equals("zip"))
            {
                list = (from el in data
                        where el.Zipcode.Equals(item.PostalCode)
                        select el).ToList();
            }
            else if(by.ToLower().Equals("subdiv"))
            {
                list = (from el in data
                        where el.SubDivision.Equals(item.LegalSubdivisionName)
                        select el).ToList();
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("Error: Unhandled by value: '{0}'",by));
            }

            var preflip = list[0].PreFlip;
            var postflip = list[0].PostFlip;
            var avgPreFlipPrice = preflip.Price;
            item.PreFlipAvgPrice = avgPreFlipPrice;

            var min = 0.80 * avgPreFlipPrice;
            var max = 1.20 * avgPreFlipPrice;

            var myPrice = item.CurrentPriceValue();
            var calc = myPrice / avgPreFlipPrice;
            var within = (calc - 1) * 100;
            item.WithinPreFlipPercent = within;

            item.PreFlipAvgLPSqFt = preflip.PriceSqFt;
            item.PostFlipAvgSPSqFt = postflip.PriceSqFt;

            var spsqft = postflip.PriceSqFtValue();
            var sqft = item.SqFtHeatedValue();
            var arv = spsqft * sqft;
            item.SetAfterRepairValue(arv);
        }        
    }
}