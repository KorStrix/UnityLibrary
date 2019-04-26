using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CManagerPooling_Component_Test
    {
        public enum ETestPoolingObjectName
        {
            Test1,
            Test2,

            Max,
        }

        public class TestPoolingObject : MonoBehaviour
        {
            static protected Dictionary<ETestPoolingObjectName, int> g_mapActiveCount;
            public ETestPoolingObjectName eTestType;

            static public void ResetActiveCount()
            {
                g_mapActiveCount = new Dictionary<ETestPoolingObjectName, int>() { { ETestPoolingObjectName.Test1, 0 }, { ETestPoolingObjectName.Test2, 0 } };
            }

            static public int GetActiveCount(ETestPoolingObjectName eTestPoolingObjectName)
            {
                return g_mapActiveCount[eTestPoolingObjectName];
            }

            private void OnEnable() { g_mapActiveCount[eTestType]++; }
            private void OnDisable() { g_mapActiveCount[eTestType]--; }
        }

        [UnityTest]
        [Category("StrixLibrary")]
        public IEnumerator WorkingTest()
        {
            CManagerPooling_Component<TestPoolingObject> pPoolingManager = CManagerPooling_Component<TestPoolingObject>.instance;
            Dictionary<ETestPoolingObjectName, TestPoolingObject> mapObjectInstance = InitTest();

            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test1));
            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test2));


            // Test1
            List<TestPoolingObject> listObjectPooling = new List<TestPoolingObject>();
            for (int i = 0; i < 10; i++)
                listObjectPooling.Add(pPoolingManager.DoPop(mapObjectInstance[ETestPoolingObjectName.Test1]));

            Assert.AreEqual(10, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test1));
            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test2));

            for (int i = 0; i < listObjectPooling.Count; i++)
                pPoolingManager.DoPush(listObjectPooling[i]);

            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test1));
            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test2));


            // Test2
            listObjectPooling.Clear();
            for (int i = 0; i < 5; i++)
                listObjectPooling.Add(pPoolingManager.DoPop(mapObjectInstance[ETestPoolingObjectName.Test2]));

            // Active Check
            for (int i = 0; i < listObjectPooling.Count; i++)
                Assert.AreEqual(true, listObjectPooling[i].gameObject.activeSelf);

            Assert.AreEqual(5, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test2));

            for (int i = 0; i < listObjectPooling.Count; i++)
                pPoolingManager.DoPush(listObjectPooling[i]);

            // Active Check - 리턴했기 때문에 False
            for (int i = 0; i < listObjectPooling.Count; i++)
                Assert.AreEqual(false, listObjectPooling[i].gameObject.activeSelf);

            Assert.AreEqual(0, TestPoolingObject.GetActiveCount(ETestPoolingObjectName.Test2));

            yield break;
        }

        private Dictionary<ETestPoolingObjectName, TestPoolingObject> InitTest()
        {
            TestPoolingObject.ResetActiveCount();

            Dictionary<ETestPoolingObjectName, TestPoolingObject> mapObjectPooling = new Dictionary<ETestPoolingObjectName, TestPoolingObject>();
            for (int i = 0; i < (int)ETestPoolingObjectName.Max; i++)
            {
                ETestPoolingObjectName eTest = (ETestPoolingObjectName)i;
                GameObject pObjectOrigin_Test = new GameObject(eTest.ToString());
                pObjectOrigin_Test.gameObject.SetActive(false);

                TestPoolingObject pTestPoolingObject = pObjectOrigin_Test.AddComponent<TestPoolingObject>();
                pTestPoolingObject.eTestType = eTest;
                mapObjectPooling.Add(eTest, pTestPoolingObject);
            }

            return mapObjectPooling;
        }
    }
}
