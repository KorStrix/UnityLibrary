using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class CRangeDictionary_Test
    {
        [Test]
        public void CRangeDictionary_Dont_Add_OverlapRange()
        {
            CRangeDictionary<int, string> rangeDictionary = new CRangeDictionary<int, string>();

            Assert.IsTrue(rangeDictionary.Add(1, 10, "1~10"));
            Assert.IsFalse(rangeDictionary.Add(5, 10, "5~10")); // Fail
            Assert.IsTrue(rangeDictionary.Add(11, 20, "11~20"));

            Assert.IsTrue(rangeDictionary.Remove(1, 10));
            Assert.IsTrue(rangeDictionary.Add(5, 10, "5~10")); // 위에선 실패했으나 이제 성공
        }

        [Test]
        public void CRangeDictionary_Is_Working()
        {
            CRangeDictionary<int, string> rangeDictionary = new CRangeDictionary<int, string>();

            Assert.IsTrue(rangeDictionary.Add(-10, 0, "-10~0"));
            Assert.IsTrue(rangeDictionary.Add(1, 10, "1~10"));
            Assert.IsTrue(rangeDictionary.Add(11, 20, "11~20"));

            // True Case
            for (int i = -10; i <= 0; i++)
                Assert.IsTrue(rangeDictionary.GetValue(i) == "-10~0");

            for (int i = 1; i <= 10; i++)
                Assert.IsTrue(rangeDictionary.GetValue(i) == "1~10");

            for (int i = 11; i <= 20; i++)
                Assert.IsTrue(rangeDictionary.GetValue(i) == "11~20");


            // False Case
            for (int i = 0; i < 10; i++)
                Assert.IsTrue(rangeDictionary.GetValue(UnityEngine.Random.Range(11, 100000)) != "1~10");

            // Null Case
            for (int i = 0; i < 10; i++)
                Assert.IsTrue(rangeDictionary.GetValue(UnityEngine.Random.Range(21, 100000)) == null);
        }
    }

}
