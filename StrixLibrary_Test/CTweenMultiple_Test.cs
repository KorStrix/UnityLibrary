using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class CTweenMultiple_Test : CTweenMultiplePosition
    {
        [Test]
        public void Test_CalculateTween_Delta()
        {
            CTweenMultiple_Test pMultiplePosition;
            InitTweenTest(out pMultiplePosition);

            float fTotalTime = pMultiplePosition.Calculate_TotalDuration();
            float fProgress_0_1 = (fTotalTime / 2f) / fTotalTime;
            float fProgress_Calculated = 0f;

            pMultiplePosition.Calculate_CurrentTweenData(0f, out fProgress_Calculated);
            Assert.AreEqual(pMultiplePosition.p_iPlayIndex_Last, 0);

            float fProgress_Start_1 = pMultiplePosition.p_listTweenData[0].fDurationSec / fTotalTime;

            pMultiplePosition.Calculate_CurrentTweenData(fProgress_Start_1 + 0.001f, out fProgress_Calculated);
            Assert.AreEqual(pMultiplePosition.p_iPlayIndex_Last, 1);

            float fProgress_Start_2 = pMultiplePosition.p_listTweenData[1].fDurationSec / fTotalTime;
            fProgress_Start_2 += fProgress_Start_1;

            pMultiplePosition.Calculate_CurrentTweenData(fProgress_Start_2 + 0.001f, out fProgress_Calculated);
            Assert.AreEqual(pMultiplePosition.p_iPlayIndex_Last, 2);
        }

        [Test]
        public void Test_CalculateTween_Index()
        {
            CTweenMultiple_Test pMultiplePosition;
            InitTweenTest(out pMultiplePosition);

            float fDetlaTime = Time.deltaTime;
            for (int i = 0; i < 3; i++)
            {
                pMultiplePosition.CalculateProgress_ByIndex(i);
                pMultiplePosition.DoSetTweening(0f);
                Assert.AreEqual(pMultiplePosition.transform.position.ToString("F1"), pMultiplePosition.p_listTweenData[i].vecPosition_Start.ToString("F1"));
            }
        }

        private static void InitTweenTest(out CTweenMultiple_Test pMultiplePosition)
        {
            GameObject pObject = new GameObject();
            pMultiplePosition = pObject.AddComponent<CTweenMultiple_Test>();
            pMultiplePosition.transform.position = Vector3.zero;

            float fTotalTime = 0f;
            for (int i = 0; i < 3; i++)
            {
                TweenData pTweenData = new TweenData();
                pTweenData.vecPosition_Start = Vector3.one * i;
                pTweenData.vecPosition_Dest = Vector3.one * (i + 1);

                pTweenData.fDurationSec = i + 1;
                pMultiplePosition.p_listTweenData.Add(pTweenData);

                fTotalTime += pTweenData.fDurationSec;
            }
            pMultiplePosition.DoSetTarget(pObject);

            Assert.AreEqual(fTotalTime, pMultiplePosition.Calculate_TotalDuration());
        }
    }

}
