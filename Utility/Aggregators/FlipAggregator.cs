using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility.Aggregators
{
    public class FlipAggregator : Aggregator2
    {
        public FlipAggregator() : base((set, row) => row.IsSold() && set.ContainsKey(row))
        {
            var initialSet = new MlsSet();
            sold = new SoldAggregator(initialSet);
        }

        private SoldAggregator sold;
        public override bool Add(MlsRow record)
        {
            var isFlip = false;
            if (isFlip = !base.Add(record))
            {
                sold.Add(record);
            }
            return isFlip;
        }

        private static void AggregateFlips(
            //          zip,              subdiv,           houseId,  soldList
            Dictionary<string, MlsRow> soldHash, Dictionary<string, Dictionary<string, Dictionary<string, List<MlsRow>>>> flippedMultiKeyHash, MlsRow record, string houseID)
        {
            //         subDiv,            house,   soldList
            Dictionary<string, Dictionary<string, List<MlsRow>>> subDivHash = AddSubDivsion(flippedMultiKeyHash, record.PostalCode);
            //         house,      
            Dictionary<string, List<MlsRow>> houseFlippedHash = AddHouseHash(record, subDivHash);
            AddFlippedHouse(soldHash, record, houseID, houseFlippedHash);
        }

        private static Dictionary<string, Dictionary<string, List<MlsRow>>> AddSubDivsion(
            Dictionary<string, Dictionary<string, Dictionary<string, List<MlsRow>>>> flippedMultiKeyHash, string zip)
        {
            Dictionary<string, Dictionary<string, List<MlsRow>>> subDivHash;
            if (flippedMultiKeyHash.ContainsKey(zip))
            {
                subDivHash = flippedMultiKeyHash[zip];
            }
            else
            {
                subDivHash = new Dictionary<string, Dictionary<string, List<MlsRow>>>();
                flippedMultiKeyHash.Add(zip, subDivHash);
            }

            return subDivHash;
        }

        private static Dictionary<string, List<MlsRow>> AddHouseHash(MlsRow record, Dictionary<string, Dictionary<string, List<MlsRow>>> subDivHash)
        {
            return GetHouseHash(subDivHash, SubDivision(record));
        }

        private static void AddFlippedHouse(Dictionary<string, MlsRow> soldHash, MlsRow record, string houseID, Dictionary<string, List<MlsRow>> houseFlippedHash)
        {
            if (houseFlippedHash.ContainsKey(houseID))
            {
                var soldRecords = houseFlippedHash[houseID];
                soldRecords.Add(record);
            }
            else
            {
                houseFlippedHash.Add(houseID, new List<MlsRow>() { soldHash[houseID], record });
            }
        }
    }
}
