using System;
using Hack.HouseFlipper.DataAccess.Csv;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class MlsDataReaderTest
    {
        [Test]
        public void ConstructorNull()
        {
            Assert.Throws(
                typeof(ArgumentException),
                () =>
                {
                    var target = new MlsDataReader(null);
                });
        }

        [Test]
        public void ConstructorNoFileExists()
        {
            var path = @"C:\foo.txt";
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assert.Throws(
                typeof(ArgumentException),
                () =>
                {
                    var target = new MlsDataReader(path);
                });
        }


        [Test]
        public void ConstructorFileExists()
        {
            var path = @"C:\temp\dummy.txt";
            if (!File.Exists(path))
            {
                using(var sw=File.Create(path))
                {                    
                }
            }

            
            var target = new MlsDataReader(path);            
        }

        [Test]
        public void ConstructorFolderNull()
        {
            Assert.Throws(
                typeof(ArgumentException),
                () =>
                {
                    var target = new MlsDataReader(null, "*", SearchOption.AllDirectories);
                });
        }

        [Test]
        public void ConstructorFolderEmpty()
        {
            Assert.Throws(
                typeof(ArgumentException),
                () =>
                {
                    var target = new MlsDataReader(string.Empty, "*", SearchOption.AllDirectories);
                });
        }

        [Test]
        public void ConstructorFolderBlank()
        {
            Assert.Throws(
                typeof(ArgumentException),
                () =>
                {
                    var target = new MlsDataReader("           ", "*", SearchOption.AllDirectories);
                });
        }

        [Test]
        public void ConstructorNoFolderExists()
        {
            var path = @"C:\foodir";
            if(Directory.Exists(path))
            {
                Directory.Delete(path);
            }

            Assert.Throws(
                typeof(ArgumentException),
                () =>
                {
                    var target = new MlsDataReader(path, "*", SearchOption.AllDirectories);
                });
        }

        [Test]
        public void ConstructorEmptyFolderExists()
        {
            var path = @"C:\temp\bardir";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach(var f in Directory.GetFiles(path))
            {
                File.Delete(f);
            }

            var target = new MlsDataReader(path, "*", SearchOption.AllDirectories);

            var fieldInfo=
            target.GetType().GetField(
                "files",
                BindingFlags.NonPublic | BindingFlags.Instance);

            var field = (string[])fieldInfo.GetValue(target);

            Assert.IsTrue(field == null || field.Length == 0);
        }

        [Test]
        public void ConstructorSearchPatternNull()
        {
            var path = @"C:\temp\bardir";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Assert.Throws(
                typeof(ArgumentException),
                () =>
                {
                    var target = new MlsDataReader(path, null, SearchOption.AllDirectories);
                });            
        }

        [Test]
        public void ConstructorSearchPatternEmpty()
        {
            var path = @"C:\temp\bardir";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Assert.Throws(
                typeof(ArgumentException),
                () =>
                {
                    var target = new MlsDataReader(path, string.Empty, SearchOption.AllDirectories);
                });
        }

        [Test]
        public void ConstructorSearchPatternBlank()
        {
            var path = @"C:\temp\bardir";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Assert.Throws(
                typeof(ArgumentException),
                () =>
                {
                    var target = new MlsDataReader(path, "  ", SearchOption.AllDirectories);
                });
        }

        [Test]
        public void ConstructorSearchPatternNonBlank()
        {
            var ext = ".cs";
            var search = "*" + ext;
            var path = @"C:\temp\anydir";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
                       
            Directory.CreateDirectory(path);
            var file1 = Path.Combine(path, "file1.cs");
            var file2 = Path.Combine(path, "file2.txt");
            var file3 = Path.Combine(path, "file3.cs");
            var file4 = Path.Combine(path, "file4.img");

            using (var sw = File.Create(file1)) { }
            using (var sw = File.Create(file2)) { }
            using (var sw = File.Create(file3)) { }
            using (var sw = File.Create(file4)) { }
            
            var target = new MlsDataReader(path, search, SearchOption.AllDirectories);

            var fieldInfo =
            target.GetType().GetField(
                "files",
                BindingFlags.NonPublic | BindingFlags.Instance);

            var field = (string[])fieldInfo.GetValue(target);
            Assert.IsNotNull(field);
            Assert.AreEqual(2, field.Length);
            Assert.AreEqual(file1, field[0]);
            Assert.AreEqual(file3, field[1]);
        }


        [Test]
        public void ConstructorSearchPatternTopLevel()
        {
            var ext = ".cs";
            var search = "*" + ext;
            var path = @"C:\temp\anydir";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);
            var file1 = Path.Combine(path, "file1.cs");
            var file2 = Path.Combine(path, "file2.txt");
            var file3 = Path.Combine(path, Path.Combine("sub","file3.cs"));
            var file4 = Path.Combine(path, "file4.img");

            using (var sw = File.Create(file1)) { }
            using (var sw = File.Create(file2)) { }
            Directory.CreateDirectory(Path.GetDirectoryName(file3));
            using (var sw = File.Create(file3)) { }
            using (var sw = File.Create(file4)) { }

            var target = new MlsDataReader(path, search, SearchOption.TopDirectoryOnly);

            var fieldInfo =
            target.GetType().GetField(
                "files",
                BindingFlags.NonPublic | BindingFlags.Instance);

            var field = (string[])fieldInfo.GetValue(target);
            Assert.IsNotNull(field);
            Assert.AreEqual(1, field.Length);
            Assert.AreEqual(file1, field[0]);            
        }

        [Test]
        public void ConstructorSearchPatternRecursive()
        {
            var ext = ".cs";
            var search = "*" + ext;
            var path = @"C:\temp\anydir";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);
            var file1 = Path.Combine(path, "file1.cs");
            var file2 = Path.Combine(path, "file2.txt");
            var file3 = Path.Combine(path, Path.Combine("sub",Path.Combine("other", "file3.cs")));
            var file4 = Path.Combine(path, "file4.img");

            using (var sw = File.Create(file1)) { }
            using (var sw = File.Create(file2)) { }
            Directory.CreateDirectory(Path.GetDirectoryName(file3));
            using (var sw = File.Create(file3)) { }
            using (var sw = File.Create(file4)) { }

            var target = new MlsDataReader(path, search, SearchOption.AllDirectories);

            var fieldInfo =
            target.GetType().GetField(
                "files",
                BindingFlags.NonPublic | BindingFlags.Instance);

            var field = (string[])fieldInfo.GetValue(target);
            Assert.IsNotNull(field);
            Assert.AreEqual(2, field.Length);
            Assert.AreEqual(file1, field[0]);
            Assert.AreEqual(file3, field[1]);
        }

        [Test]
        public void ReadLineTest()
        {
            var ext = ".cs";
            var search = "*" + ext;
            var path = @"C:\temp\anydir";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);
            var file1 = Path.Combine(path, "file1.cs");
            var file2 = Path.Combine(path, "file2.txt");
            var file3 = Path.Combine(path, Path.Combine("sub", Path.Combine("other", "file3.cs")));
            var file4 = Path.Combine(path, "file4.img");

            var lines = new List<string>();
            var newFileLines = new List<int> { 1, 3 };
            using (var f = File.Create(file1)) 
            { 
                using(var sw = new StreamWriter(f))
                {
                    var line = "bo,diddy,diddly,doo";
                    lines.Add(line);
                    sw.WriteLine(line);

                    line = "shim,shimmy,sham,shah";
                    lines.Add(line);
                    sw.WriteLine(line);
                }
            }
            using (var f = File.Create(file2)) 
            {
                using (var sw = new StreamWriter(f))
                {
                    sw.WriteLine("clackity,clack,click,boo");
                    sw.WriteLine("bergie,fergie,fridge,fries");
                }
            }
            Directory.CreateDirectory(Path.GetDirectoryName(file3));
            using (var f = File.Create(file3)) 
            {
                using (var sw = new StreamWriter(f))
                {
                    var line = "jelly,bean,bandit,dog";
                    lines.Add(line);
                    sw.WriteLine(line);


                    line = "easy,skeezy,dumbs,it";
                    lines.Add(line);
                    sw.WriteLine(line);
                }
            }
            using (var f = File.Create(file4)) 
            {
                using (var sw = new StreamWriter(f))
                {
                    sw.WriteLine("lol,lolly,lick,lop");
                    sw.WriteLine("kitchen,kill,candor,candy");
                }
            }

            var target = new MlsDataReader(path, search, SearchOption.AllDirectories);

            int count = 0;
            foreach (var actual in target.ReadLine())
            {
                ++count;
                var expected = lines[count - 1];
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected, actual.Text);
                var newFile = newFileLines.Contains(count);
                Assert.AreEqual(newFile, actual.NewFile);
            }

            Assert.AreEqual(lines.Count, count);
        }
    }
}
