using Hack.HouseFlipper.DataAccess.Csv;
using Hack.HouseFlipper.DataAccess.DB;
using Hack.HouseFlipper.DataAccess.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility
{
    public class Program
    {
        private static MlsContext db = new MlsContext();

        public static void Main(string[] args)
        {
            //string dataFolder = 
            //@"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\data";

            if (args != null && args.Length > 0)
            {
                int i = 0;
                var cmdName = args[i++].ToLower();
                if (cmdName.StartsWith("-"))
                {
                    cmdName = cmdName.Substring(1);
                }
                args = args.Skip(1).ToArray();
                switch (cmdName)
                {
                    case "import":
                        new ImportCommand().Execute(args);
                        break;
                    case "clear":
                        new ClearCommand().Execute(args);
                        break;
                    case "count":
                        new CountCommand().Execute(args);
                        break;
                    case "setupsite":
                        new SetupSiteCommand().Execute(args);
                        break;
                    case "setuptestsite":
                        new SetupTestSiteCommand().Execute(args);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown command " + cmdName);
                }
            }

            //DoOtherStuff();
        }

        private static void DoOtherStuff()
        {
            Dictionary<string, Dictionary<string, Dictionary<string, List<Listing>>>> flippedMultiKeyHash = null, activeMultiKeyHash = null;
            Dictionary<string, FlippedCharacteristics> indivSubDivAllResults = null, indivSubDivNoPoolResults = null, indivSubDivWithPoolResults = null;
            //<TESTING>
            //Process(out flippedMultiKeyHash, out indivSubDivAllResults, out indivSubDivNoPoolResults, out indivSubDivWithPoolResults, out activeMultiKeyHash);

            var zipResultsAll = new Dictionary<string, FlippedCharacteristics>();
            var zipResultsNoPool = new Dictionary<string, FlippedCharacteristics>();
            var zipResultsWithPool = new Dictionary<string, FlippedCharacteristics>();


            List<FlippedHouse> flippedHouses = new List<FlippedHouse>();
            List<FlippedCharacteristics> flipbyzipAll = new List<FlippedCharacteristics>();
            List<FlippedCharacteristics> flipbyzipNoPool = new List<FlippedCharacteristics>();
            List<FlippedCharacteristics> flipbyzipWithPool = new List<FlippedCharacteristics>();
            List<FlippedCharacteristics> flipbydivAll = new List<FlippedCharacteristics>();
            List<FlippedCharacteristics> flipbydivNoPool = new List<FlippedCharacteristics>();
            List<FlippedCharacteristics> flipbydivWithPool = new List<FlippedCharacteristics>();
            foreach (var zip in flippedMultiKeyHash.Keys)
            {
                var subDivHash = flippedMultiKeyHash[zip];

                var preFlipAll = new HouseCharacteristics() { Price = 0 };
                var postFlipAll = new HouseCharacteristics() { Price = 0 };

                var preFlipNoPool = new HouseCharacteristics() { Price = 0 };
                var postFlipNoPool = new HouseCharacteristics() { Price = 0 };

                var preFlipWithPool = new HouseCharacteristics() { Price = 0 };
                var postFlipWithPool = new HouseCharacteristics() { Price = 0 };

                double zipHousesCount = 0, zipWithPool = 0, zipNoPool = 0;
                var subDivCount = 0;
                foreach (var subDiv in subDivHash.Keys)
                {
                    ++subDivCount;
                    var houseHash = subDivHash[subDiv];
                    var numHouses = houseHash.Keys.Count;
                    var numWithPool = 0;
                    var numNoPool = 0;
                    zipHousesCount += numHouses;

                    HouseCharacteristics preSubDivFlipAll = new HouseCharacteristics() { Price = 0 };
                    HouseCharacteristics postSubDivFlipAll = new HouseCharacteristics() { Price = 0 };

                    HouseCharacteristics preSubDivFlipNoPool = new HouseCharacteristics() { Price = 0 };
                    HouseCharacteristics postSubDivFlipNoPool = new HouseCharacteristics() { Price = 0 };

                    HouseCharacteristics preSubDivFlipWithPool = new HouseCharacteristics() { Price = 0 };
                    HouseCharacteristics postSubDivFlipWithPool = new HouseCharacteristics() { Price = 0 };
                    var city = string.Empty;
                    var houseCount = 0;
                    foreach (var houseId in houseHash.Keys)
                    {
                        ++houseCount;
                        List<Listing> soldRecords = GetSortedSoldRecords(houseHash, houseId);
                        Listing lastSold, firstSold;
                        FindFirstLastSold(soldRecords, out lastSold, out firstSold);
                        UpdateSoldRecords(soldRecords, lastSold, firstSold);

                        double firstPrice, lastPrice;
                        var pool = firstSold.Pool;
                        var hasPool = !string.IsNullOrWhiteSpace(pool) && pool.ToLower().Trim().Equals("private");
                        if (hasPool)
                        {
                            zipWithPool++;
                            numWithPool++;
                            AddToSum(preFlipWithPool, postFlipWithPool, preSubDivFlipWithPool, postSubDivFlipWithPool, lastSold, firstSold, out firstPrice, out lastPrice);
                        }
                        else
                        {
                            zipNoPool++;
                            numNoPool++;
                            AddToSum(preFlipNoPool, postFlipNoPool, preSubDivFlipNoPool, postSubDivFlipNoPool, lastSold, firstSold, out firstPrice, out lastPrice);
                        }

                        AddToSum(preFlipAll, postFlipAll, preSubDivFlipAll, postSubDivFlipAll, lastSold, firstSold, out firstPrice, out lastPrice);

                        double profit = DetermineProfit(firstPrice, lastPrice);
                        var flippedHouse = PrintHouseFlip(zip, houseCount, lastSold, firstSold, profit);

                        /** Add to JSON the Flipped House record **/
                        flippedHouses.Add(flippedHouse);
                        city = lastSold.City;
                    }

                    var results = indivSubDivNoPoolResults;
                    var pre = preSubDivFlipNoPool;
                    var post = postSubDivFlipNoPool;
                    var count = numNoPool;
                    var extra = "With No Pool";
                    var fcList = flipbydivNoPool;
                    if (count > 0)
                    {
                        AddAverageFlipBySubDivision(zip, subDiv, city, results, pre, post, count, extra, fcList);
                    }

                    results = indivSubDivWithPoolResults;
                    pre = preSubDivFlipWithPool;
                    post = postSubDivFlipWithPool;
                    count = numWithPool;
                    extra = "With Pool";
                    fcList = flipbydivWithPool;
                    if (count > 0)
                    {
                        AddAverageFlipBySubDivision(zip, subDiv, city, results, pre, post, count, extra, fcList);
                    }

                    results = indivSubDivAllResults;
                    pre = preSubDivFlipAll;
                    post = postSubDivFlipAll;
                    count = numHouses;
                    extra = "All";
                    fcList = flipbydivAll;
                    if (count > 0)
                    {
                        AddAverageFlipBySubDivision(zip, subDiv, city, results, pre, post, count, extra, fcList);
                    }
                }

                {
                    /** CALCULATE AVG FLIP PER ZIP HERE **/
                    var results = zipResultsNoPool;
                    var pre = preFlipNoPool;
                    var post = postFlipNoPool;
                    var count = zipNoPool;
                    var extra = "With No Pool";
                    var fcList = flipbyzipNoPool;
                    if (count > 0)
                    {
                        var fc2 = PrintAverageFlipInZip(results, zip, pre, post, count, extra);
                        fcList.Add(fc2);
                    }

                    results = zipResultsWithPool;
                    pre = preFlipWithPool;
                    post = postFlipWithPool;
                    count = zipWithPool;
                    extra = "With Pool";
                    fcList = flipbyzipWithPool;
                    if (count > 0)
                    {
                        var fc2 = PrintAverageFlipInZip(results, zip, pre, post, count, extra);
                        fcList.Add(fc2);
                    }

                    results = zipResultsAll;
                    pre = preFlipAll;
                    post = postFlipAll;
                    count = zipHousesCount;
                    extra = "With Pool";
                    fcList = flipbyzipAll;
                    if (count > 0)
                    {
                        var fc2 = PrintAverageFlipInZip(results, zip, pre, post, count, extra);
                        fcList.Add(fc2);
                    }
                }
            }

            /** Common Subdivisions can be further grouped together **/
            var commonSubDivNoPoolResults = PrintSubDivAlphabetical(indivSubDivNoPoolResults);
            var activeZipHousesNoPool = new Dictionary<string, List<Listing>>();
            var activeSubDivHousesNoPool = new Dictionary<string, List<Listing>>();
            var activeCommonDivHousesNoPool = new Dictionary<string, List<Listing>>();
            PrintActiveWithinZip(
                activeZipHousesNoPool,
                activeSubDivHousesNoPool,
                activeCommonDivHousesNoPool,
                zipResultsNoPool,
                indivSubDivNoPoolResults,
                commonSubDivNoPoolResults,
                activeMultiKeyHash);


            var commonSubDivWithPoolResults = PrintSubDivAlphabetical(indivSubDivWithPoolResults);
            var activeZipHousesWithPool = new Dictionary<string, List<Listing>>();
            var activeSubDivHousesWithPool = new Dictionary<string, List<Listing>>();
            var activeCommonDivHousesWithPool = new Dictionary<string, List<Listing>>();
            PrintActiveWithinZip(
                activeZipHousesWithPool,
                activeSubDivHousesWithPool,
                activeCommonDivHousesWithPool,
                zipResultsWithPool,
                indivSubDivWithPoolResults,
                commonSubDivWithPoolResults,
                activeMultiKeyHash);


            var commonSubDivAllResults = PrintSubDivAlphabetical(indivSubDivAllResults);
            var activeZipHousesAll = new Dictionary<string, List<Listing>>();
            var activeSubDivHousesAll = new Dictionary<string, List<Listing>>();
            var activeCommonDivHousesAll = new Dictionary<string, List<Listing>>();
            PrintActiveWithinZip(
                activeZipHousesAll,
                activeSubDivHousesAll,
                activeCommonDivHousesAll,
                zipResultsAll,
                indivSubDivAllResults,
                commonSubDivAllResults,
                activeMultiKeyHash);

            var json = JsonConvert.SerializeObject(flippedHouses);
            using (var sw = new StreamWriter("flippedhouses.json"))
            {
                sw.Write(json);
                sw.Flush();
            }
            json = JsonConvert.SerializeObject(flipbyzipAll);
            using (var sw = new StreamWriter("flipbyzipall.json"))
            {
                sw.Write(json);
                sw.Flush();
            }
            json = JsonConvert.SerializeObject(flipbyzipNoPool);
            using (var sw = new StreamWriter("flipbyzipNoPool.json"))
            {
                sw.Write(json);
                sw.Flush();
            }
            json = JsonConvert.SerializeObject(flipbyzipWithPool);
            using (var sw = new StreamWriter("flipbyzipWithPool.json"))
            {
                sw.Write(json);
                sw.Flush();
            }


            json = JsonConvert.SerializeObject(flipbydivAll);
            using (var sw = new StreamWriter("flipbydivall.json"))
            {
                sw.Write(json);
                sw.Flush();
            }
            json = JsonConvert.SerializeObject(flipbydivNoPool);
            using (var sw = new StreamWriter("flipbydivNoPool.json"))
            {
                sw.Write(json);
                sw.Flush();
            }
            json = JsonConvert.SerializeObject(flipbydivWithPool);
            using (var sw = new StreamWriter("flipbydivWithPool.json"))
            {
                sw.Write(json);
                sw.Flush();
            }





            json = JsonConvert.SerializeObject(commonSubDivAllResults.Values.ToList());
            using (var sw = new StreamWriter("flipbycommdivall.json"))
            {
                sw.Write(json);
                sw.Flush();
            }
            json = JsonConvert.SerializeObject(commonSubDivNoPoolResults.Values.ToList());
            using (var sw = new StreamWriter("flipbycommdivNoPool.json"))
            {
                sw.Write(json);
                sw.Flush();
            }
            json = JsonConvert.SerializeObject(commonSubDivWithPoolResults.Values.ToList());
            using (var sw = new StreamWriter("flipbycommdivWithPool.json"))
            {
                sw.Write(json);
                sw.Flush();
            }




            json = JsonConvert.SerializeObject(activeZipHousesAll.Values.ToList());
            using (var sw = new StreamWriter("activezipall.json"))
            {
                sw.Write(json);
                sw.Flush();
            }
            json = JsonConvert.SerializeObject(activeZipHousesNoPool.Values.ToList());
            using (var sw = new StreamWriter("activezipNoPool.json"))
            {
                sw.Write(json);
                sw.Flush();
            }
            json = JsonConvert.SerializeObject(activeZipHousesWithPool.Values.ToList());
            using (var sw = new StreamWriter("activezipWithPool.json"))
            {
                sw.Write(json);
                sw.Flush();
            }



            json = JsonConvert.SerializeObject(activeSubDivHousesAll.Values.ToList());
            using (var sw = new StreamWriter("activedivall.json"))
            {
                sw.Write(json);
                sw.Flush();
            }
            json = JsonConvert.SerializeObject(activeSubDivHousesNoPool.Values.ToList());
            using (var sw = new StreamWriter("activedivNoPool.json"))
            {
                sw.Write(json);
                sw.Flush();
            }
            json = JsonConvert.SerializeObject(activeSubDivHousesWithPool.Values.ToList());
            using (var sw = new StreamWriter("activedivWithPool.json"))
            {
                sw.Write(json);
                sw.Flush();
            }



            json = JsonConvert.SerializeObject(activeCommonDivHousesAll.Values.ToList());
            using (var sw = new StreamWriter("activecommdivall.json"))
            {
                sw.Write(json);
                sw.Flush();
            }
            json = JsonConvert.SerializeObject(activeCommonDivHousesNoPool.Values.ToList());
            using (var sw = new StreamWriter("activecommdivNoPool.json"))
            {
                sw.Write(json);
                sw.Flush();
            }
            json = JsonConvert.SerializeObject(activeCommonDivHousesWithPool.Values.ToList());
            using (var sw = new StreamWriter("activecommdivWithPool.json"))
            {
                sw.Write(json);
                sw.Flush();
            }




            json = JsonConvert.SerializeObject(db.Listings.ToList());
            using (var sw = new StreamWriter("listings.json"))
            {
                sw.Write(json);
                sw.Flush();
            }
        }


        private static void AddAverageFlipBySubDivision(string zip, string subDiv, string city, Dictionary<string, FlippedCharacteristics> results, HouseCharacteristics pre, HouseCharacteristics post, int count, string extra, List<FlippedCharacteristics> fcList)
        {
            var fc = ComputeAverageFlip(results, zip + "::" + subDiv, pre, post, count);
            fc.SubDivision = subDiv;
            fc.City = city;

            //Console.WriteLine("Subdivision Average price prior to flip: ${0}", ToString(preSubDivFlip.Price));
            var prePriceStr = ToString(pre.Price);
            var label = string.Format("Subdivision Preflip{0} Average", string.IsNullOrWhiteSpace(extra) ? string.Empty : " " + extra);
            Console.WriteLine("{0}:\r\n" +
                "Price: ${1}\r\n" +
                "Beds: {2}\r\n" +
                "Full Baths: {3}\r\n" +
                "Half Baths: {4}\r\n" +
                "SqFt Heated: {5}\r\n" +
                "Year Built: {6}",
                label,
                prePriceStr,
                pre.Beds,
                pre.FullBaths,
                pre.HalfBaths,
                pre.SqFtHeated,
                pre.YearBuilt);

            Console.WriteLine();
            //Console.WriteLine("Subdivision Average price after flip: ${0}", ToString(postSubDivFlip.Price));
            var postPriceStr = ToString(post.Price);
            label = string.Format("Subdivision Postflip{0} Average", string.IsNullOrWhiteSpace(extra) ? string.Empty : " " + extra);
            Console.WriteLine("{0}:\r\n" +
                "Price: ${1}\r\n" +
                "Beds: {2}\r\n" +
                "Full Baths: {3}\r\n" +
                "Half Baths: {4}\r\n" +
                "SqFt Heated: {5}\r\n" +
                "Year Built: {6}",
                label,
                postPriceStr,
                post.Beds,
                post.FullBaths,
                post.HalfBaths,
                post.SqFtHeated,
                post.YearBuilt);

            var profitStr = ToString(post.Price - pre.Price);
            Console.WriteLine("Net Profit: ${0}", profitStr);
            //fc.Profit = "$" + profitStr;
            //fc.PostFlip.PriceStr = "$" + postSubDivPriceStr;
            //fc.PreFlip.PriceStr = "$" + preSubDivPriceStr;
            fcList.Add(fc);
        }

        private static void PrintActiveWithinZip(
            Dictionary<string, List<Listing>> activeByZip,
            Dictionary<string, List<Listing>> activeBySubDiv,
            Dictionary<string, List<Listing>> activeByCommDivHouses,
            Dictionary<string, FlippedCharacteristics> zipResults,
            Dictionary<string, FlippedCharacteristics> indivSubDivResults,
            Dictionary<string, FlippedCharacteristics> commonSubDivResults,
            Dictionary<string, Dictionary<string, Dictionary<string, List<Listing>>>> activeMultiKeyHash)
        {
            var activeZipHouses = new Dictionary<string, List<Listing>>();
            var activeSubDivHouses = new Dictionary<string, List<Listing>>();
            var activeCommDivHouses = new Dictionary<string, List<Listing>>();
            foreach (var zip in zipResults.Keys)
            {
                var eligibleZipHouses = new List<Listing>();
                activeZipHouses.Add(zip, eligibleZipHouses);

                var subDivHash = activeMultiKeyHash[zip];
                foreach (var subDiv in subDivHash.Keys)
                {
                    var eligibleSubDivHouses = new List<Listing>();

                    activeSubDivHouses.Add(zip + "::" + subDiv, eligibleSubDivHouses);
                    var houseHash = subDivHash[subDiv];
                    foreach (var houseId in houseHash.Keys)
                    {
                        var records = houseHash[houseId];
                        /*if(records.Count!=1)
                        {
                            throw new InvalidOperationException("Error: Don't know what to do with multiple active listings for the same house id!");
                        }
                        var house = records[0];*/
                        var house = records[records.Count - 1];
                        if (zipResults.ContainsKey(zip))
                        {
                            var flipResults = zipResults[zip];
                            var list = eligibleZipHouses;
                            DetermineEligibleActiveListing(house, flipResults, list);
                        }
                        else
                        {
                            //Console.WriteLine();
                        }

                        var key1 = subDiv;
                        var key2 = subDiv.ToUpper();
                        var key3 = zip + "::" + subDiv.ToUpper();
                        var key4 = zip + "::" + subDiv.ToLower();
                        var key5 = subDiv.ToLower();
                        bool contains = indivSubDivResults.ContainsKey(key1);
                        bool contains2 = indivSubDivResults.ContainsKey(key2);
                        bool contains3 = indivSubDivResults.ContainsKey(key3);
                        bool contains4 = indivSubDivResults.ContainsKey(key4);
                        bool contains5 = indivSubDivResults.ContainsKey(key5);
                        if (contains || contains2 || contains3 || contains4 || contains5)
                        {
                            string key = null;
                            if (contains) { key = key1; }
                            else if (contains2) { key = key2; }
                            else if (contains3) { key = key3; }
                            else if (contains4) { key = key4; }
                            else if (contains5) { key = key5; }
                            else { throw new InvalidOperationException("Error: I don't know what key to select!"); }
                            var flipResults = indivSubDivResults[key];
                            var list = eligibleSubDivHouses;
                            DetermineEligibleActiveListing(house, flipResults, list);
                        }
                        else
                        {
                            //Console.WriteLine();
                        }

                        foreach (var commDiv in commonSubDivResults.Keys)
                        {
                            if (house.LegalSubdivisionName.ToLower().Contains(commDiv.ToLower()))
                            {
                                var lookup = zip + "::" + commDiv;
                                List<Listing> eligibleCommSubDivHouses = null;
                                if (activeCommDivHouses.ContainsKey(lookup))
                                {
                                    eligibleCommSubDivHouses = activeCommDivHouses[lookup];
                                }
                                else
                                {
                                    eligibleCommSubDivHouses = new List<Listing>();
                                    activeCommDivHouses.Add(lookup, eligibleCommSubDivHouses);
                                }

                                var flipResults = commonSubDivResults[commDiv];
                                var list = eligibleCommSubDivHouses;
                                DetermineEligibleActiveListing(house, flipResults, list);
                            }
                        }
                    }
                }
            }

            foreach (var zip in activeZipHouses.Keys)
            {
                var flipResults = zipResults[zip];
                var houses = activeZipHouses[zip];
                var numHouses = houses.Count;
                if (numHouses == 0) { continue; }
                double newPrice = flipResults.PostFlip.Price;
                //Logger.Debug(
                //    "Zipcode: {0} >> {1} Eligible houses within 20% of preflip price ${2} and built within [{3}] {4}-{5}", zip, numHouses, ToString(newPrice), flipResults.PreFlip.YearBuilt, flipResults.MinYear, flipResults.MaxYear);

                Console.WriteLine(
                    "Zipcode: {0} >> {1} Eligible houses within 20% of preflip price ${2}, \r\n" +
                    "built within [{3}] {4}-{5}, \r\n" +
                    "sq ft: [{6}] {7}-{8}, \r\n" +
                    "Bedrooms: {9}\r\n" +
                    "Baths: {10}\r\n" +
                    "Half Baths: {11}",
                    zip, numHouses, ToString(newPrice),
                    flipResults.PreFlip.YearBuilt, flipResults.MinYear, flipResults.MaxYear,
                    flipResults.PreFlip.SqFtHeated, flipResults.MinSqft, flipResults.MaxSqft,
                    flipResults.PreFlip.Beds,
                    flipResults.PreFlip.FullBaths,
                    flipResults.PreFlip.HalfBaths);

                var count = 0;
                houses.Sort(
                    (x, y) =>
                    {
                        return Convert.ToDouble(x.CurrentPriceValue()).CompareTo(Convert.ToDouble(y.CurrentPriceValue()));
                    }
                    );
                activeByZip.Add(zip, houses);
                foreach (var house in houses)
                {
                    ++count;
                    var homeAddress = house.Address + " " + house.City + " " + house.PostalCode;
                    //var flipResults = zipResults[zip];
                    double currentPrice = house.CurrentPriceValue();
                    double profit = newPrice - currentPrice;
                    Console.WriteLine(
                        "{0}) {1}: {2} {3} {4} sq ft:{5} bedrooms:{6} baths:{7} half baths:{8}",
                        //"in zip {3} has current price {4} ",// + //can be sold for ${5} " +
                        //"Potential Profit: ${5}", 
                        count, house.MLNumber, homeAddress, house.CurrentPrice, house.YearBuilt, house.SqFtHeated, house.Beds, house.FullBaths, house.HalfBaths);
                    //zip, house.CurrentPrice);//, //ToString(newPrice), 
                    //ToString(profit));
                }
                Console.WriteLine();
            }

            foreach (var subDiv in activeSubDivHouses.Keys)
            {
                if (!indivSubDivResults.ContainsKey(subDiv)) { continue; }
                var flipResults = indivSubDivResults[subDiv];
                var houses = activeSubDivHouses[subDiv];
                var numHouses = houses.Count;
                if (numHouses == 0) { continue; }
                double newPrice = flipResults.PostFlip.Price;
                /*Logger.Debug(
                    "Subdivision: {0} >> {1} Eligible houses within 20% of preflip price ${2}, " +
                    "built within [{3}] {4}-{5}, " +
                    "sq ft: [{6}] {7}-{8}", 
                    subDiv, numHouses, ToString(newPrice), 
                    flipResults.PreFlip.YearBuilt,flipResults.MinYear, flipResults.MaxYear,
                    flipResults.PreFlip.SqFtHeated, flipResults.MinSqft, flipResults.MaxSqft);*/

                Console.WriteLine(
                    "Subdivision: {0} >> {1} Eligible houses within 20% of preflip price ${2}, \r\n" +
                    "built within [{3}] {4}-{5}, \r\n" +
                    "sq ft: [{6}] {7}-{8}, \r\n" +
                    "Bedrooms: {9}\r\n" +
                    "Baths: {10}\r\n" +
                    "Half Baths: {11}",
                    subDiv, numHouses, ToString(newPrice),
                    flipResults.PreFlip.YearBuilt, flipResults.MinYear, flipResults.MaxYear,
                    flipResults.PreFlip.SqFtHeated, flipResults.MinSqft, flipResults.MaxSqft,
                    flipResults.PreFlip.Beds,
                    flipResults.PreFlip.FullBaths,
                    flipResults.PreFlip.HalfBaths);



                var count = 0;
                houses.Sort(
                   (x, y) =>
                   {
                       return Convert.ToDouble(x.CurrentPriceValue()).CompareTo(Convert.ToDouble(y.CurrentPriceValue()));
                   }
                   );
                activeBySubDiv.Add(subDiv, houses);
                foreach (var house in houses)
                {
                    ++count;
                    var homeAddress = house.Address + " " + house.City + " " + house.PostalCode;
                    //var flipResults = zipResults[zip];
                    double currentPrice = house.CurrentPriceValue();
                    double profit = newPrice - currentPrice;
                    Console.WriteLine(
                        "{0}) {1}: {2} {3} {4} sq ft:{5} bedrooms:{6} baths:{7} half baths:{8}",
                        //"in zip {3} has current price {4} ",// + //can be sold for ${5} " +
                        //"Potential Profit: ${5}",
                        count, house.MLNumber, homeAddress, house.CurrentPrice, house.YearBuilt, house.SqFtHeated, house.Beds, house.FullBaths, house.HalfBaths);
                    //subDiv, house.CurrentPrice);//, //ToString(newPrice),
                    //ToString(profit));
                }
                Console.WriteLine();
            }


            foreach (var commDiv in activeCommDivHouses.Keys)
            {
                if (!commonSubDivResults.ContainsKey(commDiv)) { continue; }
                var flipResults = commonSubDivResults[commDiv];
                var houses = activeCommDivHouses[commDiv];
                var numHouses = houses.Count;
                if (numHouses == 0) { continue; }
                double newPrice = flipResults.PostFlip.Price;
                /*Logger.Debug(
                    "Subdivision: {0} >> {1} Eligible houses within 20% of preflip price ${2}, " +
                    "built within [{3}] {4}-{5}, " +
                    "sq ft: [{6}] {7}-{8}", 
                    subDiv, numHouses, ToString(newPrice), 
                    flipResults.PreFlip.YearBuilt,flipResults.MinYear, flipResults.MaxYear,
                    flipResults.PreFlip.SqFtHeated, flipResults.MinSqft, flipResults.MaxSqft);*/

                Console.WriteLine(
                    "Subdivision: {0} >> {1} Eligible houses within 20% of preflip price ${2}, \r\n" +
                    "built within [{3}] {4}-{5}, \r\n" +
                    "sq ft: [{6}] {7}-{8}, \r\n" +
                    "Bedrooms: {9}\r\n" +
                    "Baths: {10}\r\n" +
                    "Half Baths: {11}",
                    commDiv, numHouses, ToString(newPrice),
                    flipResults.PreFlip.YearBuilt, flipResults.MinYear, flipResults.MaxYear,
                    flipResults.PreFlip.SqFtHeated, flipResults.MinSqft, flipResults.MaxSqft,
                    flipResults.PreFlip.Beds,
                    flipResults.PreFlip.FullBaths,
                    flipResults.PreFlip.HalfBaths);



                var count = 0;
                houses.Sort(
                   (x, y) =>
                   {
                       return Convert.ToDouble(x.CurrentPriceValue()).CompareTo(Convert.ToDouble(y.CurrentPriceValue()));
                   }
                   );
                activeBySubDiv.Add(commDiv, houses);
                foreach (var house in houses)
                {
                    ++count;
                    var homeAddress = house.Address + " " + house.City + " " + house.PostalCode;
                    //var flipResults = zipResults[zip];
                    double currentPrice = house.CurrentPriceValue();
                    double profit = newPrice - currentPrice;
                    Console.WriteLine(
                        "{0}) {1}: {2} {3} {4} sq ft:{5} bedrooms:{6} baths:{7} half baths:{8}",
                        //"in zip {3} has current price {4} ",// + //can be sold for ${5} " +
                        //"Potential Profit: ${5}",
                        count, house.MLNumber, homeAddress, house.CurrentPrice, house.YearBuilt, house.SqFtHeated, house.Beds, house.FullBaths, house.HalfBaths);
                    //subDiv, house.CurrentPrice);//, //ToString(newPrice),
                    //ToString(profit));
                }
                Console.WriteLine();
            }
        }


        private static void DetermineEligibleActiveListing(Listing house, FlippedCharacteristics flipResults, List<Listing> list)
        {
            var flipYear = (int)flipResults.PreFlip.YearBuilt;
            var maxSqFt = flipResults.PreFlip.SqFtHeated + 200;
            var minSqFt = maxSqFt - 400;
            flipResults.MinSqft = minSqFt;
            flipResults.MaxSqft = maxSqFt;
            var roundDown = RoundDown(flipYear);
            var roundUp = RoundUp(flipYear);
            int min, max;
            if (flipYear == roundDown) //Year divisible by 10 i.e. 1980
            {
                min = roundDown;
                max = roundDown + 9;  //we set the max year to 1989
            }
            else
            {
                min = roundDown;
                max = roundUp - 1;
            }
            flipResults.MinYear = min;
            flipResults.MaxYear = max;

            if (house.CurrentPriceValue() <= 1.2 * flipResults.PreFlip.Price) //criteria #1: price
            {
                var houseYear = house.YearBuiltValue();
                if (min <= houseYear && houseYear <= max) //criteria #2: year built
                {
                    var houseSqft = house.SqFtHeatedValue();
                    if (minSqFt <= houseSqft && houseSqft <= maxSqFt) //criteria #3: sq ft
                    {
                        var minBeds = flipResults.PreFlip.Beds;
                        if (minBeds <= house.BedsValue()) //criteria #4: same beds or more
                        {
                            list.Add(house);
                        }
                    }
                }
            }
        }

        private static int RoundUp(int toRound)
        {
            if ((toRound % 10) == 0) return toRound;
            return (10 - Math.Abs(toRound) % 10) + toRound;
        }

        private static int RoundDown(int toRound)
        {
            if ((toRound % 10) == 0) return toRound;
            return toRound - Math.Abs(toRound) % 10;
        }

        private static Dictionary<string, FlippedCharacteristics> PrintSubDivAlphabetical(
            Dictionary<string, FlippedCharacteristics> indivSubDivResults)
        {
            var commonDivResults = new Dictionary<string, FlippedCharacteristics>(StringComparer.OrdinalIgnoreCase);
            var list = indivSubDivResults.Values.ToList();
            list.Sort(
                (x, y) =>
                {
                    if (x == null && y == null) { return 0; }
                    if (x == null && y != null) { return 1; }
                    if (x != null && y == null) { return -1; }
                    var cityComp = x.City.CompareTo(y.City);
                    if (cityComp == 0)
                    {
                        return x.SubDivision.CompareTo(y.SubDivision);
                    }
                    return cityComp;
                }
                );
            Console.WriteLine("Subdivisions ordered alphabetically by city then subdivision:");


            for (int k = 0; k < list.Count; k++)
            {
                var next = list[k];

                //Console.WriteLine("{0}) {1} | {2} |${3}", k + 1, r.Zipcode, r.SubDivision, ToString(r.Profit));
                Console.WriteLine(
                    "{0}) ({1}) {2} PreFlip:[Sum={3},Count={4},AvgPrice={5}] PostFlip:[Sum={6},Count={7},AvgPrice={8}] => Profit:{9}",
                    k + 1,
                    next.City,
                    next.Zipcode,
                    next.PreFlip.Sum,
                    next.PreFlip.Price,
                    next.PreFlip.NumHouses,
                    next.PostFlip.Sum,
                    next.PostFlip.NumHouses,
                    next.PostFlip.Price,
                    next.Profit);

                if (k == 0) { continue; }
                AggregateByCommonSubDivision(commonDivResults, list, k, next);
            }


            var bigSubDivResults = PrintCommonSubDiv(commonDivResults);
            //PrintCommonByProfit(bigSubDivResults);
            //PrintCommonByCityThenProfit(bigSubDivResults);
            return bigSubDivResults;
        }

        private static Dictionary<string, FlippedCharacteristics> PrintCommonSubDiv(
            Dictionary<string, FlippedCharacteristics> commonSubDivHash)
        {
            var bigSubDivResults = new Dictionary<string, FlippedCharacteristics>();
            var list = commonSubDivHash.Values.ToList();
            list.Sort(
                (x, y) =>
                {
                    if (x == null && y == null) { return 0; }
                    if (x == null && y != null) { return 1; }
                    if (x != null && y == null) { return -1; }
                    var cityComp = x.City.CompareTo(y.City);
                    if (cityComp == 0)
                    {
                        return x.SubDivision.CompareTo(y.SubDivision);
                    }
                    return cityComp;
                }
                );
            Console.WriteLine("Common subdivisions ordered alphabetically by city then subdivision:");

            var count = 0;
            for (int k = 0; k < list.Count; k++)
            {
                var next = list[k];
                if (next.Aggregation.Count == 1) { continue; }
                ++count;

                var subDiv = next.SubDivision.Trim();
                var updated = true;
                while (updated)
                {
                    updated = false;
                    var end = subDiv.Length - 1;
                    var w = end;
                    var start = 0;
                    for (; w >= start; w--)
                    {
                        var ch = subDiv[w];
                        if (Char.IsLetter(ch))
                        {
                            break;
                        }
                    }

                    if (w != end)
                    {
                        if (w >= start)
                        {
                            subDiv = subDiv.Substring(start, w - start + 1).Trim();
                            updated = true;
                        }
                    }

                    if (subDiv.ToLower().EndsWith(" unit"))
                    {
                        subDiv = subDiv.Substring(start, subDiv.Length - " unit".Length - start).Trim();
                        updated = true;
                    }

                    if (subDiv.ToLower().EndsWith(" ph"))
                    {
                        subDiv = subDiv.Substring(start, subDiv.Length - " ph".Length - start).Trim();
                        updated = true;
                    }

                    if (subDiv.ToLower().EndsWith(" sec"))
                    {
                        subDiv = subDiv.Substring(start, subDiv.Length - " sec".Length - start).Trim();
                        updated = true;
                    }
                }
                //next.Profit = "$" + ToString(next.CalculateProfit());
                //Console.WriteLine("{0}) {1} | {2} |${3}", k + 1, r.Zipcode, r.SubDivision, ToString(r.Profit));
                Console.WriteLine(
                    "{0}) ({1}) {2} ***{3}*** SubDivCount={4} " +
                    "PreFlip:[Sum={5},Count={6},AvgPrice={7}] " +
                    "PostFlip:[Sum={8},Count={9},AvgPrice={10}] => " +
                    //"Profit:${11}",
                    "Profit:{11}",
                    count, next.City, subDiv, next.Zipcode, next.Aggregation.Count,
                    next.PreFlip.Sum, next.PreFlip.NumHouses, next.PreFlip.Price,
                    next.PostFlip.Sum, next.PostFlip.NumHouses, next.PostFlip.Price,
                    //ToString(next.Profit)
                    next.Profit);

                var flipHouse = new FlippedCharacteristics()
                {
                    City = next.City,
                    PostFlip = next.PostFlip,
                    PreFlip = next.PreFlip,
                    SubDivision = subDiv,
                    Zipcode = next.Zipcode,
                    //Profit = next.Profit
                };
                flipHouse.Aggregation = new List<FlippedCharacteristics>();
                flipHouse.Aggregation.AddRange(next.Aggregation);
                bigSubDivResults.Add(subDiv, flipHouse);

                /*
                Console.WriteLine("\t [{0}]", next.SubDivision.Trim());
                Console.WriteLine("\t ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ ");
                foreach (var thisSubDiv in next.Aggregation)
                {
                    Console.WriteLine("\t+ {0}", thisSubDiv.SubDivision);
                }
                */
            }

            return bigSubDivResults;
        }

        private static void AggregateByCommonSubDivision(
            Dictionary<string, FlippedCharacteristics> commonSubDivHash,
            List<FlippedCharacteristics> list,
            int k,
            FlippedCharacteristics next)
        {
            var currentName = next.SubDivision;
            bool found = false;
            if (currentName.ToLower().Contains("ACREAGE & UNREC".ToLower()))
            {
                string prevName = list[k - 1].SubDivision;
                if (prevName != null)
                {

                    //Nothing in common with previous
                    // so add next to commonSubDiv
                    var zip = next.Zipcode;
                    AddUnCommonSubDiv(commonSubDivHash, list, k, next, zip + "::" + currentName, prevName);
                }
            }
            else
            {
                //See if you have something in common with any of the keys
                foreach (var key in commonSubDivHash.Keys)
                {
                    var prevName = key;
                    string commonSubDivName = GetCommonSubDivName(prevName, currentName).Trim();

                    if (commonSubDivName != string.Empty)
                    /*{
                        //Nothing in common
                        // Store prev and next separately
                        var data = new FlippedHouse()
                        {
                            City = next.City,
                            Zipcode = next.Zipcode,
                            PreFlip = new HouseCharacteristics()
                            {
                                Sum = next.PreFlip.Sum,
                                NumHouses = next.PreFlip.NumHouses

                            },
                            PostFlip = new HouseCharacteristics()
                            {
                                Sum = next.PostFlip.Sum,
                                NumHouses = next.PostFlip.NumHouses
                            },
                        };
                        commonSubDivHash.Add(currentName, data);

                        //averages
                        data.PreFlip.Price = data.PreFlip.Sum / data.PreFlip.NumHouses;
                        data.PostFlip.Price = data.PostFlip.Sum / data.PostFlip.NumHouses;

                        break;
                    }
                    else */
                    //Something in common
                    {
                        found = true;
                        /*if (commonSubDivHash.ContainsKey(commonSubDivName))
                        {
                            var data = commonSubDivHash[commonSubDivName];
                            data.PreFlip.Sum += next.PreFlip.Sum;
                            data.PreFlip.NumHouses += next.PreFlip.NumHouses;
                            data.PreFlip.Price = data.PreFlip.Sum / data.PreFlip.NumHouses;
                            data.PostFlip.Sum += next.PostFlip.Sum;
                            data.PostFlip.NumHouses += next.PostFlip.NumHouses;
                            data.PostFlip.Price = data.PostFlip.Sum / data.PostFlip.NumHouses;
                        }
                        else
                        {*/

                        var prev = commonSubDivHash[key];
                        string newZip = CombineZips(prev, next);

                        var totalData = new FlippedCharacteristics()
                        {
                            SubDivision = commonSubDivName,

                            City = next.City,
                            Zipcode = newZip, // prev.Zipcode + (prevZip.ToLower().Trim() != nextZip.ToLower().Trim() ? ("," + nextZip) : string.Empty),
                            PreFlip = new HouseCharacteristics()
                            {
                                Sum = prev.PreFlip.Sum + next.PreFlip.Sum,
                                NumHouses = prev.PreFlip.NumHouses + next.PreFlip.NumHouses,
                                //Beds = prev.PreFlip.Beds + next.PreFlip.Beds,
                                //FullBaths = prev.PreFlip.FullBaths + next.PreFlip.FullBaths,
                                //HalfBaths = prev.PreFlip.HalfBaths + next.PreFlip.HalfBaths,
                                //SqFtHeated = prev.PreFlip.SqFtHeated + next.PreFlip.SqFtHeated,
                                //YearBuilt = prev.PreFlip.YearBuilt + next.PreFlip.YearBuilt
                            },
                            PostFlip = new HouseCharacteristics()
                            {
                                Sum = prev.PostFlip.Sum + next.PostFlip.Sum,
                                NumHouses = prev.PostFlip.NumHouses + next.PostFlip.NumHouses,
                                //Beds = prev.PostFlip.Beds + next.PostFlip.Beds,
                                //FullBaths = prev.PostFlip.FullBaths + next.PostFlip.FullBaths,
                                //HalfBaths = prev.PostFlip.HalfBaths + next.PostFlip.HalfBaths,
                                //SqFtHeated = prev.PostFlip.SqFtHeated + next.PostFlip.SqFtHeated,
                                //YearBuilt = prev.PostFlip.YearBuilt + next.PostFlip.YearBuilt
                            }
                        };
                        totalData.PreFlip.Price = Math.Round(totalData.PreFlip.Sum / totalData.PreFlip.NumHouses, 2);
                        totalData.PostFlip.Price = Math.Round(totalData.PostFlip.Sum / totalData.PostFlip.NumHouses, 2);

                        //totalData.PreFlip.Beds = Math.Round(totalData.PreFlip.Beds / totalData.PreFlip.NumHouses, 0);
                        //totalData.PreFlip.FullBaths = Math.Round(totalData.PreFlip.FullBaths / totalData.PreFlip.NumHouses, 0);
                        //totalData.PreFlip.HalfBaths = Math.Round(totalData.PreFlip.HalfBaths / totalData.PreFlip.NumHouses, 0);
                        //totalData.PreFlip.SqFtHeated = Math.Round(totalData.PreFlip.SqFtHeated / totalData.PreFlip.NumHouses, 0);
                        //totalData.PreFlip.YearBuilt = Math.Round(totalData.PreFlip.YearBuilt / totalData.PreFlip.NumHouses, 0);

                        //totalData.PostFlip.Beds = Math.Round(totalData.PostFlip.Beds / totalData.PostFlip.NumHouses, 0);
                        //totalData.PostFlip.FullBaths = Math.Round(totalData.PostFlip.FullBaths / totalData.PostFlip.NumHouses, 0);
                        //totalData.PostFlip.HalfBaths = Math.Round(totalData.PostFlip.HalfBaths / totalData.PostFlip.NumHouses, 0);
                        //totalData.PostFlip.SqFtHeated = Math.Round(totalData.PostFlip.SqFtHeated / totalData.PostFlip.NumHouses, 0);
                        //totalData.PostFlip.YearBuilt = Math.Round(totalData.PostFlip.YearBuilt / totalData.PostFlip.NumHouses, 0);

                        totalData.Aggregation = new List<FlippedCharacteristics>();
                        totalData.Aggregation.AddRange(prev.Aggregation);
                        totalData.Aggregation.Add(next);
                        UpdateAvgHomeDimensions(totalData);
                        var newName = next.SubDivision;
                        commonSubDivHash.Remove(key);
                        commonSubDivHash.Add(commonSubDivName, totalData);

                        //}

                        break;
                    }

                }

                if (!found) // if nothing found common with any of the keys, let's look at previous
                {
                    string prevName = list[k - 1].SubDivision;
                    if (prevName != null)
                    {
                        string commonSubDivName = GetCommonSubDivName(prevName, currentName).Trim();
                        if (commonSubDivName == string.Empty)
                        {
                            //Nothing in common with previous
                            // so add next to commonSubDiv
                            AddUnCommonSubDiv(commonSubDivHash, list, k, next, currentName, prevName);
                        }
                        else //Something in common with previous  (We assume, previous has already been taken care of: aggregated into common already)
                        {
                            if (commonSubDivHash.ContainsKey(commonSubDivName)) //Look for that root common name
                            {
                                var data = commonSubDivHash[commonSubDivName];  // then just add the next data
                                data.PreFlip.Sum += next.PreFlip.Sum;
                                data.PreFlip.NumHouses += next.PreFlip.NumHouses;
                                //data.PreFlip.Beds += next.PreFlip.Beds;
                                //data.PreFlip.FullBaths += next.PreFlip.FullBaths;
                                //data.PreFlip.HalfBaths += next.PreFlip.HalfBaths;
                                //data.PreFlip.SqFtHeated += next.PreFlip.SqFtHeated;
                                //data.PreFlip.YearBuilt += next.PreFlip.YearBuilt;


                                data.PreFlip.Price = Math.Round(data.PreFlip.Sum / data.PreFlip.NumHouses, 2);
                                //data.PreFlip.Beds = Math.Round(data.PreFlip.Beds / data.PreFlip.NumHouses, 0);
                                //data.PreFlip.FullBaths = Math.Round(data.PreFlip.FullBaths / data.PreFlip.NumHouses, 0);
                                //data.PreFlip.HalfBaths = Math.Round(data.PreFlip.HalfBaths / data.PreFlip.NumHouses, 0);
                                //data.PreFlip.SqFtHeated = Math.Round(data.PreFlip.SqFtHeated / data.PreFlip.NumHouses, 0);
                                //data.PreFlip.YearBuilt = Math.Round(data.PreFlip.YearBuilt / data.PreFlip.NumHouses, 0);


                                data.PostFlip.Sum += next.PostFlip.Sum;
                                data.PostFlip.NumHouses += next.PostFlip.NumHouses;
                                //data.PostFlip.Beds += next.PostFlip.Beds;
                                //data.PostFlip.FullBaths += next.PostFlip.FullBaths;
                                //data.PostFlip.HalfBaths += next.PostFlip.HalfBaths;
                                //data.PostFlip.SqFtHeated += next.PostFlip.SqFtHeated;
                                //data.PostFlip.YearBuilt += next.PostFlip.YearBuilt;

                                data.PostFlip.Price = Math.Round(data.PostFlip.Sum / data.PostFlip.NumHouses, 2);
                                //data.PostFlip.Beds = Math.Round(data.PostFlip.Beds / data.PostFlip.NumHouses, 0);
                                //data.PostFlip.FullBaths = Math.Round(data.PostFlip.FullBaths / data.PostFlip.NumHouses, 0);
                                //data.PostFlip.HalfBaths = Math.Round(data.PostFlip.HalfBaths / data.PostFlip.NumHouses, 0);
                                //data.PostFlip.SqFtHeated = Math.Round(data.PostFlip.SqFtHeated / data.PostFlip.NumHouses, 0);
                                //data.PostFlip.YearBuilt = Math.Round(data.PostFlip.YearBuilt / data.PostFlip.NumHouses, 0);


                                data.Aggregation.Add(next);

                                UpdateAvgHomeDimensions(data);
                            }
                            else
                            {
                                var prev = list[k - 1];
                                string newZip = CombineZips(prev, next);

                                var totalData = new FlippedCharacteristics()
                                {
                                    SubDivision = commonSubDivName,
                                    City = next.City,
                                    Zipcode = newZip, //prev.Zipcode + (prev.Zipcode.ToLower().Trim() != next.Zipcode.ToLower().Trim() ? ("," + next.Zipcode) : string.Empty),
                                    PreFlip = new HouseCharacteristics()
                                    {
                                        Sum = prev.PreFlip.Sum + next.PreFlip.Sum,
                                        NumHouses = prev.PreFlip.NumHouses + next.PreFlip.NumHouses,
                                        //Beds = prev.PreFlip.Beds + next.PreFlip.Beds,
                                        //FullBaths = prev.PreFlip.FullBaths + next.PreFlip.FullBaths,
                                        //HalfBaths = prev.PreFlip.HalfBaths + next.PreFlip.HalfBaths,
                                        //SqFtHeated = prev.PreFlip.SqFtHeated + next.PreFlip.SqFtHeated,
                                        //YearBuilt = prev.PreFlip.YearBuilt + next.PreFlip.YearBuilt
                                    },
                                    PostFlip = new HouseCharacteristics()
                                    {
                                        Sum = prev.PostFlip.Sum + next.PostFlip.Sum,
                                        NumHouses = prev.PostFlip.NumHouses + next.PostFlip.NumHouses,
                                        //Beds = prev.PostFlip.Beds + next.PostFlip.Beds,
                                        //FullBaths = prev.PostFlip.FullBaths + next.PostFlip.FullBaths,
                                        //HalfBaths = prev.PostFlip.HalfBaths + next.PostFlip.HalfBaths,
                                        //SqFtHeated = prev.PostFlip.SqFtHeated + next.PostFlip.SqFtHeated,
                                        //YearBuilt = prev.PostFlip.YearBuilt + next.PostFlip.YearBuilt
                                    },
                                    Aggregation = new List<FlippedCharacteristics>() { prev, next }
                                };
                                totalData.PreFlip.Price = Math.Round(totalData.PreFlip.Sum / totalData.PreFlip.NumHouses, 2);
                                totalData.PostFlip.Price = Math.Round(totalData.PostFlip.Sum / totalData.PostFlip.NumHouses, 2);


                                //totalData.PreFlip.Beds = Math.Round(totalData.PreFlip.Beds / totalData.PreFlip.NumHouses, 0);
                                //totalData.PreFlip.FullBaths = Math.Round(totalData.PreFlip.FullBaths / totalData.PreFlip.NumHouses, 0);
                                //totalData.PreFlip.HalfBaths = Math.Round(totalData.PreFlip.HalfBaths / totalData.PreFlip.NumHouses, 0);
                                //totalData.PreFlip.SqFtHeated = Math.Round(totalData.PreFlip.SqFtHeated / totalData.PreFlip.NumHouses, 0);
                                //totalData.PreFlip.YearBuilt = Math.Round(totalData.PreFlip.YearBuilt / totalData.PreFlip.NumHouses, 0);

                                //totalData.PostFlip.Beds = Math.Round(totalData.PostFlip.Beds / totalData.PostFlip.NumHouses, 0);
                                //totalData.PostFlip.FullBaths = Math.Round(totalData.PostFlip.FullBaths / totalData.PostFlip.NumHouses, 0);
                                //totalData.PostFlip.HalfBaths = Math.Round(totalData.PostFlip.HalfBaths / totalData.PostFlip.NumHouses, 0);
                                //totalData.PostFlip.SqFtHeated = Math.Round(totalData.PostFlip.SqFtHeated / totalData.PostFlip.NumHouses, 0);
                                //totalData.PostFlip.YearBuilt = Math.Round(totalData.PostFlip.YearBuilt / totalData.PostFlip.NumHouses, 0);

                                UpdateAvgHomeDimensions(totalData);

                                commonSubDivHash.Add(commonSubDivName, totalData);
                            }
                        }
                    }
                }
            }
        }

        private static void AddUnCommonSubDiv(
            Dictionary<string, FlippedCharacteristics> commonSubDivHash,
            List<FlippedCharacteristics> list,
            int k,
            FlippedCharacteristics next,
            string currentName,
            string prevName)
        {
            var data = new FlippedCharacteristics()
            {
                SubDivision = next.SubDivision,
                City = next.City,
                Zipcode = next.Zipcode,
                PreFlip = new HouseCharacteristics()
                {
                    Sum = next.PreFlip.Sum,
                    NumHouses = next.PreFlip.NumHouses,

                    Beds = next.PreFlip.Beds,
                    FullBaths = next.PreFlip.FullBaths,
                    HalfBaths = next.PreFlip.HalfBaths,
                    SqFtHeated = next.PreFlip.SqFtHeated,
                    YearBuilt = next.PreFlip.YearBuilt

                },
                PostFlip = new HouseCharacteristics()
                {
                    Sum = next.PostFlip.Sum,
                    NumHouses = next.PostFlip.NumHouses,

                    Beds = next.PostFlip.Beds,
                    FullBaths = next.PostFlip.FullBaths,
                    HalfBaths = next.PostFlip.HalfBaths,
                    SqFtHeated = next.PostFlip.SqFtHeated,
                    YearBuilt = next.PostFlip.YearBuilt
                },
                Aggregation = new List<FlippedCharacteristics>() { next }
            };
            commonSubDivHash.Add(currentName, data);

            //averages
            data.PreFlip.Price = Math.Round(data.PreFlip.Sum / data.PreFlip.NumHouses, 2);
            data.PostFlip.Price = Math.Round(data.PostFlip.Sum / data.PostFlip.NumHouses, 2);

            if (k == 1)  //unique case where, for the first list item, we need to store it in common sub div as well if it doesn't match
            {
                var prev = list[k - 1];
                var prevData = new FlippedCharacteristics()
                {
                    SubDivision = prev.SubDivision,
                    City = prev.City,
                    Zipcode = prev.Zipcode,
                    PreFlip = new HouseCharacteristics()
                    {
                        Sum = prev.PreFlip.Sum,
                        NumHouses = prev.PreFlip.NumHouses,

                        Beds = prev.PreFlip.Beds,
                        FullBaths = prev.PreFlip.FullBaths,
                        HalfBaths = prev.PreFlip.HalfBaths,
                        SqFtHeated = prev.PreFlip.SqFtHeated,
                        YearBuilt = prev.PreFlip.YearBuilt

                    },
                    PostFlip = new HouseCharacteristics()
                    {
                        Sum = prev.PostFlip.Sum,
                        NumHouses = prev.PostFlip.NumHouses,

                        Beds = prev.PostFlip.Beds,
                        FullBaths = prev.PostFlip.FullBaths,
                        HalfBaths = prev.PostFlip.HalfBaths,
                        SqFtHeated = prev.PostFlip.SqFtHeated,
                        YearBuilt = prev.PostFlip.YearBuilt
                    },
                    Aggregation = new List<FlippedCharacteristics>() { prev }
                };
                commonSubDivHash.Add(prevName, prevData);

                //averages
                prevData.PreFlip.Price = Math.Round(prevData.PreFlip.Sum / prevData.PreFlip.NumHouses, 2);
                prevData.PostFlip.Price = Math.Round(prevData.PostFlip.Sum / prevData.PostFlip.NumHouses, 2);
            }
        }

        private static void UpdateAvgHomeDimensions(FlippedCharacteristics data)
        {
            double totalBeds = 0, totalFullBaths = 0, totalHalfBaths = 0, totalSqft = 0, totalYearBuilt = 0;
            foreach (var indivSubDiv in data.Aggregation)
            {
                totalBeds += indivSubDiv.PreFlip.Beds;
                totalFullBaths += indivSubDiv.PreFlip.FullBaths;
                totalHalfBaths += indivSubDiv.PreFlip.HalfBaths;
                totalSqft += indivSubDiv.PreFlip.SqFtHeated;
                totalYearBuilt += indivSubDiv.PreFlip.YearBuilt;
            }

            data.PreFlip.Beds = Math.Round(totalBeds / data.PreFlip.NumHouses, 0);
            data.PreFlip.FullBaths = Math.Round(totalFullBaths / data.PreFlip.NumHouses, 0);
            data.PreFlip.HalfBaths = Math.Round(totalHalfBaths / data.PreFlip.NumHouses, 0);
            data.PreFlip.SqFtHeated = Math.Round(totalSqft / data.PreFlip.NumHouses, 0);
            data.PreFlip.YearBuilt = Math.Round(totalYearBuilt / data.PreFlip.NumHouses, 0);

            totalBeds = 0; totalFullBaths = 0; totalHalfBaths = 0; totalSqft = 0; totalYearBuilt = 0;
            foreach (var indivSubDiv in data.Aggregation)
            {
                totalBeds += indivSubDiv.PostFlip.Beds;
                totalFullBaths += indivSubDiv.PostFlip.FullBaths;
                totalHalfBaths += indivSubDiv.PostFlip.HalfBaths;
                totalSqft += indivSubDiv.PostFlip.SqFtHeated;
                totalYearBuilt += indivSubDiv.PostFlip.YearBuilt;
            }

            data.PostFlip.Beds = Math.Round(totalBeds / data.PostFlip.NumHouses, 0);
            data.PostFlip.FullBaths = Math.Round(totalFullBaths / data.PostFlip.NumHouses, 0);
            data.PostFlip.HalfBaths = Math.Round(totalHalfBaths / data.PostFlip.NumHouses, 0);
            data.PostFlip.SqFtHeated = Math.Round(totalSqft / data.PostFlip.NumHouses, 0);
            data.PostFlip.YearBuilt = Math.Round(totalYearBuilt / data.PostFlip.NumHouses, 0);
        }

        private static string CombineZips(FlippedCharacteristics prev, FlippedCharacteristics next)
        {
            var prevZip = prev.Zipcode;
            string[] zipList = prevZip.Split(',');
            if (zipList == null || zipList.Count() > 0)
            {
                prevZip = StripZip(prevZip);
            }

            var nextZip = next.Zipcode;
            nextZip = StripZip(nextZip).ToLower().Trim();

            var alreadyMatch = false;
            if (zipList != null && zipList.Count() > 0)
            {
                foreach (var thisZip in zipList)
                {
                    var tmp = StripZip(thisZip).ToLower().Trim();
                    if (tmp.ToLower().Trim().Equals(nextZip))
                    {
                        alreadyMatch = true;
                        break;
                    }
                }
            }

            var newZip = prevZip;
            if (!alreadyMatch)
            {
                newZip = prevZip.Trim() + ", " + nextZip.Trim();
            }

            return newZip;
        }

        private static string StripZip(string prevZip)
        {
            var colonIndex = prevZip.IndexOf(':');

            if (colonIndex >= 0)
            {
                prevZip = prevZip.Substring(0, colonIndex);
            }

            return prevZip;
        }

        private static string GetCommonSubDivName(string origPrevName, string origCurrentName)
        {
            origPrevName = origPrevName.Trim();
            origCurrentName = origCurrentName.Trim();
            var prevName = origPrevName.ToLower().Trim();
            var currentName = origCurrentName.ToLower().Trim();
            var commonName = string.Empty;
            var x = 0;
            for (; x < currentName.Length; x++)
            {
                if (x == prevName.Length || x == currentName.Length) { break; }
                var ch = prevName[x];
                var thisCh = currentName[x];
                if (ch == thisCh)
                {
                    //commonName += ch;
                    commonName += origPrevName[x];
                }
                else
                {
                    break;
                }
            }
            if (commonName.Length > 0)
            {
                //************************************************
                //??????? REMOVE
                //************************************************
                bool reject;
                if (commonName.Length < 0.5 * origPrevName.Length)
                {//old logic: i would reject common name here
                    //commonName = string.Empty;
                    reject = true;
                    commonName = commonName + "";
                }
                else
                {// old logic: i would return common name here
                    // I want to see if I get hits for subdivisions that match over 50% of their name
                    reject = false;
                    commonName = commonName + "";
                }
                //************************************************

                if (x == prevName.Length || x == currentName.Length)
                {
                    return commonName;
                }
                var remPrev = prevName.Substring(x).Trim();
                var remNext = currentName.Substring(x).Trim();

                double num1, num2;
                if (double.TryParse(remPrev, out num1) && double.TryParse(remNext, out num2))
                {
                    return commonName;
                }

                var directions = new string[] { "north", "south", "east", "west" };
                bool prevMatch = false, nextMatch = false;
                foreach (var d in directions)
                {
                    if (!prevMatch && remPrev.Equals(d))
                    {
                        prevMatch = true;

                        if (nextMatch)
                        {
                            break;
                        }
                    }
                    if (!nextMatch && remNext.Equals(d))
                    {
                        nextMatch = true;

                        if (prevMatch)
                        {
                            break;
                        }
                    }
                }

                if (prevMatch && nextMatch)
                {
                    return commonName;
                }

                int start = 0;
                var a = start;
                for (; a < remPrev.Length; a++)
                {
                    var ch = remPrev[a];
                    if (!Char.IsNumber(ch)) // skip symbols, characters (non-numbers)
                    {
                        break;
                    }
                }

                bool prevBeginsWithNumbers;
                if (a == start)
                {
                    prevBeginsWithNumbers = false;
                }
                else
                {
                    prevBeginsWithNumbers = true;
                    // a represents the first index in which encountered a digit
                    //bool prevEndsWithNumbers = a<remPrev.Length && double.TryParse(remPrev.Substring(a), out num1);
                }

                // OK let's see if we begin with numbers
                var end = remPrev.Length - 1;
                var b = end;
                for (; b >= x; b--)
                {
                    var ch = remPrev[b];
                    if (!Char.IsNumber(ch)) // skip numbers
                    {
                        break;
                    }
                }

                bool prevEndsWithNumbers;
                // b+1...end should be numbers
                if (b == end)
                {
                    prevEndsWithNumbers = false;
                }
                else
                {
                    prevEndsWithNumbers = true;
                    //prevBeginsWithNumbers = b < (remPrev.Length - 1) && double.TryParse(remPrev.Substring(b + 1), out num1);
                }


                start = 0;
                a = start;
                for (; a < remNext.Length; a++)
                {
                    var ch = remNext[a];
                    if (!Char.IsNumber(ch))
                    {
                        break;
                    }
                }

                bool nextBeginsWithNumbers;
                if (a == start)
                {
                    nextBeginsWithNumbers = false;
                }
                else
                {
                    nextBeginsWithNumbers = true;

                    // a represents the first index in which encountered a digit
                    //bool nextEndsWithNumbers = a < remNext.Length && double.TryParse(remNext.Substring(a), out num1);
                }

                // OK let's see if we begin with numbers
                end = remNext.Length - 1;
                b = end;
                for (; b >= x; b--)
                {
                    var ch = remNext[b];
                    if (!Char.IsNumber(ch)) // skip numbers
                    {
                        break;
                    }
                }


                bool nextEndsWithNumbers;
                // b+1...end should be numbers
                if (b == end)
                {
                    nextEndsWithNumbers = false;
                }
                else
                {
                    nextEndsWithNumbers = true;
                    // a...b should be numbers
                    //bool nextBeginsWithNumbers = b < (remNext.Length - 1) && double.TryParse(remNext.Substring(b+1), out num1);
                }


                if (prevBeginsWithNumbers && nextBeginsWithNumbers)
                {
                    return commonName;
                }
                if (prevEndsWithNumbers && nextEndsWithNumbers)
                {
                    //old logic: (incorrect) accept if two strings have a number in the back
                    //return commonName;
                    commonName = commonName + "";
                }

                // I reject common name
                return string.Empty;
            }
            return commonName;
        }

        private static FlippedCharacteristics PrintAverageFlipInZip(
            Dictionary<string, FlippedCharacteristics> zipCatalog,
            string zip,
            HouseCharacteristics preFlip,
            HouseCharacteristics postFlip,
            double numHouses,
            string extra)
        {
            var fc = ComputeAverageFlip(zipCatalog, zip, preFlip, postFlip, numHouses);

            //Console.WriteLine("Zipcode Average price prior to flip: ${0}", ToString(preFlip.Price));
            var preflipPriceStr = ToString(preFlip.Price);
            var label = string.Format("Zipcode Preflip{0} Average", string.IsNullOrWhiteSpace(extra) ? string.Empty : " " + extra);
            Console.WriteLine("{0}:\r\n" +
                "Price: ${1}\r\n" +
                "Beds: {2}\r\n" +
                "Full Baths: {3}\r\n" +
                "Half Baths: {4}\r\n" +
                "SqFt Heated: {5}\r\n" +
                "Year Built: {6}",
                label,
                preflipPriceStr,
                preFlip.Beds,
                preFlip.FullBaths,
                preFlip.HalfBaths,
                preFlip.SqFtHeated,
                preFlip.YearBuilt);
            Console.WriteLine();
            //Console.WriteLine("Zipcode Average price after flip: ${0}", ToString(postFlip.Price));
            var postflipPriceStr = ToString(postFlip.Price);
            label = string.Format("Zipcode Postflip{0} Average", string.IsNullOrWhiteSpace(extra) ? string.Empty : " " + extra);
            Console.WriteLine("{0}:\r\n" +
                "Price: ${1}\r\n" +
                "Beds: {2}\r\n" +
                "Full Baths: {3}\r\n" +
                "Half Baths: {4}\r\n" +
                "SqFt Heated: {5}\r\n" +
                "Year Built: {6}",
                label,
                postflipPriceStr,
                postFlip.Beds,
                postFlip.FullBaths,
                postFlip.HalfBaths,
                postFlip.SqFtHeated,
                postFlip.YearBuilt);
            var profitStr = ToString(postFlip.Price - preFlip.Price);
            Console.WriteLine("Net Profit: ${0}", profitStr);
            //fc.Profit = "$" + profitStr;
            //fc.PreFlip.PriceStr = "$" + preflipPriceStr;
            //fc.PostFlip.PriceStr = "$" + postflipPriceStr;
            return fc;
        }

        private static FlippedCharacteristics ComputeAverageFlip(
            Dictionary<string, FlippedCharacteristics> results,
            string zip, HouseCharacteristics preFlip, HouseCharacteristics postFlip,
            double numHouses)
        {
            preFlip.Sum = preFlip.Price;
            preFlip.NumHouses = numHouses;
            preFlip.Price = Math.Round(preFlip.Sum / numHouses, 2);
            preFlip.Beds = Math.Round(preFlip.Beds / numHouses, 0);
            preFlip.FullBaths = Math.Round(preFlip.FullBaths / numHouses, 0);
            preFlip.HalfBaths = Math.Round(preFlip.HalfBaths / numHouses, 0);
            preFlip.SqFtHeated = Math.Round(preFlip.SqFtHeated / numHouses, 0);
            preFlip.YearBuilt = Math.Round(preFlip.YearBuilt / numHouses, 0);

            postFlip.Sum = postFlip.Price;
            postFlip.NumHouses = numHouses;
            postFlip.Price = Math.Round(postFlip.Sum / numHouses, 2);
            postFlip.Beds = Math.Round(postFlip.Beds / numHouses, 0);
            postFlip.FullBaths = Math.Round(postFlip.FullBaths / numHouses, 0);
            postFlip.HalfBaths = Math.Round(postFlip.HalfBaths / numHouses, 0);
            postFlip.SqFtHeated = Math.Round(postFlip.SqFtHeated / numHouses, 0);
            postFlip.YearBuilt = Math.Round(postFlip.YearBuilt / numHouses, 0);

            FlippedCharacteristics fh = new FlippedCharacteristics(zip, preFlip, postFlip);
            results.Add(zip, fh);
            return fh;
        }

        private static FlippedHouse PrintHouseFlip(string zip, int count, Listing lastSold, Listing firstSold, double profit)
        {
            var homeAddress = firstSold.Address + " " + firstSold.City + " " + firstSold.PostalCode;
            var profitStr = ToString(profit);
            Console.WriteLine("{0}) {1}: {2} in zip {3} sold for {4} in {5}, sold again for {6} in {7}, Profit: ${8}", count, firstSold.MLNumber, homeAddress, zip, firstSold.CurrentPrice, firstSold.CloseDate, lastSold.CurrentPrice, lastSold.CloseDate, profitStr);
            var flippedHouse = new FlippedHouse();
            flippedHouse.Address = firstSold.Address;
            flippedHouse.City = firstSold.City;
            flippedHouse.PostalCode = firstSold.PostalCode;
            flippedHouse.PurchasePrice = firstSold.CurrentPrice;
            flippedHouse.PurchaseDate = firstSold.CloseDate;
            flippedHouse.SoldPrice = lastSold.CurrentPrice;
            flippedHouse.SoldDate = lastSold.CloseDate;
            flippedHouse.Profit = "$" + profitStr;
            flippedHouse.RoiPurchase = Math.Round(profit / firstSold.CurrentPriceValue(), 2);
            flippedHouse.RoiSold = Math.Round(profit / lastSold.CurrentPriceValue(), 2);
            return flippedHouse;
        }

        private static string ToString(double num)
        {
            var wholeNumber = num.ToString();
            var separatorIndex = wholeNumber.IndexOf('.');
            var fraction = "";
            if (separatorIndex >= 0)
            {
                fraction = wholeNumber.Substring(separatorIndex);
                wholeNumber = wholeNumber.Substring(0, separatorIndex);
                if (fraction.Length == 2)
                {
                    fraction += "0";
                }
            }
            var k = 0;
            var beforeDecimal = "";
            for (int j = wholeNumber.Length - 1; j >= 0; j--)
            {
                if (k > 0 && k % 3 == 0)
                {
                    beforeDecimal = "," + beforeDecimal;
                }
                var digit = wholeNumber[j];
                beforeDecimal = digit + beforeDecimal;
                ++k;
            }
            return beforeDecimal + fraction;
        }

        private static List<Listing> GetSortedSoldRecords(Dictionary<string, List<Listing>> zipHash, string houseId)
        {
            var soldRecords = zipHash[houseId];
            soldRecords.Sort();
            return soldRecords;
        }

        private static void FindFirstLastSold(List<Listing> soldRecords, out Listing lastSold, out Listing firstSold)
        {
            lastSold = soldRecords[soldRecords.Count - 1];
            firstSold = null;
            for (var k = soldRecords.Count - 2; k >= 0; k--)
            {
                var prevSold = soldRecords[k];
                //if (!prevSold.CloseDateValue().HasValue) { continue; }
                if (lastSold.CloseDateValue().Value < prevSold.CloseDateValue().Value)
                {
                    throw new InvalidOperationException();
                }
                var time = lastSold.CloseDateValue().Value.Subtract(prevSold.CloseDateValue().Value);
                var valid = time <= TimeSpan.FromDays(366);
                if (valid)
                {
                    firstSold = prevSold;
                }
            }
        }

        private static double DetermineProfit(double firstPrice, double lastPrice)
        {
            return lastPrice - firstPrice;
        }

        private static void AddToSum(
            HouseCharacteristics preFlip,
            HouseCharacteristics postFlip,
            HouseCharacteristics preSubDivFlip,
            HouseCharacteristics postSubDivFlip,
            Listing lastSold,
            Listing firstSold,
            out double firstPrice,
            out double lastPrice)
        {
            firstPrice = firstSold.CurrentPriceValue();
            preFlip.Price += firstPrice;
            preFlip.Beds += firstSold.BedsValue();
            preFlip.FullBaths += firstSold.FullBathsValue();
            preFlip.HalfBaths += firstSold.HalfBathsValue();
            preFlip.SqFtHeated += firstSold.SqFtHeatedValue();
            preFlip.YearBuilt += firstSold.YearBuiltValue();

            preSubDivFlip.Price += firstPrice;
            preSubDivFlip.Beds += firstSold.BedsValue();
            preSubDivFlip.FullBaths += firstSold.FullBathsValue();
            preSubDivFlip.HalfBaths += firstSold.HalfBathsValue();
            preSubDivFlip.SqFtHeated += firstSold.SqFtHeatedValue();
            preSubDivFlip.YearBuilt += firstSold.YearBuiltValue();

            lastPrice = lastSold.CurrentPriceValue();
            postFlip.Price += lastPrice;
            postFlip.Beds += lastSold.BedsValue();
            postFlip.FullBaths += lastSold.FullBathsValue();
            postFlip.HalfBaths += lastSold.HalfBathsValue();
            postFlip.SqFtHeated += lastSold.SqFtHeatedValue();
            postFlip.YearBuilt += lastSold.YearBuiltValue();

            postSubDivFlip.Price += lastPrice;
            postSubDivFlip.Beds += lastSold.BedsValue();
            postSubDivFlip.FullBaths += lastSold.FullBathsValue();
            postSubDivFlip.HalfBaths += lastSold.HalfBathsValue();
            postSubDivFlip.SqFtHeated += lastSold.SqFtHeatedValue();
            postSubDivFlip.YearBuilt += lastSold.YearBuiltValue();
        }

        private static void UpdateSoldRecords(List<Listing> soldRecords, Listing lastSold, Listing firstSold)
        {
            soldRecords.Clear();
            soldRecords.Add(firstSold);
            soldRecords.Add(lastSold);
        }

        

        

        

        

        

        

        

        private static Dictionary<string, List<Listing>> GetHouseHash(Dictionary<string, Dictionary<string, List<Listing>>> subDivHash, string subDiv)
        {
            Dictionary<string, List<Listing>> houseHash;
            if (subDivHash.ContainsKey(subDiv))
            {
                houseHash = subDivHash[subDiv];
            }
            else
            {
                houseHash = new Dictionary<string, List<Listing>>();
                subDivHash.Add(subDiv, houseHash);
            }

            return houseHash;
        }

       

        private static void AddActive(
            Dictionary<string, Dictionary<string, Dictionary<string, List<Listing>>>> activeMultiKeyHash,
            //Dictionary<string, MlsRow> currentActiveHash,
            string zip,
            string subDiv,
            string houseID,
            Listing record)
        {
            zip = zip.ToLower().Trim();
            subDiv = subDiv.ToLower().Trim();
            houseID = houseID.ToLower().Trim();

            Dictionary<string, Dictionary<string, List<Listing>>> subDivHash;
            if (activeMultiKeyHash.ContainsKey(zip))
            {
                subDivHash = activeMultiKeyHash[zip];
            }
            else
            {
                subDivHash = new Dictionary<string, Dictionary<string, List<Listing>>>();
                activeMultiKeyHash.Add(zip, subDivHash);
            }

            Dictionary<string, List<Listing>> houseHash;
            if (subDivHash.ContainsKey(subDiv))
            {
                houseHash = subDivHash[subDiv];
            }
            else
            {
                houseHash = new Dictionary<string, List<Listing>>();
                subDivHash.Add(subDiv, houseHash);
            }

            List<Listing> houseList;
            if (houseHash.ContainsKey(houseID))
            {
                houseList = houseHash[houseID];
                //throw new InvalidOperationException("Error: Not sure why  2 active records would exist for the same house id");
            }
            else
            {
                houseList = new List<Listing>();
                houseHash.Add(houseID, houseList);
            }

            houseList.Add(record);

            /*
            if(currentActiveHash.ContainsKey(houseID))
            {
                throw new InvalidOperationException("Error: Not sure why  2 active records would exist for the same house id");
            }
            else
            {
                currentActiveHash.Add(houseID, record);
            }*/
        }                    
    }    
}
