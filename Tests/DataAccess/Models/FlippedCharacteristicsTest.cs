using Hack.HouseFlipper.DataAccess.Models;
using NUnit.Framework;
using System;

namespace Tests.DataAccess.Models
{
    [TestFixture]
    public class FlippedCharacteristicsTest
    {
        [Test]
        public void DefaultConstructor()
        {
            var target = new FlippedCharacteristics();
            Assert.IsNull(target.Zipcode);
            Assert.IsNull(target.PreFlip);
            Assert.IsNull(target.PostFlip);
            Assert.IsNull(target.Profit);
            Assert.AreEqual(0, target.MinYear);
            Assert.AreEqual(0, target.MaxYear);
            Assert.AreEqual(0, target.MinSqft);
            Assert.AreEqual(0, target.MaxSqft);
        }

        [Test]
        public void ConstructorNull()
        {
            var target = new FlippedCharacteristics(null, null, null);
            Assert.IsNull(target.Zipcode);
            Assert.IsNull(target.PreFlip);
            Assert.IsNull(target.PostFlip);
        }

        [Test]
        public void ConstructorZipEmpty()
        {
            var zip = string.Empty;
            var target = new FlippedCharacteristics(zip, null, null);
            Assert.AreEqual(zip,target.Zipcode);
            Assert.IsNull(target.PreFlip);
            Assert.IsNull(target.PostFlip);
        }

        [Test]
        public void ConstructorZipBlank()
        {
            int num = new Random().Next(1, 11);
            var zip = string.Empty.PadLeft(num);
            var target = new FlippedCharacteristics(zip, null, null);
            Assert.AreEqual(zip, target.Zipcode);
            Assert.IsNull(target.PreFlip);
            Assert.IsNull(target.PostFlip);
        }

        [Test]
        public void ConstructorZipPlusFour()
        {
            var zip = "32825-2245";
            var target = new FlippedCharacteristics(zip, null, null);
            Assert.AreEqual(zip, target.Zipcode);
            Assert.IsNull(target.PreFlip);
            Assert.IsNull(target.PostFlip);
        }

        [Test]
        public void ConstructorInvalidZipWithEndDash()
        {
            var zip = "32825-";
            var target = new FlippedCharacteristics(zip, null, null);
            Assert.AreEqual(zip, target.Zipcode);
            Assert.IsNull(target.PreFlip);
            Assert.IsNull(target.PostFlip);
        }

        [Test]
        public void ConstructorInvalidZipWithStartDash()
        {
            var zip = "-32825";
            var target = new FlippedCharacteristics(zip, null, null);
            Assert.AreEqual(zip, target.Zipcode);
            Assert.IsNull(target.PreFlip);
            Assert.IsNull(target.PostFlip);
        }

        [Test]
        public void ConstructorInvalidZipWithMiddleDash()
        {
            var zip = "32-825";
            var target = new FlippedCharacteristics(zip, null, null);
            Assert.AreEqual(zip, target.Zipcode);
            Assert.IsNull(target.PreFlip);
            Assert.IsNull(target.PostFlip);
        }

        [Test]
        public void ConstructorInvalidZipWithMultiDash()
        {
            var zip = "32-825-";
            var target = new FlippedCharacteristics(zip, null, null);
            Assert.AreEqual(zip, target.Zipcode);
            Assert.IsNull(target.PreFlip);
            Assert.IsNull(target.PostFlip);
        }

        [Test]
        public void ConstructorInvalidZipWithChar()
        {
            var zip = "938293io232";
            var target = new FlippedCharacteristics(zip, null, null);
            Assert.AreEqual(zip, target.Zipcode);
            Assert.IsNull(target.PreFlip);
            Assert.IsNull(target.PostFlip);
        }

        [Test]
        public void ConstructorInvalidZipTooLarge()
        {
            var zip = "938293000232";
            var target = new FlippedCharacteristics(zip, null, null);
            Assert.AreEqual(zip, target.Zipcode);
            Assert.IsNull(target.PreFlip);
            Assert.IsNull(target.PostFlip);

            zip = "938293";
            target = new FlippedCharacteristics(zip, null, null);
            Assert.AreEqual(zip, target.Zipcode);
            Assert.IsNull(target.PreFlip);
            Assert.IsNull(target.PostFlip);
        }

        [Test]
        public void ConstructorPreflipNotNull()
        {
            var zip = "32825-2245";
            var preflip = new HouseCharacteristics();
            var target = new FlippedCharacteristics(zip, preflip, null);
            Assert.AreEqual(zip, target.Zipcode);
            Assert.IsNotNull(target.PreFlip);
            Assert.AreSame(preflip,target.PreFlip);
            Assert.IsNull(target.PostFlip);
        }

        [Test]
        public void ConstructorPostflipNotNull()
        {
            var zip = "32825-2245";
            var preflip = new HouseCharacteristics();
            var postflip = new HouseCharacteristics();
            var target = new FlippedCharacteristics(zip, preflip, postflip);
            Assert.AreEqual(zip, target.Zipcode);
            Assert.IsNotNull(target.PreFlip);
            Assert.AreSame(preflip, target.PreFlip);
            Assert.IsNotNull(target.PostFlip);
            Assert.AreSame(postflip, target.PostFlip);
        }

        [Test]
        public void ProfitValue()
        {
            var random = new Random();
            var price1 = random.Next(1, 30000001);
            var price2 = random.Next(price1, 30000001);
            var zip = "32897";
            var preflip = new HouseCharacteristics()
            {
                Price = price1
            };
            var postflip = new HouseCharacteristics()
            {
                Price = price2
            };
            var target = new FlippedCharacteristics(zip, preflip, postflip);
            var actual = target.ProfitValue();
            Assert.IsTrue(actual >= 0);
            Assert.AreEqual(price2 - price1, actual);

            price1 = random.Next(1, 30000001);
            do
            {
                price2 = random.Next(1, price1);
            } 
            while (price2 > price1);

            zip = "32897";
            preflip = new HouseCharacteristics()
            {
                Price = price1
            };
            postflip = new HouseCharacteristics()
            {
                Price = price2
            };
            target = new FlippedCharacteristics(zip, preflip, postflip);
            actual = target.ProfitValue();
            Assert.IsTrue(actual < 0);
            Assert.AreEqual(price2 - price1, actual);
        }

        [Test]
        public void ProfitPreflipNull()
        {
            var random = new Random();
            var price1 = random.Next(1, 30000001);
            var price2 = random.Next(price1, 30000001);
            var zip = "32897";
            HouseCharacteristics preflip = null;
            var postflip = new HouseCharacteristics()
            {
                Price = price2
            };
            var target = new FlippedCharacteristics(zip, preflip, postflip);
            Assert.IsNull(target.Profit);
        }

        [Test]
        public void ProfitPostflipNull()
        {
            var random = new Random();
            var price1 = random.Next(1, 30000001);
            var price2 = random.Next(price1, 30000001);
            var zip = "32897";
            HouseCharacteristics postflip = null;
            var preflip = new HouseCharacteristics()
            {
                Price = price2
            };
            var target = new FlippedCharacteristics(zip, preflip, postflip);
            Assert.IsNull(target.Profit);
        }

        [Test]
        public void Profit()
        {            
            double price1 = 26564313;
            double price2 = 27569105;
            double profit = 1004792;
            var profitStr = "$1,004,792";
            var zip = "32897";
            var preflip = new HouseCharacteristics()
            {
                Price = price1
            };
            var postflip = new HouseCharacteristics()
            {
                Price = price2
            };
            Console.WriteLine("Price1: " + price1);
            Console.WriteLine("Price2: " + price2);
            Console.WriteLine("Profit: " + profit);
            Console.WriteLine();
            var target = new FlippedCharacteristics(zip, preflip, postflip);
            var actual = target.ProfitValue();
            var actual2 = target.Profit;
            Assert.IsTrue(actual >= 0);
            Assert.AreEqual(profit, actual);
            Assert.AreEqual(profitStr, actual2);

            price1 = 21254890;
            price2 = 17307973.20;                 
            profit = price2 - price1;
            profitStr = "-$3,946,916.80";
            zip = "32897";
            preflip = new HouseCharacteristics()
            {
                Price = price1
            };
            postflip = new HouseCharacteristics()
            {
                Price = price2
            };
            Console.WriteLine("Price1: " + price1);
            Console.WriteLine("Price2: " + price2);
            Console.WriteLine("Profit: " + profit);
            Console.WriteLine();
            target = new FlippedCharacteristics(zip, preflip, postflip);
            actual = target.ProfitValue();
            actual2 = target.Profit;
            Assert.IsTrue(actual < 0);
            Assert.AreEqual(profit, actual);
            Assert.AreEqual(profitStr, actual2);
        }

        [Test]
        public void CompareTo()
        {
            var random = new Random();
            var price1 = random.Next(1, 30000001);
            var price2 = random.Next(price1, 30000001);
            var zip = "32897";
            var preflip = new HouseCharacteristics()
            {
                Price = price1
            };
            var postflip = new HouseCharacteristics()
            {
                Price = price2
            };
            var target1 = new FlippedCharacteristics(zip, preflip, postflip);
            var actual1 = target1.ProfitValue();
            Assert.IsTrue(actual1 >= 0);
            Assert.AreEqual(price2 - price1, actual1);

            price1 = random.Next(1, 30000001);
            do
            {
                price2 = random.Next(1, price1);
            }
            while (price2 > price1);

            zip = "32897";
            preflip = new HouseCharacteristics()
            {
                Price = price1
            };
            postflip = new HouseCharacteristics()
            {
                Price = price2
            };
            var target2 = new FlippedCharacteristics(zip, preflip, postflip);
            var actual2 = target2.ProfitValue();
            Assert.IsTrue(actual2 < 0);
            Assert.AreEqual(price2 - price1, actual2);

            Assert.AreEqual(0, target1.CompareTo(target1));
            Assert.AreEqual(1, target1.CompareTo(null));
            Assert.AreEqual(1, target1.CompareTo(string.Empty));


            Assert.AreEqual(1, target1.CompareTo(target2));
            Assert.AreEqual(-1, target2.CompareTo(target1));
        }
    }
}
