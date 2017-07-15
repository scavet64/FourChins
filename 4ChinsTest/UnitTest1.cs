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
            Assert.AreEqual(FourChins.FourChins.GetTheGet("1542215"), 1);
            Assert.AreEqual(FourChins.FourChins.GetTheGet("1542211"), 2);
            Assert.AreEqual(FourChins.FourChins.GetTheGet("1542000"), 3);
            Assert.AreEqual(FourChins.FourChins.GetTheGet("1540000"), 4);
            Assert.AreEqual(FourChins.FourChins.GetTheGet("1500000"), 5);
            Assert.AreEqual(FourChins.FourChins.GetTheGet("1000000"), 6);
            Assert.AreEqual(FourChins.FourChins.GetTheGet("0000000"), 7);
            Assert.AreEqual(FourChins.FourChins.GetTheGet("00000000"), 8);
            Assert.AreEqual(FourChins.FourChins.GetTheGet("000000000"), 9);
        }
    }
}
