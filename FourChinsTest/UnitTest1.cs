using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _4ChinsTest
{
    [TestClass]
    public class ChinsTest
    {
        [TestMethod]
        public void TestGetsGetter()
        {
            FourChins.FourChins fourchin = new FourChins.FourChins();

            Assert.AreEqual(fourchin.GetTheGet("1542215"), 1);
            Assert.AreEqual(fourchin.GetTheGet("1542211"), 2);
            Assert.AreEqual(fourchin.GetTheGet("1542000"), 3);
            Assert.AreEqual(fourchin.GetTheGet("1540000"), 4);
            Assert.AreEqual(fourchin.GetTheGet("1500000"), 5);
            Assert.AreEqual(fourchin.GetTheGet("1000000"), 6);
            Assert.AreEqual(fourchin.GetTheGet("0000000"), 7);
            Assert.AreEqual(fourchin.GetTheGet("00000000"), 8);
            Assert.AreEqual(fourchin.GetTheGet("000000000"), 9);
        }
    }
}
