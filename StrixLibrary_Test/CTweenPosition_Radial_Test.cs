using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class CTweenPosition_Radial_Test
    {
        [UnityTest]
        public IEnumerator GetChildIndex_ClosestDirection_Test()
        {
            GameObject pObject = new GameObject("방사형트윈_가까운방향의_자식인덱스구하기");
            CTweenPosition_Radial pTweenTest = pObject.AddComponent<CTweenPosition_Radial>();

            pTweenTest.p_iChildCount = 1;
            pTweenTest.p_fRaidalRangeAngle = 360;
            pTweenTest.p_fRaidalStartAngle = 0;

            Assert.AreEqual(0, pTweenTest.GetChildIndex_ClosestDirection(Vector3.up));

            pTweenTest.p_iChildCount = 4;
            pTweenTest.p_fRaidalRangeAngle = 360;
            pTweenTest.p_fRaidalStartAngle = 0;

            Assert.AreEqual(1, pTweenTest.GetChildIndex_ClosestDirection(Vector3.right));
            Assert.AreEqual(3, pTweenTest.GetChildIndex_ClosestDirection(Vector3.left));

            pTweenTest.p_iChildCount = 4;
            pTweenTest.p_fRaidalRangeAngle = 180;
            pTweenTest.p_fRaidalStartAngle = 0;

            Assert.AreEqual(0, pTweenTest.GetChildIndex_ClosestDirection(Vector3.up));
            Assert.AreEqual(3, pTweenTest.GetChildIndex_ClosestDirection(Vector3.down));

            pTweenTest.p_iChildCount = 8;
            pTweenTest.p_fRaidalRangeAngle = 180;
            pTweenTest.p_fRaidalStartAngle = 90;

            Assert.AreEqual(0, pTweenTest.GetChildIndex_ClosestDirection(Vector3.right));
            Assert.AreEqual(7, pTweenTest.GetChildIndex_ClosestDirection(Vector3.left));

            yield break;
        }
    }

}
