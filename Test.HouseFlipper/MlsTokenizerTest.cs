using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Hack.HouseFlipper.DataAccess.Csv;

namespace Test.HouseFlipper
{
    [TestClass]
    public class MlsTokenizerTest
    {
        [TestMethod]
        public void GetValues()
        {
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Test.HouseFlipper\data\listing.csv";
            List<string> lines = FileHelper.GetLines(path);
            var instance = new MlsTokenizer();
            foreach(var line in lines)
            {
                var tmp = line;
                var tokens = tmp.Split(',');
                for(int i=0; i<tokens.Length; i++)
                {
                    tokens[i] = tokens[i].Replace("\"", string.Empty);
                }
               var actual = instance.GetValues(line);
                Assert.AreEqual(tokens.Length, actual.Count);
                for(int j=0; j<tokens.Length; j++)
                {
                    Assert.AreEqual(tokens[j], actual[j]);
                }
            }
        }
    }
}
