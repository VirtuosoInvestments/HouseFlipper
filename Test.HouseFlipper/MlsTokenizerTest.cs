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
            foreach (var line in lines)
            {
                var tokens = Tokenize(line);
                var actual = instance.GetValues(line);
                Assert.AreEqual(tokens.Count, actual.Count);
                for (int j = 0; j < tokens.Count; j++)
                {
                    Assert.AreEqual(tokens[j], actual[j]);
                }
            }
        }
        [TestMethod]
        public void TokenTest()
        {
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Test.HouseFlipper\data\token-test.csv";
            List<string> allLines = FileHelper.GetLines(path);
            var instance = new MlsTokenizer();
            int currentIndex = 0;
            var line = allLines[currentIndex];

            /*var tokens =*/var actual = Tokenize(line, allLines, currentIndex);
            //var actual = instance.GetValues(line);

            var expected = new List<string>()
            {
                "Hi, I'm billy \",",
                "I have a \"car now\"",
                "\"Didn't you know\"",
                "This is amazing \"\" yes!",
                "\"\"Yella yella \"\"\"",
                "\"NewLine \"\"   continue \"huh  "
            };

            Assert.AreEqual(expected.Count, actual.Count);
            for(int i=0; i<expected.Count; i++)
            {
                var tmp = expected[i];
                var a = actual[i];
                Assert.AreEqual(tmp, a);
            }
        }

        private static List<string> Tokenize(string line, List<string> allLines=null, int? currentIndex=null)
        {
            var tmp = line;
            /*var tokens = tmp.Split(',');
            for (int i = 0; i < tokens.Length; i++)
            {
                tokens[i] = tokens[i].Replace("\"", string.Empty);
            }

            return tokens;*/

            var tokens = new List<string>();
            int quoteCount = 0;
            string word = null;
            char? prevCh = null;
            while (true)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    var ch = line[i];
                    if (ch == '"')
                    {
                        if (prevCh.HasValue && prevCh.Value == '"')
                        {
                            --quoteCount;
                            AddCharacter(ref word, '"');
                            prevCh = null;
                            continue;
                        }
                        else
                        {
                            quoteCount++;
                        }
                    }
                    else if (ch == ',')
                    {
                        if (quoteCount % 2 == 0)
                        {
                            tokens.Add(word);
                            word = null;
                        }
                        else
                        {
                            AddCharacter(ref word, ch);
                        }
                    }
                    else
                    {
                        AddCharacter(ref word, ch);
                    }
                    prevCh = ch;
                }
                if (quoteCount % 2 != 0)
                {
                    if (currentIndex.HasValue && allLines != null && currentIndex.Value < allLines.Count)
                    {
                        ++currentIndex;
                        line = allLines[currentIndex.Value];
                        continue;
                    }
                    else
                    {
                        throw new InvalidOperationException("Something's not right. Incomplete line detected.");
                    }
                }
                else if (word != null)
                {
                    tokens.Add(word);
                }
                break;
            }
            return tokens;
        }

        private static void AddCharacter(ref string word, char ch)
        {
            if(word==null)
            {
                word = string.Empty;
            }
            word += ch;            
        }
    }
}
