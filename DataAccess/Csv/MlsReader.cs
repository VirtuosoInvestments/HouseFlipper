﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace HouseFlipper.DataAccess.Csv
{
    public class MlsReader
    {
        private string[] files;

        public string CurrentFile { get; private set; }

        public MlsReader(string dataFolder, string filesSearchPattern, SearchOption searchOption)
        {  
            if(string.IsNullOrWhiteSpace(dataFolder))
            {
                throw new ArgumentException(
                    "Error: dataFolder cannot be null, empty or blank");
            }
            if(!Directory.Exists(dataFolder))
            {
                throw new ArgumentException(
                    string.Format("Error: Directory path does not exist: '{0}'!", dataFolder));
            }

            if(string.IsNullOrWhiteSpace(filesSearchPattern))
            {
                throw new ArgumentException(
                    "Error: filesSearchPattern cannot be null, empty or blank");
            }

            files = Directory.GetFiles(dataFolder, filesSearchPattern, searchOption);
        }

        public MlsReader(string filePath)
        {            
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException(
                    "Error: filePath cannot be null, empty or blank");
            }
            if(!File.Exists(filePath))
            {
                throw new ArgumentException(
                    string.Format("Error: File path does not exist: '{0}'!", filePath));
            }
            files = new string[] { filePath };
        }

        public virtual IEnumerable<MlsRow> ReadLine()
        {
            foreach (var file in files)
            {
                this.CurrentFile = file;
                var newFile = true;
                using (var sr = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read)))
                {
                    string line;                    
                    while ((line = sr.ReadLine()) != null)
                    {
                        if(string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }
                        yield return new MlsRow(line, newFile);
                        newFile = false;
                    }
                }
            }
        }

        public virtual void ReadParallel(Action<string,MlsRow> callback)
        {
            Parallel.ForEach(files, (file) =>
            {
                var newFile = true;
                using (var sr = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.ReadWrite)))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }
                        /*var asyncResult = */callback/*.BeginInvoke*/(file, new MlsRow(line, newFile)/*,null,null*/);
                        //callback.EndInvoke(asyncResult);
                        newFile = false;
                    }
                }
            });            
        }

        public async virtual void ReadParallel2(Action<string, MlsRow> receiveData, Action completed)
        {
            //return Task.Run(async () =>
            //{
                foreach (var file in files)
                {
                    var newFile = true;
                    using (var sr = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.ReadWrite)))
                    {
                        string line;
                        while ((line = await sr.ReadLineAsync()) != null)
                        {
                            if (string.IsNullOrWhiteSpace(line))
                            {
                                continue;
                            }
                            /*var asyncResult = */
                            receiveData/*.BeginInvoke*/(file, new MlsRow(line, newFile)/*,null,null*/);
                            newFile = false;
                        }
                    }
                }//);
            //});
            completed();
        }

        public async virtual void ReadParallel4(Action<string, MlsRow> receiveData, Action completed)
        {
            //return Task.Run(async () =>
            //{
            foreach (var file in files)
            {
                var newFile = true;
                using (var sr = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.ReadWrite)))
                {

                    var line = await sr.ReadLineAsync();
                    while(true)
                    {
                        if(line==null)
                        {
                            break;
                        }
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            line = await sr.ReadLineAsync();
                            continue;
                        }
                        /*var asyncResult = */
                        receiveData.BeginInvoke(file, new MlsRow(line, newFile),
                            /*async*/ (ar)=>
                            {
                                var result = (AsyncResult)ar;
                                var caller = (Action<string, MlsRow>)result.AsyncDelegate;
                                //line = await sr.ReadLineAsync();
                                string l = (string)ar.AsyncState;
                                Console.WriteLine("Got: "+l);
                                caller.EndInvoke(ar);
                            }
                            ,line);
                        newFile = false;
                    }
                }
            }//);
            //});
            completed();
        }

        public async virtual void ReadParallel3(Action<string, MlsRow> receiveData)
        {
            foreach(var file in files)
            {
                var newFile = true;
                using (var sr = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.ReadWrite)))
                {
                    string line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }
                        /*var asyncResult = */
                        //receiveData/*.BeginInvoke*/(file, new MlsRow(line, newFile)/*,null,null*/);

                        /*var asyncResult = */
                        receiveData.BeginInvoke(file, new MlsRow(line, newFile),
                            (ar) =>
                            {
                                var result = (AsyncResult)ar;
                                var caller = (Action<string, MlsRow>)result.AsyncDelegate;

                                caller.EndInvoke(ar);
                            }, 
                            null);
                        newFile = false;
                    }
                }
            }//);
        }

        public virtual void ReadBulk(Action<string, List<MlsRow>> callback)
        {
            Parallel.ForEach(files, (file) =>
            {
                var newFile = true;
                var list = new List<MlsRow>();
                using (var sr = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.ReadWrite)))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }
                        ///callback(file, new MlsRow(line, newFile));
                        list.Add(new MlsRow(line, newFile));
                        newFile = false;
                    }
                }
                if (list != null && list.Count > 0)
                {
                    callback/*.BeginInvoke*/(file, list/*,null,null*/);
                }
            });
        }
    }    
}
