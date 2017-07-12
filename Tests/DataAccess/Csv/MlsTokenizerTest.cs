using System;
using System.Collections.Generic;
using HouseFlipper.DataAccess.Csv;
using NUnit.Framework;
using Test.HouseFlipper.Common;

namespace Test.HouseFlipper.DataAccess
{
    [TestFixture]
    public class MlsTokenizerTest
    {
        [Test]
        public void Split()
        {
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Tests\WebSite\data\listing.csv";
            List<string> lines = FileHelper.GetLines(path);
            foreach (var line in lines)
            {
                var tokens = Tokenize(line);
                var actual = MlsTokenizer.Split(line);
                Assert.AreEqual(tokens.Count, actual.Length);
                for (int j = 0; j < tokens.Count; j++)
                {
                    Assert.AreEqual(tokens[j], actual[j]);
                }
            }
        }
        [Test]
        public void TokenTest()
        {
            var path = @"C:\Users\ralph.joachim\Documents\Visual Studio 2015\Projects\HouseFlipper\Tests\WebSite\data\token-test.csv";
            List<string> allLines = FileHelper.GetLines(path);
            int currentIndex = 0;
            var line = allLines[currentIndex];

            var actual = Tokenize(line, allLines, currentIndex);

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
