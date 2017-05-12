using Hack.HouseFlipper.DataAccess.DB;
using Hack.HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility.Commands
{
    public class ProcessCommand
    {
        public Dictionary<string, Dictionary<string, Dictionary<string, List<MlsRow>>>> flippedMultiKeyHash;
        public Dictionary<string, FlippedCharacteristics> indivSubDivAllResults;
        public Dictionary<string, FlippedCharacteristics> indivSubDivNoPoolResults;
        public Dictionary<string, FlippedCharacteristics> indivSubDivWithPoolResults;
        public Dictionary<string, Dictionary<string, Dictionary<string, List<MlsRow>>>> activeMultiKeyHash;

        public void Execute()
        {
            // property => sold record
            var soldHash = new Dictionary<string, MlsRow>();

            // zip => subdiv => property => sold records
            flippedMultiKeyHash = new Dictionary<string, Dictionary<string, Dictionary<string, List<MlsRow>>>>();

            // subdiv => flipped house characteristics
            indivSubDivAllResults = new Dictionary<string, FlippedCharacteristics>(StringComparer.OrdinalIgnoreCase);

            // subdiv => flipped house characteristics
            indivSubDivNoPoolResults = new Dictionary<string, FlippedCharacteristics>(StringComparer.OrdinalIgnoreCase);

            // subdiv => flipped house characteristics
            indivSubDivWithPoolResults = new Dictionary<string, FlippedCharacteristics>(StringComparer.OrdinalIgnoreCase);

            // zip => subdiv => property => active records matching flipped house characteristics
            activeMultiKeyHash = new Dictionary<string, Dictionary<string, Dictionary<string, List<MlsRow>>>>(StringComparer.OrdinalIgnoreCase);

            var flipAgg = new FlipAggregator();
            var activeAgg = new ActiveAggregator();
            using (var db = new MlsContext())
            {
                foreach (var record in db.Listings)
                {
                    //AggregateBySold(soldHash, flippedMultiKeyHash, record);
                    //AggregateByActive(activeMultiKeyHash, record);

                    flipAgg.Add(record);
                    activeAgg.Add(record);/*
                    else if (record.IsActive())
                    {
                        var zip = record.PostalCode;
                        var subDiv = record.LegalSubdivisionName;
                        string houseID = record.PropertyId();
                        AddActive(activeMultiKeyHash, zip, subDiv, houseID, record);
                    }*/

                }
            }
        }
    }
}
