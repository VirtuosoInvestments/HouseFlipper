using HouseFlipper.Common.Pipeline;
using HouseFlipper.DataAccess.Csv;
using HouseFlipper.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess
{
    public class ImporterV2
    {
        private MlsReader reader;
        private IPipe pipe;

        public ImporterV2(MlsReader reader, IPipe pipe)
        {
            this.reader = reader;
            this.pipe = pipe;
        }

        public void Run()
        {
            Console.WriteLine("Running import ...");

            var timer = new Stopwatch();
            timer.Start();
            //string[] colNames = null;
            var rowNum = 0;

            reader.ReadBulk(
                        (file, list) =>
                        {
                            BulkProcess(
                                file, 
                                //ref colNames, 
                                list, 
                                ref rowNum);
                        });
        }

        private void BulkProcess(
                        string file,
                        //ref string[] colNames,
                        List<MlsRow> list,
                        ref int rowNum)
        {
            //lock (_locker)
            //{

            string[] colNames = null;
            using (var context = new MlsContext())
            {
                foreach (var mlsRow in list)
                {
                    /*Interlocked.Increment(ref rowNum);
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
                    */

                    var values = MlsTokenizer.Split(mlsRow.Text);
                    if (mlsRow.IsHeader)
                    {
                        colNames = values;
                    }
                    else
                    {
                        //Task.Run(() => pipe.Enter(mlsRow));
                        pipe.Enter(new object[] { colNames, values, file });
                    }
                }
                //context.SaveChanges();
            }
            //}
        }
    }
}
