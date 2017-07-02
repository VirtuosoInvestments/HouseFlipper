using HouseFlipper.DataAccess.Csv;
using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace HouseFlipper.DataAccess
{
    public class Importer
    {
        private MlsContext context;
        private MlsReader reader;

        public Importer(MlsReader reader, MlsContext context)
        {
            this.reader = reader;
            this.context = context;
        }
        public void Run()
        {
            string[] fieldNames = null;
            var rowNum = 0;
            foreach (var line in reader.ReadLine())
            {
                ++rowNum;
                Console.WriteLine("{0}: {1}", rowNum, line.Text);
                var values = MlsTokenizer.Split(line.Text);
                if (line.IsHeader)
                {
                    fieldNames = values;
                }
                else
                {
                    AddRecord(fieldNames, values);
                }
            }
            context.SaveChanges();
        }

        public virtual Listing AddRecord(
            string[] colNames,
            string[] fields)
        {
            StringDictionary data = ToDictionary(colNames, fields);
            var record = new Listing(data);
            var mlNum = record.MLNumber.ToLower().Trim();
            var query = from i in context.Listings
                        where i.MLNumber.ToLower().Trim()==mlNum
                        select i;
            var count = query.Count();
            if (count == 0)
            {
                context.Listings.Add(record);
                context.SaveChanges();
                AddProperty(record);                
            }
            else
            {
                record = query.First();
            }

            return record;
        }

        private void AddProperty(Listing record)
        {
            var addr = record.Address.ToLower().Trim();
            var city = record.City.ToLower().Trim();
            var zip = record.PostalCode.ToLower().Trim();


            var query = from i in context.Properties
                         where i.Address.ToLower().Trim() == addr && i.City.ToLower().Trim() == city && i.PostalCode.Trim().ToLower() == zip
                         select i;
            var count2 = query.Count();
            Property property = null;
            if (count2 == 0)
            {
                property = new Property(record);
                context.Properties.Add(property);
                context.SaveChanges();
            }
            else
            {
                property = query.First();
                property.Status = record.Status;
            }
            AddPropertyListing(record, property);
        }

        private void AddPropertyListing(Listing record, Property property)
        {
            var propListing = new PropertyListing(record, property);
            context.PropertyListings.Add(propListing);
            context.SaveChanges();
        }

        private static StringDictionary ToDictionary(string[] colNames, string[] fields)
        {
            var data = new StringDictionary();
            for (var j = 0; j < fields.Length; j++)
            {
                var field = fields[j];
                data.Add(colNames[j], field);
            }

            return data;
        }
    }
}
