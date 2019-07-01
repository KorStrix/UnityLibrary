using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

#if Firebase
namespace StrixLibrary_Test
{
    public class CManagerFirebase_Test : MonoBehaviour
    {
        [System.Serializable]
        public class TestData : IFirebaseData
        {
            public string IFirebaseData_strDBKey { get; set; }
            public string strUserName;
            public int iGold;

            public TestData(string strUserID, string strUserName, int iGold)
            {
                IFirebaseData_strDBKey = strUserID;
                this.strUserName = strUserName;
                this.iGold = iGold;
            }

        }

        Dictionary<string, TestData> _mapDBResult;

        TestData _pData;
        bool _bIsInit_FirebaseManager;
        bool _bResult;

        // ===================================================================================================

        [UnityTest]
        public IEnumerator Write_And_Read_And_Delete_Test()
        {
            CManagerFirebase_Test pTester = CreateTesterObject(nameof(Write_And_Read_And_Delete_Test));
            yield return pTester.StartCoroutine(WaitForInit(pTester));

            CManagerFirebase pManagerFirebase = CManagerFirebase.instance;

            _pData = null;
            Assert.IsNull(_pData);

            for (int i = 0; i < 3; i++)
            {
                int iRandomNumber = UnityEngine.Random.Range(0, int.MaxValue);
                TestData pTestData = new TestData("Test User ID " + iRandomNumber.ToString(), "Test User Name" + iRandomNumber.ToString(), iRandomNumber);

                // 랜덤으로 데이터를 삽입한다.
                yield return pTester.StartCoroutine(pManagerFirebase.DoSetData_Coroutine<TestData>(pTestData, null));

                // 테이블 내의 모든 데이터를 얻어온다.
                yield return pTester.StartCoroutine(pManagerFirebase.DoGetData_Single_Coroutine<TestData>(pTestData.IFirebaseData_strDBKey, OnFinishGetData_Single));
                Assert.AreEqual(_pData.IFirebaseData_strDBKey, pTestData.IFirebaseData_strDBKey);

                // 테이블 내의 모든 데이터를 얻어온다.
                yield return pTester.StartCoroutine(pManagerFirebase.DoGetData_Multi_Coroutine<TestData>(OnFinishGetData_Multi));

                // 랜덤으로 삽입된 데이터가 얻어온 DB에 들어있는지 확인한다.
                Assert.AreEqual(_mapDBResult[pTestData.IFirebaseData_strDBKey].IFirebaseData_strDBKey, pTestData.IFirebaseData_strDBKey);
                Assert.AreEqual(_mapDBResult.Count, 1);

                // 데이터를 삭제한다.
                yield return pTester.StartCoroutine(pManagerFirebase.DoRemoveData_Coroutine<TestData>(pTestData, null));
            }

            yield break;
        }

        [UnityTest]
        public IEnumerator Proxy_Test()
        {
            CManagerFirebase_Test pTester = CreateTesterObject(nameof(Proxy_Test));
            yield return pTester.StartCoroutine(WaitForInit(pTester));

            CManagerFirebase pManagerFirebase = CManagerFirebase.instance;

            int iRandomNumber = UnityEngine.Random.Range(0, int.MaxValue);
            string strDBKey = "Test User ID " + iRandomNumber.ToString();
            TestData pTestData = new TestData(strDBKey, "Test User Name" + iRandomNumber.ToString(), iRandomNumber);

            // 프록시를 통해 데이터를 삽입한다. DB에 데이터가 있으면 DB에있는 데이터가 들어온다.
            CProxy<TestData> pProxyData = new CProxy<TestData>(strDBKey, pTestData, 0.1f, OnFinish_Proxy);

            // 프록시를 통해 순간적으로 대량의 수정 요청을 한다.
            int iGoldResult = pTestData.iGold;
            for (int i = 0; i < 1000; i++)
            {
                int iGoldChangeAmount = UnityEngine.Random.Range(-100, 100);
                pProxyData.pData_RequireUpdate_Directly.iGold += iGoldChangeAmount;
                iGoldResult += iGoldChangeAmount;
            }


            // DB 갱신까지 잠시 기다린다.
            float fElpaseTime = 0f;
            while (pProxyData.p_bIsWaitServer && fElpaseTime < 5f)
            {
                fElpaseTime += Time.deltaTime;
                yield return null;
            }

            if (fElpaseTime >= 5f)
            {
                Debug.LogError("시간 초과" + fElpaseTime);
                yield break;
            }

            _pData = null;
            Assert.IsNull(_pData);

            // DB로부터 데이터를 얻어온 데이터와 로컬의 프록시와 일치하는지 확인한다.
            yield return pTester.StartCoroutine(pManagerFirebase.DoGetData_Single_Coroutine<TestData>(pTestData.IFirebaseData_strDBKey, OnFinishGetData_Single));
            Assert.AreEqual(pProxyData.pData.iGold, _pData.iGold, iGoldResult);

            // 테스트를 끝낸 뒤 데이터를 삭제한다.
            yield return pTester.StartCoroutine(pManagerFirebase.DoRemoveData_Coroutine<TestData>(pTestData, null));

            yield break;
        }

        private void OnFinish_Proxy()
        {
        }

        private void OnFinishInitailizing(bool bIsFinish)
        {
            _bIsInit_FirebaseManager = bIsFinish;
        }

        private void OnFinishGetData_Single(bool bIsSuccess, string strJsonResult, TestData pDBResult)
        {
            _bResult = bIsSuccess;
            _pData = pDBResult;
        }

        private void OnFinishGetData_Multi(bool bIsSuccess, string strJsonResult, Dictionary<string, TestData> mapDBResult)
        {
            _bResult = bIsSuccess;
            _mapDBResult = mapDBResult;
        }

        // ===================================================================================================

        private IEnumerator WaitForInit(CManagerFirebase_Test pTester)
        {
            _mapDBResult = null;
            _bResult = false;

            Assert.IsNull(_mapDBResult);
            Assert.IsFalse(_bResult);

            _bIsInit_FirebaseManager = false;
            CManagerFirebase pFirebaseManager = CManagerFirebase.instance;
            CManagerFirebase.p_Event_OnFinish_Initailizing.Subscribe_And_Listen_CurrentData += OnFinishInitailizing;

            float fWaitTimeSec = 0f;
            while (_bIsInit_FirebaseManager == false && fWaitTimeSec < 5f)
            {
                Debug.Log("Wait For FireBase Manager Initializing... WaitTimeSec : " + fWaitTimeSec);

                fWaitTimeSec += Time.deltaTime;
                yield return null;
            }

            if (fWaitTimeSec >= 5f)
            {
                Debug.LogError("Error - Init Fail FirebaseManager - TimeOut");
                yield break;
            }

            // 기존에 들어있는 모든 데이터를 다 지운다.
            yield return pTester.StartCoroutine(pFirebaseManager.DoGetData_Multi_Coroutine<TestData>(OnFinishGetData_Multi));
            foreach (var pData in _mapDBResult.Values)
                yield return pTester.StartCoroutine(pFirebaseManager.DoRemoveData_Coroutine<TestData>(pData, null));
        }

        private CManagerFirebase_Test CreateTesterObject(string strGameObjectName)
        {
            GameObject pObjectNew = new GameObject(strGameObjectName);
            return pObjectNew.AddComponent<CManagerFirebase_Test>();
        }
    }
}
#endif