using HouseFlipper.DataAccess.Csv;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using Test.HouseFlipper.Common;

namespace Test.HouseFlipper.DataAccess
{
    [TestFixture]
    public class MlsDataReaderTest
    {
        [Test]
        public void Read()
        {
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Test.HouseFlipper\data\listing.csv";
            List<string> lines = FileHelper.GetLines(path);
            var instance = new MlsReader(path);
            var collection = instance.ReadLine();
            int count = 0;
            foreach (var line in collection)
            {
                ++count;
                if (count == 1)
                {
                    Assert.IsTrue(line.IsHeader);                    
                }
                else
                {
                    Assert.IsFalse(line.IsHeader);
                }
                Assert.AreEqual(lines[count-1], line.Text);
            }
            Assert.AreEqual(lines.Count, count);
        }

        [Test]
        public void ReadFolder()
        {
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Test.HouseFlipper\data";
            List<List<string>> all = FileHelper.GetFiles(path);
            var instance = new MlsReader(path, "*.csv", SearchOption.AllDirectories);
            var collection = instance.ReadLine();
            int fNum = 0;
            int lineNumberInFile = 0;
            int totalLinesRead = 0;
            var lines = all[fNum];
            
            foreach (var line in collection)
            {                                               
                if (totalLinesRead==0 || lineNumberInFile == lines.Count)
                {
                    Assert.IsTrue(line.IsHeader);
                    if (totalLinesRead > 0)
                    {
                        Assert.AreEqual(lines.Count, lineNumberInFile);
                        fNum++;
                        lineNumberInFile = 0;
                        lines = all[fNum];
                    }
                }
                else
                {
                    Assert.IsFalse(line.IsHeader);
                }
                Assert.AreEqual(lines[lineNumberInFile], line.Text);
                ++lineNumberInFile;
                ++totalLinesRead;
            }
            Assert.AreEqual(lines.Count, lineNumberInFile);
        }        
    }
}
