using HouseFlipper.DataAccess.Csv;
using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace HouseFlipper.DataAccess
{
    public class Importer
    {
        private MlsContext _context = null;
        private MlsContext Context
        {
            get
            {
                if (_context != null)
                {
                    return _context;
                }
                return new MlsContext();
            }
        }

        private MlsReader _reader;
        private ConcurrentDictionary<string, Listing> hash = new ConcurrentDictionary<string, Listing>();


        public Importer(MlsReader reader, MlsContext context)
        {
            _reader = reader;
            _context = context;
        }
        public void Run(bool parallel)
        {
            var timer = new Stopwatch();
            timer.Start();
            string[] fieldNames = null;
            var rowNum = 0;
            if (parallel)
            {
                _reader.ReadParallel(
                    (mlsRow) =>
                    {
                        ++rowNum;
                        Console.WriteLine("{0}: {1}", rowNum, mlsRow.Text);
                        Process(ref fieldNames, mlsRow);
                    });
            }
            else
            {
                var context = this.Context;
                foreach (var mlsRow in _reader.ReadLine())
                {
                    ++rowNum;
                    Console.WriteLine("{0}: {1}", rowNum, mlsRow.Text);
                    Process(ref fieldNames, mlsRow, context);
                }
                context.SaveChanges();
            }
            timer.Stop();
            Console.WriteLine("Total time: {0} minutes", timer.Elapsed.TotalMinutes);

        }

        private string[] Process(
            ref string[] fieldNames,
            MlsRow mlsRow,
            MlsContext context = null)
        {
            var values = MlsTokenizer.Split(mlsRow.Text);
            if (mlsRow.IsHeader)
            {
                fieldNames = values;
            }
            else
            {
                AddRecord(fieldNames, values, context);
            }

            return fieldNames;
        }

        public virtual Listing AddRecord(
            string[] colNames,
            string[] fields,
            MlsContext context = null)
        {
            StringDictionary data = ToDictionary(colNames, fields);
            var record = new Listing(data);
            var mlNum = record.MLNumber.ToLower().Trim();
            bool doSave = false;
            if (context == null)
            {
                doSave = true;
                context = this.Context;
            }

            //var query = from i in context.Listings
            //            where i.MLNumber.ToLower().Trim() == mlNum
            //            select i;
            //var count = query.Count();
            //var exists = false;
            /*if(hash.ContainsKey(mlNum))
            {
                //exists = true;
                record = hash[mlNum];
                return record;
            }
            else
            {
                hash[mlNum] = record;*/
            //}

            //if (!exists) //count==0
            //{
            context.Listings.Add(record);
            if (doSave)
            {
                context.SaveChanges();
            }
            //AddProperty(record);                
            /*}
            else
            {
                record = query.First();
            *///}

            return record;
        }

        private void AddProperty(MlsContext context, Listing record)
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
            AddPropertyListing(context, record, property);
        }

        private void AddPropertyListing(MlsContext context, Listing record, Property property)
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
