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
            using (var db = new MlsContext())
            {
                foreach (var record in db.Listings)
                {
                    //AggregateBySold(soldHash, flippedMultiKeyHash, record);
                    //AggregateByActive(activeMultiKeyHash, record);

                    if (record.IsSold())
                    {
                        if (soldSet.Add(record))
                        {
                            AggregateFlips(soldHash, flippedMultiKeyHash, record, houseID);
                        }
                    }
                    else if (record.IsActive())
                    {
                        var zip = record.PostalCode;
                        var subDiv = record.LegalSubdivisionName;
                        string houseID = record.PropertyId();
                        AddActive(activeMultiKeyHash, /*currentActiveHash,*/ zip, subDiv, houseID, record);
                    }

                }
            }
        }

        public void Add(MlsRow record)
        {
            
        }

        private void AggregateBySold(Dictionary<string, MlsRow> soldHash, Dictionary<string, Dictionary<string, Dictionary<string, List<MlsRow>>>> flippedMultiKeyHash, MlsRow record)
        {
            if (record.IsSold())
            {
                string houseID = PropertyAddress(record);
                if (Contains(soldHash, houseID))
                {
                    AggregateFlips(soldHash, flippedMultiKeyHash, record, houseID);
                }
                else
                {
                    AddFirstSold(soldHash, record, houseID);
                }
            }
        }

        private static void AggregateByActive(Dictionary<string, Dictionary<string, Dictionary<string, List<MlsRow>>>> activeMultiKeyHash, /*Dictionary<string, MlsRow> currentActiveHash,*/ MlsRow record)
        {
            if (record.IsActive())
            {
                var zip = record.PostalCode;
                var subDiv = record.LegalSubdivisionName;
                string houseID = record.PropertyId();
                AddActive(activeMultiKeyHash, /*currentActiveHash,*/ zip, subDiv, houseID, record);
            }
        }
    }
}
