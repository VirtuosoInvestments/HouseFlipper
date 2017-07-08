using HouseFlipper.DataAccess.Csv;
using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace HouseFlipper.DataAccess
{
    public class Importer
    {
        //'''private MlsContext _context = null;'''
        private bool _multiThreaded;
        protected virtual MlsContext Context
        {
            get
            {
                /*'''if (_context != null)
                {
                    return _context;
                }'''*/
                return new MlsContext();
            }
        }

        private MlsReader _reader;
        private ConcurrentDictionary<string, Listing> hash = new ConcurrentDictionary<string, Listing>();


        public Importer(MlsReader reader, bool multiThreaded)
        {
            _reader = reader;
            _multiThreaded = multiThreaded;
        }
        public void Run(bool bulk = false)
        {
            Console.WriteLine("Running in parallel={0}, bulk={1} mode", _multiThreaded, bulk);
            var timer = new Stopwatch();
            timer.Start();
            string[] colNames = null;
            var rowNum = 0;
            if (_multiThreaded)
            {
                if (bulk)
                {
                    _reader.ReadBulkParallel(
                        (file, list) =>
                        {
                            ProcessBulk(file, ref colNames, list, ref rowNum);
                        });
                }
                else
                {
                    _reader.ReadParallel(
                        (file, mlsRow) =>
                        {
                            Interlocked.Increment(ref rowNum);
                            Console.WriteLine("{0}: {1}", rowNum, mlsRow.Text);
                            Process(file, ref colNames, mlsRow);
                        });
                }
            }
            else
            {
                using (var context = this.Context)
                {
                    foreach (var mlsRow in _reader.ReadLine())
                    {
                        ++rowNum;
                        Console.WriteLine("{0}: {1}", rowNum, mlsRow.Text);
                        Process(_reader.CurrentFile, ref colNames, mlsRow, context);
                    }
                    context.SaveChangesAsync();
                }
            }
            timer.Stop();
            Console.WriteLine("Completed run in parallel={0}, bulk={1} mode", _multiThreaded, bulk);
            Console.WriteLine("Total time: {0} minutes", timer.Elapsed.TotalMinutes);

        }

        private void ProcessBulk(
            string file,
            ref string[] colNames,
            List<MlsRow> list,
            ref int rowNum)
        {
            using (var context = this.Context)
            {
                foreach (var mlsRow in list)
                {
                    Interlocked.Increment(ref rowNum);
                    Console.WriteLine("{0}: {1}", rowNum, mlsRow.Text);

                    var values = MlsTokenizer.Split(mlsRow.Text);
                    if (mlsRow.IsHeader)
                    {
                        colNames = values;
                    }
                    else
                    {
                        Listing record = CreateListing(colNames, values, file);
                        context.Listings.Add(record);

                    }
                }
                context.SaveChangesAsync();
            }
        }

        private string[] Process(
            string file,
            ref string[] colNames,
            MlsRow mlsRow,
            MlsContext context = null)
        {
            var values = MlsTokenizer.Split(mlsRow.Text);
            if (mlsRow.IsHeader)
            {
                colNames = values;
            }
            else
            {
                AddRecord(colNames, values, file, context);
            }

            return colNames;
        }

        public virtual Listing AddRecord(
            string[] colNames,
            string[] fields,
            string file = null,
            MlsContext context = null)
        {
            Listing record = CreateListing(colNames, fields, file);
            //var mlNum = record.MLNumber.ToLower().Trim();
            //'''bool doSave = false;'''
            //'''if (context == null)'''
            if (_multiThreaded)
            {
                //'''doSave = true;'''
                using (context = this.Context)
                {
                    context.Listings.Add(record);
                    context.SaveChangesAsync();
                }
            }
            else
            {
                context.Listings.Add(record);
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
            /*'''if (doSave)
            {
                context.SaveChanges();
            }'''*/
            //AddProperty(record);                
            /*}
            else
            {
                record = query.First();
            *///}

            return record;
        }

        private Listing CreateListing(string[] colNames, string[] fields, string file)
        {
            StringDictionary data = ToDictionary(colNames, fields);
            var record = new Listing(data);
            record.State = "FL";
            if (file != null)
            {
                record.County = GetCounty(file);
            }

            return record;
        }

        private string GetCounty(string file)
        {
            var fileName = Path.GetFileName(file).ToLower().Trim();
            foreach (var k in CountyAbbreviations.Instance.Keys)
            {
                var t = k.ToLower().Trim();
                if (fileName.StartsWith(t))
                {
                    return CountyAbbreviations.Instance[k];
                }
            }
            throw new InvalidOperationException("Error: Could not find county that maps to file name '" + fileName + "'");
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
