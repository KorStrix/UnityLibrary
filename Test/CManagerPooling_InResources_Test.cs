using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    [Category("StrixLibrary")]
    public class CManagerPooling_InResources_Test
    {
        public enum ETestPoolingObjectName
        {
            Test1,
            Test2,
            Test3,
        }

        public class TestPoolingObject : MonoBehaviour
        {
            static public Dictionary<ETestPoolingObjectName, int> g_mapActiveCount;
            public ETestPoolingObjectName eTestType;

            private void OnEnable() { g_mapActiveCount[eTestType]++; }
            private void OnDisable() { g_mapActiveCount[eTestType]--; }
        }

        [UnityTest]
        public IEnumerator Working_Test()
        {
            // 풀링 매니져를 Init합니다.
            CManagerPooling_InResources<ETestPoolingObjectName, TestPoolingObject> pPoolingManager = InitGeneratePoolingTest();

            Assert.AreEqual(0, TestPoolingObject.g_mapActiveCount[ETestPoolingObjectName.Test1]);
            Assert.AreEqual(0, TestPoolingObject.g_mapActiveCount[ETestPoolingObjectName.Test2]);

            List<GameObject> listObjectPooling = new List<GameObject>();
            for (int i = 0; i < 10; i++)
                listObjectPooling.Add(pPoolingManager.DoPop(ETestPoolingObjectName.Test1).gameObject);
            Assert.AreEqual(10, TestPoolingObject.g_mapActiveCount[ETestPoolingObjectName.Test1]);

            for (int i = 0; i < 10; i++)
                pPoolingManager.DoPush(listObjectPooling[i].GetComponent<TestPoolingObject>());
            Assert.AreEqual(0, TestPoolingObject.g_mapActiveCount[ETestPoolingObjectName.Test1]);

            listObjectPooling.Clear();
            for (int i = 0; i < 5; i++)
                listObjectPooling.Add(pPoolingManager.DoPop(ETestPoolingObjectName.Test2).gameObject);
            Assert.AreEqual(5, TestPoolingObject.g_mapActiveCount[ETestPoolingObjectName.Test2]);

            pPoolingManager.DoPushAll();
            Assert.AreEqual(0, TestPoolingObject.g_mapActiveCount[ETestPoolingObjectName.Test2]);

            yield break;
        }

        static public Dictionary<ETestPoolingObjectName, int> g_mapMakeCount;
        static public Dictionary<ETestPoolingObjectName, int> g_mapPopCount;
        static public Dictionary<ETestPoolingObjectName, int> g_mapPushCount;

        [UnityTest]
        public IEnumerator Event_Test()
        {
            g_mapMakeCount = new Dictionary<ETestPoolingObjectName, int>() { { ETestPoolingObjectName.Test3, 0 } };
            g_mapPopCount = new Dictionary<ETestPoolingObjectName, int>() { { ETestPoolingObjectName.Test3, 0 } };
            g_mapPushCount = new Dictionary<ETestPoolingObjectName, int>() { { ETestPoolingObjectName.Test3, 0 } };
            CManagerPooling_InResources<ETestPoolingObjectName, TestPoolingObject> pPoolingManager = InitGeneratePoolingTest();

            pPoolingManager.p_EVENT_OnMakeResource += PPoolingManager_p_EVENT_OnMakeResource;
            pPoolingManager.p_EVENT_OnPopResource += PPoolingManager_p_EVENT_OnPopResource;
            pPoolingManager.p_EVENT_OnPushResource += PPoolingManager_p_EVENT_OnPushResource;

            int iTotalMakeCount = Random.Range(15, 50);
            for (int i = 0; i < iTotalMakeCount; i++)
                pPoolingManager.DoPop(ETestPoolingObjectName.Test3);

            Assert.AreEqual(g_mapMakeCount[ETestPoolingObjectName.Test3], iTotalMakeCount);
            Assert.AreEqual(g_mapPopCount[ETestPoolingObjectName.Test3], iTotalMakeCount);
            Assert.AreEqual(g_mapPushCount[ETestPoolingObjectName.Test3], 0);

            pPoolingManager.DoPushAll();

            Assert.AreEqual(g_mapMakeCount[ETestPoolingObjectName.Test3], iTotalMakeCount);
            Assert.AreEqual(g_mapPopCount[ETestPoolingObjectName.Test3], iTotalMakeCount);
            Assert.AreEqual(g_mapPushCount[ETestPoolingObjectName.Test3], iTotalMakeCount);

            for (int i = 0; i < iTotalMakeCount; i++)
                pPoolingManager.DoPop(ETestPoolingObjectName.Test3);

            Assert.AreEqual(g_mapMakeCount[ETestPoolingObjectName.Test3], iTotalMakeCount);
            Assert.AreEqual(g_mapPopCount[ETestPoolingObjectName.Test3], iTotalMakeCount * 2);
            Assert.AreEqual(g_mapPushCount[ETestPoolingObjectName.Test3], iTotalMakeCount);

            pPoolingManager.DoPushAll();

            Assert.AreEqual(g_mapMakeCount[ETestPoolingObjectName.Test3], iTotalMakeCount);
            Assert.AreEqual(g_mapPopCount[ETestPoolingObjectName.Test3], iTotalMakeCount * 2);
            Assert.AreEqual(g_mapPushCount[ETestPoolingObjectName.Test3], iTotalMakeCount * 2);

            yield break;
        }

        private void PPoolingManager_p_EVENT_OnMakeResource(ETestPoolingObjectName arg1, TestPoolingObject arg2) { g_mapMakeCount[arg1]++; }
        private void PPoolingManager_p_EVENT_OnPushResource(ETestPoolingObjectName arg1, TestPoolingObject arg2) { g_mapPushCount[arg1]++; }
        private void PPoolingManager_p_EVENT_OnPopResource(ETestPoolingObjectName arg1, TestPoolingObject arg2) { g_mapPopCount[arg1]++; }

        private CManagerPooling_InResources<ETestPoolingObjectName, TestPoolingObject> InitGeneratePoolingTest()
        {
            TestPoolingObject.g_mapActiveCount = new Dictionary<ETestPoolingObjectName, int>() { { ETestPoolingObjectName.Test1, 0 }, { ETestPoolingObjectName.Test2, 0 }, { ETestPoolingObjectName.Test3, 0 } };

            List<GameObject> listObjectPooling = new List<GameObject>();
            for (int i = 0; i < 3; i++)
            {
                ETestPoolingObjectName eTest = (ETestPoolingObjectName)i;
                GameObject pObjectOrigin_Test = new GameObject(eTest.ToString());
                pObjectOrigin_Test.gameObject.SetActive(false);
                pObjectOrigin_Test.AddComponent<TestPoolingObject>().eTestType = eTest;
                listObjectPooling.Add(pObjectOrigin_Test.gameObject);
            }

            CManagerPooling_InResources<ETestPoolingObjectName, TestPoolingObject> pPoolingManager = CManagerPooling_InResources<ETestPoolingObjectName, TestPoolingObject>.instance;
            pPoolingManager.DoInitPoolingObject(listObjectPooling);

            pPoolingManager.p_EVENT_OnMakeResource -= PPoolingManager_p_EVENT_OnMakeResource;
            pPoolingManager.p_EVENT_OnPopResource -= PPoolingManager_p_EVENT_OnPopResource;
            pPoolingManager.p_EVENT_OnPushResource -= PPoolingManager_p_EVENT_OnPushResource;

            return pPoolingManager;
        }
    }
}
