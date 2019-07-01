using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class PrimitiveHelper_Test
    {
        [System.Flags]
        public enum ETestEnumFlag
        {
            Test1 = 1 << 1,
            Test2 = 1 << 2,
            Test3 = 1 << 3,
            Test4 = 1 << 4,
            Test5 = 1 << 5,

        }

        [Test]
        public void Test_EnumExtension_ContainEnumFlag()
        {
            ETestEnumFlag eTestEnum = ETestEnumFlag.Test1 | ETestEnumFlag.Test2 | ETestEnumFlag.Test3;
            Assert.IsTrue(eTestEnum.ContainEnumFlag(ETestEnumFlag.Test1));
            Assert.IsTrue(eTestEnum.ContainEnumFlag(ETestEnumFlag.Test2));
            Assert.IsTrue(eTestEnum.ContainEnumFlag(ETestEnumFlag.Test3));

            Assert.IsTrue(eTestEnum.ContainEnumFlag(ETestEnumFlag.Test1, ETestEnumFlag.Test2, ETestEnumFlag.Test4));


            Assert.IsFalse(eTestEnum.ContainEnumFlag(ETestEnumFlag.Test4));
            Assert.IsFalse(eTestEnum.ContainEnumFlag(ETestEnumFlag.Test4, ETestEnumFlag.Test5));

        }

        [Test]
        static public void Test_IntExtension_CutDigitString_Number([Random(1000, 10000, 5)] int iTestNum)
        {
            System.Text.StringBuilder pStrBuilder = new System.Text.StringBuilder();
            List<int> listTest = iTestNum.CutDigitString_Number();
            for (int i = 0; i < listTest.Count; i++)
                pStrBuilder.Append(listTest[i]);

            Assert.IsTrue(pStrBuilder.ToString() == iTestNum.ToString());
        }

        [Test]
        static public void Test_IntExtension_CutDigitString([Random(1000, 10000000, 5)] int iTestNum)
        {
            System.Text.StringBuilder pStrBuilder = new System.Text.StringBuilder();
            List<string> listTest = iTestNum.CutDigitString();
            for (int i = 0; i < listTest.Count; i++)
                pStrBuilder.Append(listTest[i]);

            Assert.IsTrue(pStrBuilder.ToString() == iTestNum.ToString());
        }

        [Test]
        static public void Test_IntExtension_CutDigitString_WithComma([Random(1000, 10000000, 5)] int iTestNum)
        {
            System.Text.StringBuilder pStrBuilder = new System.Text.StringBuilder();
            List<string> listTest = iTestNum.CutDigitString_WithComma();
            for (int i = 0; i < listTest.Count; i++)
                pStrBuilder.Append(listTest[i]);

            Assert.IsTrue(pStrBuilder.ToString() == iTestNum.ToStringString_WithComma());
        }

        public enum ETestCase_FloatExtension
        {
            Similar,
            NotSimilar
        }

        [Test]
        static public void Test_FloatExtension_IsSimilar()
        {
            System.Random pRandom = new System.Random();
            float fTestNum = (float)pRandom.NextDouble();
            float fTestSimlarGap = (float)pRandom.NextDouble();

            ETestCase_FloatExtension eRandomTest = (ETestCase_FloatExtension)(pRandom.Next() % 2);
            if (eRandomTest == ETestCase_FloatExtension.NotSimilar)
            {
                float fSimilarValueNot = fTestNum + (fTestSimlarGap * 2f);
                Assert.IsFalse(fTestNum.IsSimilar(fSimilarValueNot, fTestSimlarGap));
            }
            else if (eRandomTest == ETestCase_FloatExtension.Similar)
            {
                float fSimilarValue = fTestNum + (fTestSimlarGap * 0.9f);
                Assert.IsTrue(fTestNum.IsSimilar(fSimilarValue, fTestSimlarGap));
            }
        }

        [Test]
        public void Test_Vector3Extension_InverseLerp()
        {
            Vector3 vecStart = Vector3.one * Random.Range(0, 10);
            Vector3 vecDest = Vector3.one * Random.Range(0, 1000);

            float fTestLerp = Random.Range(0f, 1f);
            Vector3 vecTest = Vector3.Lerp(vecStart, vecDest, fTestLerp);
            float fInverseLerp = vecTest.InverseLerp_0_1(vecStart, vecDest);

            Debug.Log("vecStart : " + vecStart + " vecDest : " + vecDest + " fTestLerp : " + fTestLerp + " fInverseLerp : " + fInverseLerp);
            Assert.AreEqual(fTestLerp.ToString("F2"), fInverseLerp.ToString("F2"));

            // Start와 Dest가 스왑되면 Lerp값도 다르다.
            Vector3 vecTest2 = Vector3.Lerp(vecDest, vecStart, fTestLerp);
            float fInverseLerp2 = vecTest2.InverseLerp_0_1(vecDest, vecStart);

            Debug.Log("vecStart : " + vecDest + " vecDest : " + vecStart + " fTestLerp : " + fTestLerp + " fInverseLerp : " + fInverseLerp2);
            Assert.AreEqual(fTestLerp.ToString("F2"), fInverseLerp2.ToString("F2"));

        }

        public List<int> GetSomthingList_Garbage()
        {
            List<int> listReturn = new List<int>();
            // Logic..

            return listReturn;
        }

        //========================================

        List<int> listTemp = new List<int>();

        public List<int> GetSomthingList_NoneGarbage()
        {
            listTemp.Clear();
            // Logic..

            return listTemp;
        }

    }

}
