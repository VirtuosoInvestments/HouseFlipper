using Hack.HouseFlipper.DataAccess.DB;
using Hack.HouseFlipper.DataAccess.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace WebSite.Controllers
{

    public class FlipsController : Controller
    {
        enum FlipStatsType
        {
            PreFlip,
            PostFlip
        }
        private static string dataFolder = @"~/App_Data";
        private static string file = "flippedHouses.json";
        private static string byZipFile = "flipbyzip.json";
        private static string byZipNoPool = "flipbyzipNoPool.json";

        private static string byDivFile = "flipbydiv.json";
        private static string byCommDivFile = "flipbycommdiv.json";

        private MlsContext db = new MlsContext();
        private static Lazy<List<FlippedHouse>> flippedHouses =
            new Lazy<List<FlippedHouse>>(
                () =>
                {
                    return InitFlippedHouses();
                });
        /*
        private static Lazy<Dictionary<string, Dictionary<string, Subdivision>>> zipSubdivFlippedHomes =
            new Lazy<Dictionary<string, Dictionary<string, Subdivision>>>(
                () =>
                {
                    return InitFlipStatsByZipSubdiv();
                });

        private static Dictionary<string, Dictionary<string, Subdivision>> InitFlipStatsByZipSubdiv()
        {
            var zipLookup = new Dictionary<string, Dictionary<string, Subdivision>>(StringComparer.OrdinalIgnoreCase);
            var summaryDivBought = new Dictionary<string, Dictionary<string, HouseCharacteristics>>(StringComparer.OrdinalIgnoreCase);
            var summaryDivSold = new Dictionary<string, Dictionary<string, HouseCharacteristics>>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in flippedHouses.Value)
            {
                var zip = f.PostalCode.Trim();
                var subdiv = f.Subdivision.Trim().ToUpper();
                HouseCharacteristics avgDivBought;
                HouseCharacteristics avgDivSold;
                GetAverageVars(summaryDivBought, summaryDivSold, zip, subdiv, out avgDivBought, out avgDivSold);

                Dictionary<string, Subdivision> subdivLookup;
                if (zipLookup.ContainsKey(zip))
                {
                    subdivLookup = zipLookup[zip];
                }
                else
                {
                    subdivLookup = new Dictionary<string, Subdivision>(StringComparer.OrdinalIgnoreCase);
                    zipLookup.Add(zip, subdivLookup);
                }

                var target = new ListingsController();
                List<MlsRow> list = target.Search(f);
                if (list == null || list.Count == 0)
                {
                    if (list == null)
                    {
                        list = new List<MlsRow>();
                    }
                }
                else
                {
                    MlsRow bought;
                    MlsRow sold;
                    GetBoughtSold(list, out bought, out sold);

                    SetValues(f, bought);

                    double proftNum = -1;
                    double boughtROI = -1;
                    double soldROI = -1;
                    if (sold != null)
                    {
                        SetAdditionalValues(f, bought, sold, out proftNum, out boughtROI, out soldROI);
                    }
                    AccumulateValues(f, avgDivBought, avgDivSold, bought, sold, proftNum, boughtROI, soldROI);
                }

                if (subdivLookup.ContainsKey(subdiv))
                {
                    subdivLookup[subdiv].Add(f);
                }
                else
                {
                    subdivLookup.Add(subdiv, new Subdivision(new List<FlippedHouse>() { f }, zip) { Zipcode = zip, Subdivision = subdiv });
                }
            }

            foreach (var zip in zipLookup.Keys)
            {
                var subdivLookup = zipLookup[zip];
                foreach (var subdiv in subdivLookup.Keys)
                {
                    var homes = subdivLookup[subdiv];
                    double rawAvgSPSqFT;
                    double rawAvgPrice;
                    SetAverages(homes, out rawAvgSPSqFT, out rawAvgPrice);
                    SetActiveHomes(zip, homes, rawAvgSPSqFT, rawAvgPrice);
                }
            }

            return zipLookup;
        }
        */
        private static Lazy<List<FlippedCharacteristics>> flipByZip =
            new Lazy<List<FlippedCharacteristics>>(
        () =>
        {
            return InitFlipByZip();
        });
        private static Dictionary<string, HouseCharacteristics> avgPreFlipInZip =
            new Dictionary<string, HouseCharacteristics>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, HouseCharacteristics> avgPostFlipInZip =
            new Dictionary<string, HouseCharacteristics>(StringComparer.OrdinalIgnoreCase);
        private static Lazy<Zips> flipZips = new Lazy<Zips>(
            () =>
            {
                return InitFlipStatsByZip();
            });

        private static Dictionary<string, Dictionary<string, Subdivision>> zipSubdivFlippedHomes
            = new Dictionary<string, Dictionary<string, Subdivision>>(StringComparer.OrdinalIgnoreCase);

        private static Zips InitFlipStatsByZip()
        {
            var result = new Zips();
            var flipHomesInZip = result.Flips;
            var zipLookup = zipSubdivFlippedHomes;
            var summaryZipBought = new Dictionary<string, HouseCharacteristics>(StringComparer.OrdinalIgnoreCase);
            var summaryZipSold = new Dictionary<string, HouseCharacteristics>(StringComparer.OrdinalIgnoreCase);
            var summaryDivBought = new Dictionary<string, Dictionary<string, HouseCharacteristics>>(StringComparer.OrdinalIgnoreCase);
            var summaryDivSold = new Dictionary<string, Dictionary<string, HouseCharacteristics>>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in flippedHouses.Value)
            {
                var zip = f.PostalCode.Trim();
                HouseCharacteristics avgZipBought = null;
                HouseCharacteristics avgZipSold = null;
                HouseCharacteristics avgDivBought = null;
                HouseCharacteristics avgDivSold = null;
                GetAverageVars(summaryZipBought, summaryZipSold, zip, out avgZipBought, out avgZipSold);

                var target = new ListingsController();
                List<Listing> list = target.Search(f);
                if (list == null || list.Count == 0)
                {
                    if (list == null)
                    {
                        list = new List<Listing>();
                    }
                }
                else
                {
                    Listing bought;
                    Listing sold;
                    GetBoughtSold(list, out bought, out sold);
                    SetValues(f, bought);

                    var subdiv = f.Subdivision.Trim().ToUpper();
                    GetAverageVars(summaryDivBought, summaryDivSold, zip, subdiv, out avgDivBought, out avgDivSold);

                    Dictionary<string, Subdivision> subdivLookup;
                    if (zipLookup.ContainsKey(zip))
                    {
                        subdivLookup = zipLookup[zip];
                    }
                    else
                    {
                        subdivLookup = new Dictionary<string, Subdivision>(StringComparer.OrdinalIgnoreCase);
                        zipLookup.Add(zip, subdivLookup);
                    }

                    if (subdivLookup.ContainsKey(subdiv))
                    {
                        subdivLookup[subdiv].Add(f);
                    }
                    else
                    {
                        Subdivision t;
                        subdivLookup.Add(subdiv, t=new Subdivision(new List<FlippedHouse>() { f }, zip) { Zipcode = zip, Subdivision = subdiv });
                        t.AverageBought = avgDivBought;
                        t.AverageSold = avgDivSold;
                    }

                    double proftNum = -1;
                    double boughtROI = -1;
                    double soldROI = -1;
                    if (sold != null)
                    {
                        SetAdditionalValues(f, bought, sold, out proftNum, out boughtROI, out soldROI);
                    }
                    AccumulateValues(f, avgZipBought, avgZipSold, bought, sold, proftNum, boughtROI, soldROI);
                    AccumulateValues(f, avgDivBought, avgDivSold, bought, sold, proftNum, boughtROI, soldROI);
                }

                Homes h = AddZipFlip(result, flipHomesInZip, f, zip, avgZipBought, avgZipSold);
                AddSubdivFlip(f, zip, h, avgDivBought, avgDivSold);
            }

            foreach (var zip in flipHomesInZip.Keys)
            {
                var homes = flipHomesInZip[zip];
                double rawAvgSPSqFT = -1;
                double rawAvgPrice = -1;
                SetAverages(homes, out rawAvgSPSqFT, out rawAvgPrice);
                SetActiveHomes(zip, homes, rawAvgSPSqFT, rawAvgPrice);

                var subdivLookup = zipLookup[zip];
                foreach (var subdiv in subdivLookup.Keys)
                {
                    Subdivision s = subdivLookup[subdiv];
                    rawAvgSPSqFT = -1;
                    rawAvgPrice = -1;
                    SetAverages(s, out rawAvgSPSqFT, out rawAvgPrice);
                    SetActiveHomes(zip, s, rawAvgSPSqFT, rawAvgPrice);

                    if(homes.Subdivisions.Lookup.ContainsKey(subdiv))
                    {
                        var t = homes.Subdivisions.Lookup[subdiv];

                        t.AverageBeds = s.AverageBeds;
                        t.AverageFullBaths = s.AverageFullBaths;
                        t.AverageHalfBaths = s.AverageHalfBaths;
                        t.AverageYearBuilt = s.AverageYearBuilt;
                        t.AveragePurchase = s.AveragePurchase;
                        t.AverageSoldPrice = s.AverageSoldPrice;
                        t.AverageSqFtHeated = s.AverageSqFtHeated;

                        t.AverageProfit = s.AverageProfit;
                        t.AverageADOM = s.AverageADOM;
                        t.AveragePPSqFt = s.AveragePPSqFt;
                        t.AverageSPSqFT = s.AverageSPSqFT;
                        t.AverageROI = s.AverageROI;
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("Error: Subdiv not found - '{0}'", subdiv));
                    }
                }
            }

            return result;
        }

        private static Zips InitFlipStatsByZipOld()
        {
            var result = new Zips();
            var flipHomesInZip = result.Flips;
            var summaryZipBought = new Dictionary<string, HouseCharacteristics>(StringComparer.OrdinalIgnoreCase);
            var summaryZipSold = new Dictionary<string, HouseCharacteristics>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in flippedHouses.Value)
            {
                var zip = f.PostalCode.Trim();
                HouseCharacteristics avgZipBought;
                HouseCharacteristics avgZipSold;
                GetAverageVars(summaryZipBought, summaryZipSold, zip, out avgZipBought, out avgZipSold);

                var target = new ListingsController();
                List<Listing> list = target.Search(f);
                if (list == null || list.Count == 0)
                {
                    if (list == null)
                    {
                        list = new List<Listing>();
                    }
                }
                else
                {
                    Listing bought;
                    Listing sold;
                    GetBoughtSold(list, out bought, out sold);
                    SetValues(f, bought);

                    double proftNum = -1;
                    double boughtROI = -1;
                    double soldROI = -1;
                    if (sold != null)
                    {
                        SetAdditionalValues(f, bought, sold, out proftNum, out boughtROI, out soldROI);
                    }
                    AccumulateValues(f, avgZipBought, avgZipSold, bought, sold, proftNum, boughtROI, soldROI);
                }

                Homes h = AddZipFlip(result, flipHomesInZip, f, zip, avgZipBought, avgZipSold);
                AddSubdivFlip(f, zip, h, null, null);
            }

            foreach (var zip in flipHomesInZip.Keys)
            {
                var homes = flipHomesInZip[zip];
                double rawAvgSPSqFT;
                double rawAvgPrice;
                SetAverages(homes, out rawAvgSPSqFT, out rawAvgPrice);
                SetActiveHomes(zip, homes, rawAvgSPSqFT, rawAvgPrice);
            }

            return result;
        }

        private static void GetAverageVars(
            Dictionary<string, Dictionary<string, HouseCharacteristics>> summaryBought,
            Dictionary<string, Dictionary<string, HouseCharacteristics>> summarySold,
            string zip,
            string subdiv,
            out HouseCharacteristics avgBought,
            out HouseCharacteristics avgSold)
        {
            avgBought = new HouseCharacteristics();
            avgSold = new HouseCharacteristics();

            Dictionary<string, HouseCharacteristics> zipBought;
            Dictionary<string, HouseCharacteristics> zipSold;
            if (summaryBought.ContainsKey(zip))
            {
                zipBought = summaryBought[zip];
            }
            else
            {
                zipBought = new Dictionary<string, HouseCharacteristics>();
                summaryBought.Add(zip, zipBought);
            }

            if (summarySold.ContainsKey(zip))
            {
                zipSold = summarySold[zip];
            }
            else
            {
                zipSold = new Dictionary<string, HouseCharacteristics>();
                summarySold.Add(zip, zipSold);
            }

            if (zipBought.ContainsKey(subdiv))
            {
                avgBought = zipBought[subdiv];
            }
            else
            {
                zipBought.Add(subdiv, avgBought);
            }

            if (zipSold.ContainsKey(subdiv))
            {
                avgSold = zipSold[subdiv];
            }
            else
            {
                zipSold.Add(subdiv, avgSold);
            }
        }

        private static void GetAverageVars(
            Dictionary<string, HouseCharacteristics> summaryBought,
            Dictionary<string, HouseCharacteristics> summarySold,
            string zip,
            out HouseCharacteristics avgBought,
            out HouseCharacteristics avgSold)
        {
            avgBought = new HouseCharacteristics();
            avgSold = new HouseCharacteristics();
            if (summaryBought.ContainsKey(zip))
            {
                avgBought = summaryBought[zip];
            }
            else
            {
                summaryBought.Add(zip, avgBought);
            }

            if (summarySold.ContainsKey(zip))
            {
                avgSold = summarySold[zip];
            }
            else
            {
                summarySold.Add(zip, avgSold);
            }
        }

        private static Homes AddZipFlip(Zips result, Dictionary<string, Homes> flipHomesInZip, FlippedHouse f, string zip, HouseCharacteristics avgBought, HouseCharacteristics avgSold)
        {
            Homes h = null;
            if (flipHomesInZip.ContainsKey(zip))
            {
                h = flipHomesInZip[zip];
                h.Add(f);
            }
            else
            {
                //flipHomesInZip.Add(zip, new List<FlippedHouse>() { f });

                flipHomesInZip.Add(zip, h = new Homes(new List<FlippedHouse>() { f }, zip) { Zipcode = zip, AverageSold = avgSold, AverageBought = avgBought });
                result.Add(h);
            }
            return h;
        }

        private static void AddSubdivFlip(
            FlippedHouse f,
            string zip,
            Homes h,
            HouseCharacteristics avgBought,
            HouseCharacteristics avgSold)
        {
            Subdivision s;
            var subdiv = f.Subdivision.Trim().ToUpper();
            if (h.Subdivisions.Lookup.ContainsKey(subdiv))
            {
                s = h.Subdivisions.Lookup[subdiv];
                s.Add(f);
            }
            else
            {
                h.Subdivisions.Lookup.Add(subdiv, s = new Subdivision(new List<FlippedHouse>() { f }, zip) { Zipcode = zip, Subdivision = subdiv, AverageSold = avgSold, AverageBought = avgBought });
                h.Subdivisions.Add(s);
            }
        }

        private static void SetActiveHomes(string zip, Homes homes, double rawAvgSPSqFT, double rawAvgPrice)
        {
            //Find Active homes under 120% of avg price, matching avg beds, avg baths, avg sqft (over/under 200 sqft), within decade of avg year built
            //var avgPrice = homes.AveragePurchaseValue;
            var maxPrice = 1.2 * rawAvgPrice;
            var avgBed = homes.AverageBedsValue;
            var avgFullBath = homes.AverageFullBathsValue;
            var avgHalfBath = homes.AverageHalfBathsValue;
            var avgSqft = homes.AverageSqFtHeatedValue;
            var minSqft = avgSqft - 200;
            var maxSqft = avgSqft + 200;
            var avgYearBuilt = homes.AverageYearBuilt;
            var roundDown = RoundDown((int)avgYearBuilt);
            var roundUp = RoundUp((int)avgYearBuilt);
            int minYear = roundDown, maxYear = roundUp;
            //if (avgYearBuilt == roundDown) //Year divisible by 10 i.e. 1980
            //{
            //    minYear = roundDown;
            //    maxYear = roundDown + 9;  //we set the max year to 1989
            //}
            //else
            //{
            //    minYear = roundDown;
            //    maxYear = roundUp - 1;
            //}
            var activeList = ActiveController.ActiveHomes(
                zip,
                maxPrice,
                avgBed,
                //avgFullBath,
                //avgHalfBath,
                minSqft,
                maxSqft,
                minYear,
                maxYear);

            var h = homes;
            foreach (var item in activeList)
            {
                item.SetAfterRepairValue(rawAvgSPSqFT * item.SqFtHeatedValue());
                var subdiv = item.LegalSubdivisionName.Trim().ToUpper();
                Subdivision s;
                var divs = h.Subdivisions;
                if (divs.Lookup.ContainsKey(subdiv))
                {
                    s = divs.Lookup[subdiv];
                    if (s.Active == null)
                    {
                        s.Active = new Homes();
                    }
                    s.Active.Add(item);
                }
                else
                {
                    divs.Lookup.Add(subdiv, s = new Subdivision(new List<Listing>() { }, zip) { Zipcode = zip, Subdivision = subdiv });
                    s.Active = new Homes() { };
                    s.Active.Add(item);
                    divs.Add(s);
                    //throw new InvalidOperationException(
                    //    string.Format("Error: Cannot find subdiv for active property '{0}': '{1}'", 
                    //    item.Address, 
                    //    item.LegalSubdivisionName));
                }
            }
            homes.Active = new Homes(activeList, zip) { Zipcode = zip };
        }

        private static void SetAverages(Homes homes, out double rawAvgSPSqFT, out double rawAvgPrice)
        {
            double count = homes.Count;
            var avgBought = homes.AverageBought;
            var avgSold = homes.AverageSold;
            avgSold.Price /= count;
            avgSold.SqFtHeated /= count;
            avgSold.Beds /= count;
            avgSold.FullBaths /= count;
            avgSold.HalfBaths /= count;
            avgSold.PoolValue /= count;
            avgSold.YearBuilt /= count;
            avgSold.SPSqFT /= count;
            //avgSold.PPSqFt /= count;
            avgSold.Profit /= count;
            avgSold.ROIOnPurchasePrice /= count;
            avgSold.ROIOnSoldPrice /= count;
            avgSold.ADOM /= count;

            avgBought.Beds /= count;
            avgBought.FullBaths /= count;
            avgBought.HalfBaths /= count;
            avgBought.PoolValue /= count;
            avgBought.YearBuilt /= count;
            avgBought.SPSqFT /= count;
            avgBought.SqFtHeated /= count;
            //avgBought.PPSqFt /= count;
            avgBought.Price /= count;
            avgBought.ADOM /= count;

            // Round the values
            rawAvgSPSqFT = avgSold.SPSqFT;
            avgSold.Price = Math.Round(avgSold.Price, 2);
            avgSold.SqFtHeated = Math.Round(avgSold.SqFtHeated);
            avgSold.Beds = Math.Round(avgSold.Beds);
            avgSold.FullBaths = Math.Round(avgSold.FullBaths);
            avgSold.HalfBaths = Math.Round(avgSold.HalfBaths);
            avgSold.PoolValue = Math.Round(avgSold.PoolValue);
            avgSold.YearBuilt = Math.Round(avgSold.YearBuilt);
            avgSold.SPSqFT = Math.Round(rawAvgSPSqFT, 2);
            //avgSold.PPSqFt = Math.Round(avgSold.PPSqFt, 2);
            avgSold.Profit = Math.Round(avgSold.Profit);
            avgSold.ROIOnPurchasePrice = Math.Round(avgSold.ROIOnPurchasePrice);
            avgSold.ROIOnSoldPrice = Math.Round(avgSold.ROIOnSoldPrice);
            avgSold.ADOM = Math.Round(avgSold.ADOM);

            rawAvgPrice = avgBought.Price;
            avgBought.Price = Math.Round(rawAvgPrice);
            avgBought.SqFtHeated = Math.Round(avgBought.SqFtHeated);
            avgBought.Beds = Math.Round(avgBought.Beds);
            avgBought.FullBaths = Math.Round(avgBought.FullBaths);
            avgBought.HalfBaths = Math.Round(avgBought.HalfBaths);
            avgBought.PoolValue = Math.Round(avgBought.PoolValue);
            avgBought.YearBuilt = Math.Round(avgBought.YearBuilt);
            avgBought.SPSqFT = Math.Round(avgBought.SPSqFT, 2);
            avgBought.PPSqFt = Math.Round(avgBought.PPSqFt, 2);
            avgBought.ADOM = Math.Round(avgBought.ADOM);

            homes.AverageBeds = avgBought.Beds.ToString();
            if (avgBought.Beds != avgSold.Beds)
            {
                homes.AverageBeds += "/" + avgSold.Beds;
            }
            homes.AverageFullBaths = avgBought.FullBaths.ToString();
            if (avgBought.FullBaths != avgSold.FullBaths)
            {
                homes.AverageFullBaths += "/" + avgSold.FullBaths;
            }
            homes.AverageHalfBaths = avgBought.HalfBaths.ToString();
            if (avgBought.HalfBaths != avgSold.HalfBaths)
            {
                homes.AverageHalfBaths += "/" + avgSold.HalfBaths;
            }
            homes.AverageYearBuilt = avgSold.YearBuilt;
            homes.AveragePurchase = avgBought.PriceStr;
            homes.AverageSoldPrice = avgSold.PriceStr;
            homes.AverageSqFtHeated = avgBought.SqFtHeated.ToString();
            if (avgBought.SqFtHeated != avgSold.SqFtHeated)
            {
                homes.AverageSqFtHeated += "/" + avgSold.SqFtHeated;
            }
            homes.AverageProfit = avgSold.ProfitStr;
            homes.AverageADOM = /*avgBought.ADOM + "/" + */avgSold.ADOM.ToString();
            homes.AveragePPSqFt = "$" + avgBought.SPSqFT.ToString();
            homes.AverageSPSqFT = "$" + avgSold.SPSqFT.ToString();
            homes.AverageROI = /*avgSold.ROIOnPurchasePrice + "%/" + */avgSold.ROIOnSoldPrice + "%";
        }

        private static void AccumulateValues(
            FlippedHouse f,
            HouseCharacteristics avgBought,
            HouseCharacteristics avgSold,
            Listing bought,
            Listing sold,
            double proftNum,
            double boughtROI,
            double soldROI)
        {
            // Accummulate the numeric values we care to show as summary on Flips > Zips page
            avgBought.Beds += bought.BedsValue();
            avgBought.FullBaths += bought.FullBathsValue();
            avgBought.HalfBaths += bought.HalfBathsValue();
            avgBought.YearBuilt += bought.YearBuiltValue();
            avgBought.SqFtHeated += bought.SqFtHeatedValue();
            //avgBought.PPSqFt += bought.SPSqFtValue();
            avgBought.SPSqFT += bought.SPSqFtValue();
            avgBought.Price += bought.CurrentPriceValue();
            avgBought.PoolValue += bought.PoolValue();
            avgBought.ADOM += bought.ADOMValue();
            if (sold != null)
            {
                // Accummulate the numeric values we care to show as summary on Flips > Zips page
                avgSold.Price += sold.CurrentPriceValue();
                avgSold.SqFtHeated += sold.SqFtHeatedValue();
                avgSold.Beds += sold.BedsValue();
                avgSold.FullBaths += sold.FullBathsValue();
                avgSold.HalfBaths += sold.HalfBathsValue();
                avgSold.PoolValue += sold.PoolValue();
                avgSold.YearBuilt += sold.YearBuiltValue();
                //avgSold.PPSqFt += sold.SPSqFtValue();
                avgSold.SPSqFT += sold.SPSqFtValue();
                avgSold.Profit += proftNum;
                avgSold.ROIOnPurchasePrice += boughtROI;
                avgSold.ROIOnSoldPrice += soldROI;
                avgSold.ADOM += sold.ADOMValue();
            }
        }

        private static void SetAdditionalValues(FlippedHouse f, Listing bought, Listing sold, out double proftNum, out double boughtROI, out double soldROI)
        {
            f.SoldPrice = sold.CurrentPrice;
            if (sold.SqFtHeated.Trim().ToLower() != f.SqFtHeated.Trim().ToLower())
            {
                f.SqFtHeated += "/" + sold.SqFtHeated;
            }

            if (sold.Beds.Trim().ToLower() != f.Beds.Trim().ToLower())
            {
                f.Beds += "/" + sold.Beds;
            }

            if (sold.FullBaths.Trim().ToLower() != f.FullBaths.Trim().ToLower())
            {
                f.FullBaths += "/" + sold.FullBaths;
            }

            if (sold.HalfBaths.Trim().ToLower() != f.HalfBaths.Trim().ToLower())
            {
                f.HalfBaths += "/" + sold.HalfBaths;
            }

            if (sold.Pool.Trim().ToLower() != f.Pool.Trim().ToLower())
            {
                f.Pool += "/" + sold.Pool;
            }

            f.SPSqFt = sold.SPSqFt;
            proftNum = sold.CurrentPriceValue() - bought.CurrentPriceValue();
            f.Profit = FlippedCharacteristics.ToDollars(proftNum);

            boughtROI = Math.Round(proftNum / bought.CurrentPriceValue(), 2) * 100;
            soldROI = Math.Round(proftNum / sold.CurrentPriceValue(), 2) * 100;
            f.ROI = boughtROI + "%/" + soldROI + "%";

            //var soldPrice = sold.CurrentPriceValue();
            //var boughtPrice = bought.CurrentPriceValue();
            //var ratio = Math.Round(soldPrice / boughtPrice, 1);
            //f.SPoverPP = ratio + "x";

            f.ADOM = sold.ADOM;
        }

        private static void SetValues(FlippedHouse f, Listing bought)
        {
            f.Subdivision = bought.LegalSubdivisionName;
            f.Beds = bought.Beds;
            f.FullBaths = bought.FullBaths;
            f.HalfBaths = bought.HalfBaths;
            f.Pool = bought.Pool;
            f.YearBuilt = bought.YearBuilt;
            f.SqFtHeated = bought.SqFtHeated;
            f.PPSqFt = bought.SPSqFt;
            f.PurchasePrice = bought.CurrentPrice;
            f.Date = f.PurchaseDate + " " + f.SoldDate;
        }

        private static void GetBoughtSold(List<Listing> list, out Listing bought, out Listing sold)
        {
            list.OrderBy(x => x.CloseDateValue()).ToList();
            bought = null;
            sold = null;

            if (list.Count >= 1)
            {
                var firstSold = list[0];
                bought = firstSold;
            }
            if (list.Count >= 2)
            {
                var lastSold = list[list.Count - 1];
                sold = lastSold;
            }
        }

        private static int RoundDown(int x)
        {
            if ((x % 10) == 0) return x;
            var val = (x / 10) * 10;
            return val;
        }

        private static int RoundUp(int x)
        {
            if ((x % 10) == 0) return x;
            var val = ((x / 10) + 1) * 10;
            return val;
        }

        private static void AddData(Dictionary<string, double> stats, string key, double val)
        {
            if (stats.ContainsKey(key))
            {
                stats[key] += val;
            }
            else
            {
                stats.Add(key, val);
            }
        }

        private static Dictionary<string, Dictionary<FlipStatsType, Dictionary<string, double>>> zipStats =
            new Dictionary<string, Dictionary<FlipStatsType, Dictionary<string, double>>>(StringComparer.OrdinalIgnoreCase);

        private static Dictionary<string, double> GetStats(string zip, FlipStatsType which)
        {
            Dictionary<FlipStatsType, Dictionary<string, double>> stats;
            if (zipStats.ContainsKey(zip))
            {
                stats = zipStats[zip];
            }
            else
            {
                stats = new Dictionary<FlipStatsType, Dictionary<string, double>>();
                zipStats.Add(zip, stats);
            }

            Dictionary<string, double> dimensions;
            if (stats.ContainsKey(which))
            {
                dimensions = stats[which];
            }
            else
            {
                dimensions = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
                stats.Add(which, dimensions);
            }

            return dimensions;
        }

        private static Lazy<List<FlippedCharacteristics>> flipByZipNoPool =
            new Lazy<List<FlippedCharacteristics>>(
                () =>
                {
                    return InitFlipByZipNoPool();
                });

        private static Lazy<List<FlippedCharacteristics>> flipBySubDiv =
            new Lazy<List<FlippedCharacteristics>>(
                () =>
                {
                    return InitFlipBySubDiv();
                });
        private static Lazy<List<FlippedCharacteristics>> flipByCommonDiv =
            new Lazy<List<FlippedCharacteristics>>(
                () =>
                {
                    return InitFlipByCommonDiv();
                });

        public static List<FlippedHouse> FlippedHouses { get { return flippedHouses.Value; } }

        public static List<FlippedCharacteristics> FlipByZip { get { return flipByZip.Value; } }

        public static List<FlippedCharacteristics> FlipByZipNoPool { get { return flipByZipNoPool.Value; } }

        public static List<FlippedCharacteristics> FlipBySubDivision { get { return flipBySubDiv.Value; } }

        public static List<FlippedCharacteristics> FlipByCommonSubDivision { get { return flipByCommonDiv.Value; } }

        // GET: Flips
        public ActionResult Index()
        {
            //foreach(var item in flippedHouses)
            //{
            //    var list = (from el in db.Listings
            //               where el.Address.Equals(item.Address) && 
            //                     el.City.Equals(item.City) &&
            //                     el.PostalCode.Equals(item.PostalCode) &&
            //                     item.PurchaseDate.Equals(el.CloseDate)
            //               select el).ToList();
            //    var found = list[0];
            //    item.SqFtHeated = found.SqFtHeated;
            //    item.LPSqFt = found.SPSqFt;


            //    list = (from el in db.Listings
            //                where el.Address.Equals(item.Address) &&
            //                      el.City.Equals(item.City) &&
            //                      el.PostalCode.Equals(item.PostalCode) &&
            //                      item.SoldDate.Equals(el.CloseDate)
            //                select el).ToList();
            //    found = list[0];
            //    item.SPSqFt = found.SPSqFt;
            //}
            //json = JsonConvert.SerializeObject(flippedHouses);
            //using (var sw = new StreamWriter(path))
            //{
            //    sw.Write(json);
            //}
            return View(flippedHouses.Value.OrderBy(x => x.City).ThenByDescending(x => x.Profit));
        }

        public ActionResult FlippedHomes(
            string id,
            string sortOrder = null,
            string filter = null)
        {
            return Homes(id: id, sortOrder: sortOrder, filter: filter);
        }
        /*
        [ActionName("SubdivHomes")]
        public ActionResult Homes(
            string zip, //same as id
            string subdiv, // new parameter
            bool active //same as filter
            )
        {
            if (active)
            {
                var zipLookup = flipZips.Value.Flips;
                if (string.IsNullOrWhiteSpace(zip) || !zipLookup.ContainsKey(zip))
                {
                    return View(new Homes());
                }

                var subdivs = zipLookup[zip].Subdivisions;
                if (subdivs.Lookup.ContainsKey(subdiv))
                {
                    var div = subdivs.Lookup[subdiv];
                    return View(div.Active);
                }
                else
                {
                    return View(new Homes());
                }
            }
            else
            {
                throw new InvalidOperationException("Error: Active==false unhandled!");
            }
        }

        public ActionResult Homes(
            string id, //same as id
            string subdiv=null, // new parameter
            string sortOrder = null, // View telling controller: new way to be sorted
            string filter = null
            )
        {
            var tokens = filter.Split(new char[] { ',' });
            bool active = false;
            foreach(var t in tokens)
            {
                if ("active".Equals(t))
                {
                    active = true;
                }
                if (filter.StartsWith("subdiv:"))
                {
                    var filterKey = "subdiv:";
                    var filterValue = GetFilterValue(filter, filterKey);
                    subdiv = filterValue.Trim();
                }
            }
            if (active)
            {
                var zipLookup = flipZips.Value.Flips;
                if (string.IsNullOrWhiteSpace(id) || !zipLookup.ContainsKey(id))
                {
                    return View(new Homes());
                }

                var subdivs = zipLookup[id].Subdivisions;
                if (subdivs.Lookup.ContainsKey(subdiv))
                {
                    var div = subdivs.Lookup[subdiv];
                    return View(div.Active);
                }
                else
                {
                    return View(new Homes());
                }
            }
            else
            {
                throw new InvalidOperationException("Error: Active==false unhandled!");
            }
        }
        */
        public ActionResult Homes(
            string id,
            string subdiv = null,
            string sortOrder = null, // View telling controller: new way to be sorted
            string filter = null)
        {
            sortOrder = String.IsNullOrEmpty(sortOrder) ? "Address" : sortOrder;

            if (string.IsNullOrEmpty(filter))
            {
                if (string.IsNullOrWhiteSpace(id) || !flipZips.Value.Flips.ContainsKey(id))
                {
                    return View(new Homes());
                }
                //return View(flipHomesInZip.Value[id].OrderBy(x => x.Address));
                var homes = flipZips.Value.Flips[id];
                //IEnumerable<FlippedHouse> list =
                //    Sort(homes, currentSort, currentSortDirection, newSort);
                //return View(new Homes(list) { Zipcode = homes.Zipcode });

                flipZips.Value.Flips[id] = Sort(homes, sortOrder);
                return View(flipZips.Value.Flips[id]);
            }
            else
            {
                var filterKey = "subdiv:";
                if (filter.StartsWith(filterKey))
                {
                    var zipLookup = flipZips.Value.Flips;
                    if (string.IsNullOrWhiteSpace(id) || !zipLookup.ContainsKey(id))
                    {
                        return View(new Homes());
                    }

                    var filterValue = GetFilterValue(filter, filterKey);
                    var filterSubDiv = filterValue.Trim();
                    var subdivs = zipLookup[id].Subdivisions;
                    if (subdivs.Lookup.ContainsKey(filterSubDiv))
                    {
                        var div = subdivs.Lookup[filterSubDiv];
                        return View(div);
                    }
                    else
                    {
                        return View(new Homes());
                    }
                }
                else if ("active".Equals(filter))
                {
                    if (string.IsNullOrWhiteSpace(subdiv))
                    {
                        if (flipZips.Value.Flips.ContainsKey(id))
                        {
                            var homes = flipZips.Value.Flips[id].Active;
                            //return View(new Homes(homes.OrderBy(x => x.Address)) { Zipcode = id });
                            flipZips.Value.Flips[id].Active = Sort(homes, sortOrder);
                            return View(flipZips.Value.Flips[id].Active);
                        }
                        else
                        {
                            return View(new Homes());
                        }
                    }
                    else
                    {
                        var zip = id;
                        var zipLookup = flipZips.Value.Flips;
                        if (string.IsNullOrWhiteSpace(zip) || !zipLookup.ContainsKey(zip))
                        {
                            return View(new Homes());
                        }

                        var subdivs = zipLookup[zip].Subdivisions;
                        if (subdivs.Lookup.ContainsKey(subdiv))
                        {
                            var div = subdivs.Lookup[subdiv];
                            return View(div.Active);
                        }
                        else
                        {
                            return View(new Homes());
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format("Error: Unknown filter '{0}", filter));
                }
            }
        }

        private static string GetFilterValue(string filter, string filterKey)
        {
            var start = filterKey.Length;
            var filterValue = filter.Substring(start);
            return filterValue;
        }

        private Homes Sort(
            Homes homes,
            string newSort)
        {
            // Figure out which direction we are going to sort
            string currentSort = homes.SortBy;
            SortDirection currentDirection = homes.SortDirection;
            SortDirection newDirection;
            if (!newSort.Equals(currentSort)) // if we sort a new column, default to ascending
            {
                newDirection = SortDirection.Ascending;
            }
            else // however, if we sorting again on the same column, change direction
            {
                newDirection =
                    (SortDirection)Math.Abs((int)currentDirection - 1);
            }

            // Now use sort direction
            homes.SortBy = newSort;
            homes.SortDirection = newDirection;
            IEnumerable<Listing> list;
            if (newDirection == SortDirection.Ascending)
            {
                list = homes.OrderBy(m => m, new HomeComparer(newSort));
            }
            else
            {
                list = homes.OrderByDescending(m => m, new HomeComparer(newSort));
            }
            return new Homes(list, homes.Zipcode)
            {
                SortBy = homes.SortBy,
                SortDirection = homes.SortDirection,
                Zipcode = homes.Zipcode,
                Subdivision = homes.Subdivision,
                AverageSold = homes.AverageSold,
                AverageBought = homes.AverageBought,
                AverageBeds = homes.AverageBeds,
                AverageFullBaths = homes.AverageFullBaths,
                AverageHalfBaths = homes.AverageHalfBaths,
                AverageYearBuilt = homes.AverageYearBuilt,
                AveragePurchase = homes.AveragePurchase,
                AverageSoldPrice = homes.AverageSoldPrice,
                AverageSqFtHeated = homes.AverageSqFtHeated,
                AverageProfit = homes.AverageProfit,
                AverageADOM = homes.AverageADOM,
                AveragePPSqFt = homes.AveragePPSqFt,
                AverageSPSqFT = homes.AverageSPSqFT,
                AverageROI = homes.AverageROI,
                Active = homes.Active,
                Subdivisions = homes.Subdivisions
            };
            //homes.SetList(list);
            //return homes;
        }

        private IEnumerable<Listing> Sort(
            Homes homes,
            string currentSort, // View telling controller: how we currently sorted
            SortDirection currentSortDirection, /* View telling controller: what's our sort direction*/
            string newSort /* View telling controller: new way to be sorted*/)
        {
            // Figure out which direction we are going to sort

            if (!newSort.Equals(currentSort)) // if we sort a new column, default to ascending
            {
                ViewBag.CurrentSortDirection = SortDirection.Ascending;
            }
            else // however, if we sorting again on the same column, change direction
            {
                ViewBag.CurrentSortDirection =
                    (SortDirection)Math.Abs((int)currentSortDirection - 1);
            }

            // Now use sort direction
            ViewBag.CurrentSort = newSort;
            IEnumerable<Listing> list;
            if (ViewBag.CurrentSortDirection == SortDirection.Ascending)
            {
                list = homes.OrderBy(m => m, new HomeComparer(newSort));
            }
            else
            {
                list = homes.OrderByDescending(m => m, new HomeComparer(newSort));
            }
            return list;
        }

        public ActionResult ByZip(string sortOrder = null)
        {
            //foreach (var item in flippedHouses)
            //{
            //    item.RoiPurchase = Math.Round(item.ProfitValue() / item.PreFlip.Price, 2);
            //    item.RoiSold = Math.Round(item.ProfitValue() / item.PostFlip.Price, 2);
            //}
            //json = JsonConvert.SerializeObject(flippedHouses);
            //using(var sw = new StreamWriter(path))
            //{
            //    sw.Write(json);
            //} 

            /*
            var list =
            from f in flipByZip.Value
            select new FlippedCharacteristics()
            {
                Zipcode = f.Zipcode,
                Houses = flipHomesInZip.Value[f.Zipcode],
                PreFlip = f.PreFlip,
                PostFlip = f.PostFlip,
                City = f.City,
                Aggregation = f.Aggregation,
                MaxSqft = f.MaxSqft,
                MaxYear = f.MaxYear,
                MinSqft = f.MinSqft,
                MinYear = f.MinYear,
                RoiPurchase = f.RoiPurchase,
                RoiSold = f.RoiSold,
                SubDivision = f.SubDivision
            };
            return View(list.OrderBy(x => x.Zipcode).ThenByDescending(x => x.Profit));
            */
            sortOrder = String.IsNullOrEmpty(sortOrder) ? "Zipcode" : sortOrder;
            //var list = new List<Homes>();
            //foreach (var zip in flipHomesInZip.Value.Keys)
            //{
            //    var homes = flipHomesInZip.Value[zip];
            //    list.Add(homes);
            //}
            var zips = Sort(flipZips.Value, sortOrder);
            return View(zips);
        }

        private Zips Sort(Zips zips, string newSort)
        {
            // Figure out which direction we are going to sort
            string currentSort = zips.SortBy;
            SortDirection currentDirection = zips.SortDirection;
            SortDirection newDirection;
            if (!newSort.Equals(currentSort)) // if we sort a new column, default to ascending
            {
                newDirection = SortDirection.Ascending;
            }
            else // however, if we sorting again on the same column, change direction
            {
                newDirection =
                    (SortDirection)Math.Abs((int)currentDirection - 1);
            }

            // Now use sort direction
            zips.SortBy = newSort;
            zips.SortDirection = newDirection;
            IEnumerable<Homes> list;
            if (newDirection == SortDirection.Ascending)
            {
                list = zips.OrderBy(m => m, new ZipComparer(newSort));
            }
            else
            {
                list = zips.OrderByDescending(m => m, new ZipComparer(newSort));
            }
            return new Zips(list) { SortBy = zips.SortBy, SortDirection = zips.SortDirection, Flips = zips.Flips };
            //zips.SetList(list);
            return zips;
        }

        public ActionResult ByZipNoPool()
        {
            foreach (var item in flipByZipNoPool.Value)
            {
                item.RoiPurchase = Math.Round(item.ProfitValue() / item.PreFlip.Price, 2);
                item.RoiSold = Math.Round(item.ProfitValue() / item.PostFlip.Price, 2);
            }
            //json = JsonConvert.SerializeObject(flippedHouses);
            //using(var sw = new StreamWriter(path))
            //{
            //    sw.Write(json);
            //}            
            return View(flipByZipNoPool.Value.OrderBy(x => x.Zipcode).ThenByDescending(x => x.Profit));
        }

        public ActionResult BySubDivision()
        {
            //foreach (var item in flippedHouses)
            //{
            //    item.RoiPurchase = Math.Round(item.ProfitValue() / item.PreFlip.Price, 2);
            //    item.RoiSold = Math.Round(item.ProfitValue() / item.PostFlip.Price, 2);
            //}
            //json = JsonConvert.SerializeObject(flippedHouses);
            //using (var sw = new StreamWriter(path))
            //{
            //    sw.Write(json);
            //}
            return View(flipBySubDiv.Value.OrderBy(x => x.SubDivision).ThenByDescending(x => x.Profit));
        }

        public ActionResult Subdivision(string id)
        {
            if (!string.IsNullOrWhiteSpace(id) && flipZips.Value.Flips.ContainsKey(id))
            {
                var subdivs = flipZips.Value.Flips[id].Subdivisions.OrderBy(x => x.Subdivision);

                /*
                var zipLookup = zipSubdivFlippedHomes;
                var subdivLookup = zipLookup.Value[id];
                if (!subdivLookup.ContainsKey(subdiv))
                {
                    return View(new Homes());
                }

                var homes = subdivLookup[subdiv];
                */

                return View(new Subdivisions(subdivs) { Zipcode = id });

                //return View(new Homes(subdivs.OrderBy(x => x.Address)) { Zipcode = id });
                //flipZips.Value.Flips[id].Active = Sort(homes, sortOrder);
                //return View(flipZips.Value.Flips[id].Active);
            }
            else
            {
                return View(new Subdivisions());
            }

        }

        //public ActionResult Subdivision()
        //{
        //    var zipLookup = subDivFlippedHomes.Value;
        //    var list = new List<FlippedCharacteristics>();
        //    foreach (var zip in zipLookup.Keys)
        //    {
        //        var subdivLookup = zipLookup[zip];
        //        foreach (var subdiv in subdivLookup.Keys)
        //        {
        //            var fc = new FlippedCharacteristics();
        //            list.Add(fc);
        //            fc.Zipcode = zip;
        //            fc.SubDivision = subdiv;
        //            var homes = subdivLookup[subdiv];
        //            fc.Houses = homes;
        //        }
        //    }
        //    return View(list.OrderBy(x => x.Zipcode).ThenByDescending(x => x.SubDivision));
        //}

        public ActionResult ByCommonSubDivision()
        {
            //foreach (var item in flippedHouses)
            //{
            //    item.RoiPurchase = Math.Round(item.ProfitValue() / item.PreFlip.Price, 2);
            //    item.RoiSold = Math.Round(item.ProfitValue() / item.PostFlip.Price, 2);
            //}
            //json = JsonConvert.SerializeObject(flippedHouses);
            //using (var sw = new StreamWriter(path))
            //{
            //    sw.Write(json);
            //}
            return View(flipByCommonDiv.Value.OrderBy(x => x.SubDivision).ThenByDescending(x => x.Profit));
        }

        // GET: Listings/History/5
        public ActionResult History(string address, string city, string zip)
        {
            if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(zip))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //MlsRow mlsRow = db.Listings.Find(id);
            //if (mlsRow == null)
            //{
            //    return HttpNotFound();
            //}

            var list = (from el in db.Listings
                        where address.Equals(el.Address) && city.Equals(el.City) && zip.Equals(el.PostalCode)
                        select el).ToList();

            if (list.Count == 0)
            {
                return HttpNotFound();
            }
            return View(list);
        }

        private static List<FlippedCharacteristics> InitFlipBySubDiv()
        {
            string path = HostingEnvironment.MapPath(
                string.Format("{0}/{1}", dataFolder, byDivFile));

            string json = null;
            using (var sr = new StreamReader(path))
            {
                json = sr.ReadToEnd();
            }

            var list = JsonConvert.DeserializeObject<List<FlippedCharacteristics>>(json);
            return list;
        }

        private static List<FlippedCharacteristics> InitFlipByCommonDiv()
        {
            string path = HostingEnvironment.MapPath(
               string.Format("{0}/{1}", dataFolder, byCommDivFile));

            string json = null;
            using (var sr = new StreamReader(path))
            {
                json = sr.ReadToEnd();
            }

            var list = JsonConvert.DeserializeObject<List<FlippedCharacteristics>>(json);

            return list;
        }

        private static List<FlippedHouse> InitFlippedHouses()
        {
            string path = HostingEnvironment.MapPath(
                        string.Format("{0}/{1}", dataFolder, file));

            string json = null;
            using (var sr = new StreamReader(path))
            {
                json = sr.ReadToEnd();
            }

            //var controller = new ListingsController();
            var list = JsonConvert.DeserializeObject<List<FlippedHouse>>(json);
            //foreach(var f in list)
            //{
            //    controller.Search(f);
            //}
            return list;
        }

        private static List<FlippedCharacteristics> InitFlipByZip()
        {
            string path = HostingEnvironment.MapPath(
                string.Format("{0}/{1}", dataFolder, byZipFile));

            string json = null;
            using (var sr = new StreamReader(path))
            {
                json = sr.ReadToEnd();
            }

            var list = JsonConvert.DeserializeObject<List<FlippedCharacteristics>>(json);
            return list;

        }

        private static List<FlippedCharacteristics> InitFlipByZipNoPool()
        {
            string path = HostingEnvironment.MapPath(
                string.Format("{0}/{1}", dataFolder, byZipNoPool));

            string json = null;
            using (var sr = new StreamReader(path))
            {
                json = sr.ReadToEnd();
            }

            var list = JsonConvert.DeserializeObject<List<FlippedCharacteristics>>(json);
            return list;

        }
    }
}