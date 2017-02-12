using Hack.HouseFlipper.DataAccess.Csv;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class MlsDataLineTest
    {
        [Test]
        public void ConstructorTextNull()
        {
            string expected = null;
            var target = new MlsDataLine(expected, true);
            Assert.AreEqual(expected, target.Text);
        }

        [Test]
        public void ConstructorTextEmpty()
        {
            string expected = string.Empty;
            var target = new MlsDataLine(expected, true);
            Assert.AreEqual(expected, target.Text);
        }

        [Test]
        public void ConstructorTextBlank()
        {
            string expected = "         ";
            var target = new MlsDataLine(expected, true);
            Assert.AreEqual(expected, target.Text);
        }

        [Test]
        public void ConstructorText()
        {
            string expected = "---- many thanks to alll  %%%@1432 ";
            var target = new MlsDataLine(expected, true);
            Assert.AreEqual(expected, target.Text);
        }

        [Test]
        public void ConstructorTextWithNewLine()
        {
            string expected = "---- many thanks to alll  %%%@1432 \r\n river walk dance \t !!!"; ;
            var target = new MlsDataLine(expected, true);
            Assert.AreEqual(expected, target.Text);
        }

        [Test]
        public void ConstructorNewFileTrue()
        {
            var expected = true;
            var target = new MlsDataLine(string.Empty, expected);
            Assert.AreEqual(expected, target.NewFile);
        }

        [Test]
        public void ConstructorNewFileFalse()
        {
            var expected = false;
            var target = new MlsDataLine(string.Empty, expected);
            Assert.AreEqual(expected, target.NewFile);
        }
    }
}
