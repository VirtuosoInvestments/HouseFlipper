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

            var rowNum = 0;

            reader.ReadFiles(
                        (file, list) =>
                        {
                            FileProcess(
                                file,
                                list,
                                ref rowNum);
                        });

            timer.Stop();
            Console.WriteLine("Total time: {0} minutes", timer.Elapsed.TotalMinutes);
        }

        private void FileProcess(
                        string file,
                        List<MlsRow> contents,
                        ref int rowNum)
        {
            string[] colNames = null;
            AutoResetEvent allDone = new AutoResetEvent(false);
            long currentTasks = contents.Count-1;

            foreach (var mlsRow in contents)
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
                    Task.Run(() =>
                    {
                        try
                        {
                            //Interlocked.Increment(ref currentTasks);
                            pipe.Enter(new object[] { colNames, values, file });
                        }
                        finally
                        {
                            if (Interlocked.Decrement(ref currentTasks) == 0)
                            {
                                allDone.Set();
                            }
                        }
                    });
                }
            }

            if (Interlocked.Read(ref currentTasks) > 0)
            {
                allDone.WaitOne();
            }
        }
    }
}

