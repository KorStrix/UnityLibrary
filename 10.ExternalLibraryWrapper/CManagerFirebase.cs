#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-12 오후 6:55:13
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using System;
using NUnit.Framework;
using UnityEngine.TestTools;

#if Firebase && Newtonsoft
using Newtonsoft.Json;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
#endif

public interface IFirebaseData
{
#if Newtonsoft
    [JsonIgnore]
#endif
    string IFirebaseData_strDBKey { get; set; }
}

public interface IProxy
{
    bool p_bIsWaitServer { get; set; }
    bool p_bIsInit { get; }

    IFirebaseData IProxy_GetData();
    System.Type IProxy_GetDataType();
    string IProxy_GetDataDBKey();

    void IProxy_Check_IsRequireUpdate(float fDeltaTime, ref bool bIsRequireUpdate_Defualt_Is_False);
    void IProxy_SetData(bool bIsSuccess, string strJsonData);
}

/// <summary>
/// Firebase Data를 보관하는 컨테이너. 주기적으로 갱신됩니다.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CProxy<T> : IProxy, IEnumerator
    where T : IFirebaseData
{
    public T pData => _pDataInstance;
    public T pData_RequireUpdate_Directly
    {
        get
        {
            _fElapseTime = _fUpdateCycle;
            return _pDataInstance;
        }
    }

    public bool p_bIsInit { get; private set; }
    public bool p_bIsWaitServer { get; set; }

    public object Current => null;

    T _pDataInstance;
    float _fUpdateCycle;
    float _fElapseTime;

    System.Action _OnFinishLoad;

    // =========================================================================================================================

    public CProxy(string strDBKey, T pData_OnNullDB, float fUpdateCycle, System.Action OnFinishLoad)
    {
        p_bIsInit = false;

        this._pDataInstance = pData_OnNullDB;
        this._pDataInstance.IFirebaseData_strDBKey = strDBKey;

        this._fUpdateCycle = fUpdateCycle;
        this._fElapseTime = fUpdateCycle;

        _OnFinishLoad = OnFinishLoad;

#if Firebase
        CManagerFirebase.instance.DoRegistProxy(this);
#endif
    }

    public void IProxy_Check_IsRequireUpdate(float fDeltaTime, ref bool bIsRequireUpdate_Defualt_Is_False)
    {
        _fElapseTime += fDeltaTime;
        if (_fElapseTime > _fUpdateCycle)
        {
            _fElapseTime = 0f;
            bIsRequireUpdate_Defualt_Is_False = true;
        }
    }

    public IFirebaseData IProxy_GetData()
    {
        return pData;
    }

    public void IProxy_SetData(bool bIsSuccess, string strJson)
    {
        if (p_bIsInit == false)
        {
            string strKeyBackup = _pDataInstance.IFirebaseData_strDBKey;
            JsonUtility.FromJsonOverwrite(strJson, _pDataInstance);
            _pDataInstance.IFirebaseData_strDBKey = strKeyBackup;

            p_bIsInit = true;
        }

        _OnFinishLoad?.Invoke();
        _OnFinishLoad = null;

        p_bIsWaitServer = false;
    }

    public Type IProxy_GetDataType()
    {
        return typeof(T);
    }

    public string IProxy_GetDataDBKey()
    {
        return _pDataInstance.IFirebaseData_strDBKey;
    }

    public bool MoveNext()
    {
        return p_bIsWaitServer;
    }

    public void Reset() { }
}

#if Firebase && Newtonsoft

/// <summary>
/// 
/// </summary>
public class CManagerFirebase : CSingletonNotMonoBase<CManagerFirebase>
{
    static public CObserverSubject<bool> p_Event_OnFinish_Initailizing { get; private set; } = new CObserverSubject<bool>();
    static public bool p_bIsInit { get; private set; } = false;

    public delegate void OnFinish_FirebaseDB_GetValue_Multi<T>(bool bIsSuccess, string strJsonResult, Dictionary<string, T> mapDBResult);
    public delegate void OnFinish_FirebaseDB_GetValue_Single<T>(bool bIsSuccess, string strJsonResult, T pDBResult);
    public delegate void OnFinish_FirebaseDB_GetValue_Single(bool bIsSuccess, string strJsonResult);

    public delegate void OnFinish_FirebaseDB_SetValue_Single(bool bIsSuccess, string strJson);
    public delegate void OnFinish_FirebaseDB_SimpleOrder<T>(bool bIsSuccess, T pSetValue);

    public string p_strDB_URL = "https://randomskilldefence.firebaseio.com/";

    List<IProxy> _listProxy = new List<IProxy>();

    // =========================================================================================================================

    public void DoRegistProxy(IProxy pProxy)
    {
        if (_listProxy.Contains(pProxy) == false)
            _listProxy.Add(pProxy);
    }

    public IEnumerator DoGetData_Single_Coroutine<T>(string strKey, OnFinish_FirebaseDB_GetValue_Single<T> OnFinishGetData)
    where T : IFirebaseData
    {
        var pTask = DoGetData_Single<T>(strKey, OnFinishGetData);
        while (pTask.IsCompleted == false)
        {
            yield return null;
        }
    }

    public IEnumerator DoGetData_Multi_Coroutine<T>(OnFinish_FirebaseDB_GetValue_Multi<T> OnFinishGetData)
        where T : IFirebaseData
    {
        var pTask = DoGetData_Multi(OnFinishGetData);
        while (pTask.IsCompleted == false)
        {
            yield return null;
        }
    }

    public IEnumerator DoSetData_Coroutine<T>(T pObject, OnFinish_FirebaseDB_SimpleOrder<T> OnFinishSetData)
        where T : IFirebaseData
    {
        var pTask = DoSetData(pObject).
            ContinueWith(task =>
            {
                OnFinishSetData?.Invoke(task.IsFaulted == false && task.IsCompleted, pObject);
            });

        while (pTask.IsCompleted == false)
        {
            yield return null;
        }
    }

    public IEnumerator DoRemoveData_Coroutine<T>(T pObject, OnFinish_FirebaseDB_SimpleOrder<T> OnFinishSetData)
        where T : IFirebaseData
    {
        var pTask = DoRemoveData(pObject).
            ContinueWith(task =>
            {
                OnFinishSetData?.Invoke(task.IsFaulted == false && task.IsCompleted, pObject);
            });

        while (pTask.IsCompleted == false)
        {
            yield return null;
        }
    }

    public Task DoGetData_Multi<T>(OnFinish_FirebaseDB_GetValue_Multi<T> OnFinishGetData)
        where T : IFirebaseData
    {
        string strRootNodeName = GetRootNodeName(typeof(T));
        return FirebaseDatabase.DefaultInstance.GetReference(strRootNodeName)
            .GetValueAsync().ContinueWith(task =>
            {
                string strJsonResult = task.Result.GetRawJsonValue();
                if (task.IsFaulted || string.IsNullOrEmpty(strJsonResult))
                {
                    OnFinishGetData?.Invoke(false, strJsonResult, new Dictionary<string, T>());
                }
                else if (task.IsCompleted)
                {
                    Dictionary<string, T> mapDBResult = null;
                    bool bSuccessParsing = true;
                    try
                    {
                        mapDBResult = JsonConvert.DeserializeObject<Dictionary<string, T>>(strJsonResult);
                        foreach (var pData in mapDBResult)
                            pData.Value.IFirebaseData_strDBKey = pData.Key;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error - " + nameof(DoGetData_Multi) + " strRootName : " + strRootNodeName + " Exception : " + e);
                        bSuccessParsing = false;
                    }
                    OnFinishGetData?.Invoke(bSuccessParsing, strJsonResult, mapDBResult);
                }
            });
    }

    public Task DoGetData_Single<T>(string strKey, OnFinish_FirebaseDB_GetValue_Single<T> OnFinishGetData)
        where T : IFirebaseData
    {
        string strRootNodeName = GetRootNodeName(typeof(T));
        return FirebaseDatabase.DefaultInstance.RootReference.Child(strRootNodeName).
            Child(strKey).
            GetValueAsync().
            ContinueWith(task =>
            {
                string strJsonResult = task.Result.GetRawJsonValue();
                if (task.IsFaulted || string.IsNullOrEmpty(strJsonResult))
                {
                    OnFinishGetData?.Invoke(false, strJsonResult, default(T));
                }
                else if (task.IsCompleted)
                {
                    T pData = JsonConvert.DeserializeObject<T>(strJsonResult);
                    pData.IFirebaseData_strDBKey = strKey;
                    OnFinishGetData?.Invoke(true, strJsonResult, pData);
                }
            });
    }


    public Task DoGetData_Single(System.Type pType, string strKey, OnFinish_FirebaseDB_GetValue_Single OnFinishGetData)
    {
        string strRootNodeName = GetRootNodeName(pType);
        return FirebaseDatabase.DefaultInstance.RootReference.Child(strRootNodeName).
            Child(strKey).
            GetValueAsync().
            ContinueWith(task =>
            {
                string strJsonResult = task.Result.GetRawJsonValue();
                if (task.IsFaulted || string.IsNullOrEmpty(strJsonResult))
                {
                    OnFinishGetData?.Invoke(false, strJsonResult);
                }
                else if (task.IsCompleted)
                {
                    OnFinishGetData?.Invoke(true, strJsonResult);
                }
            });
    }

    public Task DoSetData<T>(T pData)
        where T : IFirebaseData
    {
        return DoSetData(typeof(T), pData.IFirebaseData_strDBKey, pData, null);
    }

    public Task DoSetData(System.Type pType, string strKey, object pData)
    {
        return DoSetData(pType, strKey, pData, null);
    }

    public Task DoSetData(System.Type pType, string strKey, object pData, OnFinish_FirebaseDB_SetValue_Single OnFinishSingleData)
    {
        if(string.IsNullOrEmpty(strKey))
        {
            Debug.LogError("Error " + nameof(DoSetData) + "strKey is Null");
            return null;
        }

        string strRootNodeName = GetRootNodeName(pType);
        string strJson = JsonUtility.ToJson(pData);

        return FirebaseDatabase.DefaultInstance.RootReference.Child(strRootNodeName).
                Child(strKey).
                SetRawJsonValueAsync(strJson).
                ContinueWith(task =>
                {
                    if (OnFinishSingleData == null)
                        return;

                    if (task.IsFaulted)
                    {
                        OnFinishSingleData.Invoke(false, "");
                    }
                    else if (task.IsCompleted)
                    {
                        DoGetData_Single(pType, strKey,
                            (bool bIsSuccess, string strJsonResult) =>
                            {
                                OnFinishSingleData.Invoke(bIsSuccess, strJsonResult);
                            });
                    }
                });
    }

    public Task DoRemoveData<T>(T pData)
        where T : IFirebaseData
    {
        string strRootNodeName = GetRootNodeName(typeof(T));
        string strKey = pData.IFirebaseData_strDBKey;
        string strJson = JsonUtility.ToJson(pData);

        return FirebaseDatabase.DefaultInstance.RootReference.Child(strRootNodeName).
            Child(strKey).
            RemoveValueAsync();
    }

    private static string GetRootNodeName(System.Type pType)
    {
        return pType.GetFriendlyName();
    }

    // =========================================================================================================================

    protected override void OnMakeSingleton(out bool bIsGenearteGameObject, out bool bIsUpdateAble)
    {
        base.OnMakeSingleton(out bIsGenearteGameObject, out bIsUpdateAble);

        bIsUpdateAble = true;
        p_bIsInit = false;
        p_Event_OnFinish_Initailizing.DoNotify(false);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp, i.e.
                //   app = Firebase.FirebaseApp.DefaultInstance;
                // where app is a Firebase.FirebaseApp property of your application class.

                // Set a flag here indicating that Firebase is ready to use by your
                // application.

                // _pFirebaseDataBase = FirebaseDatabase.DefaultInstance;
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }

            p_bIsInit = true;
            p_Event_OnFinish_Initailizing.DoNotify(true);
        });

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(p_strDB_URL);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        float fDeltaTime = Time.deltaTime;
        for (int i = 0; i < _listProxy.Count; i++)
        {
            bool bIsRequireUpdate = false;
            IProxy pCurrentProxy = _listProxy[i];
            if (pCurrentProxy.p_bIsWaitServer)
                continue;

            pCurrentProxy.IProxy_Check_IsRequireUpdate(fDeltaTime, ref bIsRequireUpdate);
            if (p_bIsInit && bIsRequireUpdate)
            {
                pCurrentProxy.p_bIsWaitServer = true;

                if(pCurrentProxy.p_bIsInit)
                {
                    DoSetData(pCurrentProxy.IProxy_GetDataType(),
                        pCurrentProxy.IProxy_GetDataDBKey(),
                        pCurrentProxy.IProxy_GetData(),
                        pCurrentProxy.IProxy_SetData);
                }
                else
                {
                    DoGetData_Single(pCurrentProxy.IProxy_GetDataType(),
                        pCurrentProxy.IProxy_GetDataDBKey(),
                        pCurrentProxy.IProxy_SetData);
                }
            }
        }
    }
}

// ===================================================================================================

public class CFirebaseTester : MonoBehaviour
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
        CFirebaseTester pTester = CreateTesterObject(nameof(Write_And_Read_And_Delete_Test));
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
        CFirebaseTester pTester = CreateTesterObject(nameof(Proxy_Test));
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

    private IEnumerator WaitForInit(CFirebaseTester pTester)
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

    private CFirebaseTester CreateTesterObject(string strGameObjectName)
    {
        GameObject pObjectNew = new GameObject(strGameObjectName);
        return pObjectNew.AddComponent<CFirebaseTester>();
    }
}
#endif
