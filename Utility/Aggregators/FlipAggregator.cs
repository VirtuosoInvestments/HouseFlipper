using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility
{
    public class FlipAggregator : Aggregator
    {
        public FlipAggregator() : base((set, row) => row.IsSold() && set.ContainsKey(row))
        {
            this.DataSet = new Dictionary<string, List<Flip>>();
            var initialSet = new MlsSet();
            sold = new SoldAggregator(initialSet);
            var zipTable = new ZipTable();
            var subDivTable = new SubdivisionTable();
            this.AddEvent += zipTable.HandleAdd;
            this.AddEvent += subDivTable.HandleAdd;
        }

        private SoldAggregator sold;

        public Dictionary<string, List<Flip>> DataSet { get; set; }

        public override bool Add(Listing record)
        {
            var isFlip = base.Add(record);
            if (!isFlip)
            {
                sold.Add(record);
            }
            return isFlip;
        }

        private static void AggregateFlips(
            //          zip,              subdiv,           houseId,  soldList
            Dictionary<string, Listing> soldHash, Dictionary<string, Dictionary<string, Dictionary<string, List<Listing>>>> flippedMultiKeyHash, Listing record, string houseID)
        {
            //         subDiv,            house,   soldList
            Dictionary<string, Dictionary<string, List<Listing>>> subDivHash = AddSubDivsion(flippedMultiKeyHash, record.PostalCode);
            //         house,      
            Dictionary<string, List<Listing>> houseFlippedHash = AddHouseHash(record, subDivHash);
            AddFlippedHouse(soldHash, record, houseID, houseFlippedHash);
        }

        private static Dictionary<string, Dictionary<string, List<Listing>>> AddSubDivsion(
            Dictionary<string, Dictionary<string, Dictionary<string, List<Listing>>>> flippedMultiKeyHash, string zip)
        {
            Dictionary<string, Dictionary<string, List<Listing>>> subDivHash;
            if (flippedMultiKeyHash.ContainsKey(zip))
            {
                subDivHash = flippedMultiKeyHash[zip];
            }
            else
            {
                subDivHash = new Dictionary<string, Dictionary<string, List<Listing>>>();
                flippedMultiKeyHash.Add(zip, subDivHash);
            }

            return subDivHash;
        }

        private static Dictionary<string, List<Listing>> AddHouseHash(Listing record, Dictionary<string, Dictionary<string, List<Listing>>> subDivHash)
        {
            throw new NotImplementedException(); //return GetHouseHash(subDivHash, SubDivision(record));
        }

        private static void AddFlippedHouse(Dictionary<string, Listing> soldHash, Listing record, string houseID, Dictionary<string, List<Listing>> houseFlippedHash)
        {
            if (houseFlippedHash.ContainsKey(houseID))
            {
                var soldRecords = houseFlippedHash[houseID];
                soldRecords.Add(record);
            }
            else
            {
                houseFlippedHash.Add(houseID, new List<Listing>() { soldHash[houseID], record });
            }
        }
    }
}
