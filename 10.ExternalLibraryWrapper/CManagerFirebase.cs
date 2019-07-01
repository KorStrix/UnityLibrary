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

    public override void OnUpdate(float fTimeScale_Individual)
    {
        base.OnUpdate(fTimeScale_Individual);

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
#endif