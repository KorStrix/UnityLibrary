using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class SHugeNumber_Test
    {
        [Test]
        public void HugeNumber_ToStringTest()
        {
            int iValue = 1000;
            string striValueString = "1A";

            HugeNumber sHugeNumber = new HugeNumber(iValue);
            Assert.IsTrue(sHugeNumber.ToString() == striValueString);
        }
    }
}
