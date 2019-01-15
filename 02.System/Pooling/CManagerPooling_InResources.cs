/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-25 오전 12:03:32
   Description : 
   Edit Log    : 
   ============================================ */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class CManagerPooling_InResources<ENUM_Resource_Name, Class_Resource> : CSingletonNotMonoBase<CManagerPooling_InResources<ENUM_Resource_Name, Class_Resource>>
#if UNITY_EDITOR
    , IUpdateAble // 하이어라키 실시간 풀링 상황 모니터링을 위한 UpdateAble
#endif
    where ENUM_Resource_Name : System.IComparable, System.IConvertible
    where Class_Resource : Component
{
    /* public - Field declaration            */

    public event System.Action<ENUM_Resource_Name, Class_Resource> p_EVENT_OnMakeResource;
    public event System.Action<ENUM_Resource_Name, Class_Resource> p_EVENT_OnPopResource;
    public event System.Action<ENUM_Resource_Name, Class_Resource> p_EVENT_OnPushResource;

    /* protected - Field declaration            */

    protected Dictionary<ENUM_Resource_Name, Class_Resource> _mapResourceOrigin = new Dictionary<ENUM_Resource_Name, Class_Resource>();
    protected Dictionary<ENUM_Resource_Name, Class_Resource> _mapResourceOriginCopy = new Dictionary<ENUM_Resource_Name, Class_Resource>();

    /* private - Field declaration           */

    private bool _bIsInit = false;

    private Dictionary<int, Class_Resource> _mapPoolingInstance = new Dictionary<int, Class_Resource>();
    private Dictionary<int, ENUM_Resource_Name> _mapPoolingResourceType = new Dictionary<int, ENUM_Resource_Name>();

    private Dictionary<ENUM_Resource_Name, Queue<Class_Resource>> _queuePoolingDisable = new Dictionary<ENUM_Resource_Name, Queue<Class_Resource>>();
    private Dictionary<ENUM_Resource_Name, int> _mapResourcePoolingCount = new Dictionary<ENUM_Resource_Name, int>();

    public int p_iPopCount { get; private set; }
    public string p_strResourcesPath;

    private string p_strManagerName { get { return string.Format("리소스 풀링<{0},{1}>/{2}개 활성", typeof(ENUM_Resource_Name).Name, typeof(Class_Resource).Name, p_iPopCount); } }

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    public void DoInitPoolingObject(List<GameObject> listPoolingObject)
    {
        ProcInitManagerPooling(listPoolingObject);
    }

    public void DoInitPoolingObject(string strResourcesPath)
    {
        p_strResourcesPath = strResourcesPath;
        GameObject[] arrResources = Resources.LoadAll<GameObject>(string.Format("{0}/", strResourcesPath));
        ProcInitManagerPooling(arrResources.ToList());
    }

    public void DoInitPoolingObject(string strResourcesPath, List<ENUM_Resource_Name> listPoolingObject)
    {
        List<GameObject> listObject = new List<GameObject>();
        GameObject[] arrResources = Resources.LoadAll<GameObject>(string.Format("{0}/", strResourcesPath));
        for (int i = 0; i < arrResources.Length; i++)
        {
            ENUM_Resource_Name eResourceName;
            if (arrResources[i].name.ConvertEnum(out eResourceName))
            {
                if (listPoolingObject.Contains(eResourceName))
                    listObject.Add(arrResources[i]);
            }
        }

        ProcInitManagerPooling(listObject);
    }

    /// <summary>
    /// 현재 사용하지 않는 오브젝트를 얻습니다. 주의) 사용후 반드시 Return 바람
    /// </summary>
    /// <param name="eResourceName">Enum형태의 리소스 이름</param>
    /// <returns></returns>
    public Class_Resource DoPop(ENUM_Resource_Name eResourceName, bool bGameObjectActive = true)
    {
        Class_Resource pFindResource = null;

        if (_queuePoolingDisable.ContainsKey(eResourceName) == false)
        {
            _mapResourcePoolingCount.Add(eResourceName, 0);
            _queuePoolingDisable.Add(eResourceName, new Queue<Class_Resource>());
        }

        int iLoopCount = 0;
        while (pFindResource == null && iLoopCount++ < _queuePoolingDisable[eResourceName].Count)
        {
            pFindResource = _queuePoolingDisable[eResourceName].Dequeue();
        }

        if (pFindResource == null)
        {
            pFindResource = MakeResource(eResourceName);
            if (pFindResource == null)
            {
                Debug.Log(eResourceName + "가 Pop에 실패했습니다..", transform);
                return null;
            }
        }

        if (pFindResource.transform.parent != transform)
            pFindResource.transform.SetParent(transform);

        pFindResource.gameObject.SetActive(bGameObjectActive);
        p_iPopCount++;

        if (p_EVENT_OnPopResource != null)
            p_EVENT_OnPopResource(eResourceName, pFindResource);

        //if (typeof( ENUM_Resource_Name ) == typeof( string ))
        //	Debug.Log( "Pop " + eResourceName + " Count : " + _queuePoolingDisable[eResourceName].Count, pFindResource );

        return pFindResource;
    }

    /// <summary>
    /// 사용했던 오브젝트를 반환합니다. 자동으로 GameObject가 Disable 됩니다.
    /// </summary>
    /// <param name="pResource">사용한 리소스</param>
    public void DoPush(Class_Resource pResource)
    {
        ProcReturnResource(pResource, false);
    }

    /// <summary>
    /// 사용했던 오브젝트를 반환합니다. 자동으로 GameObject가 Disable 됩니다.
    /// </summary>
    /// <param name="pResource">사용한 리소스</param>
    public void DoPush(Class_Resource pResource, bool bSetPaents_ManagerObject)
    {
        ProcReturnResource(pResource, bSetPaents_ManagerObject);
    }

    /// <summary>
    /// Enum형태의 리소스 이름의 List에 있는 오브젝트만 풀링을 위해 오브젝트를 새로 생성합니다. 
    /// </summary>
    /// <param name="listRequestPooling">풀링할 Enum 형태의 리소스 리스트</param>
    public void DoStartPooling(int iPoolingCount)
    {
        ProcPooling_From_ResourcesFolder();

        List<ENUM_Resource_Name> listKey = _mapResourceOriginCopy.Keys.ToList();
        for (int i = 0; i < listKey.Count; i++)
        {
            ENUM_Resource_Name eResourceName = listKey[i];
            for (int j = 0; j < iPoolingCount; j++)
            {
                if (_queuePoolingDisable[eResourceName].Count > iPoolingCount)
                    continue;

                Class_Resource pResource = MakeResource(eResourceName);
                p_iPopCount++;
                ProcReturnResource(pResource, true);
            }
        }
    }

    /// <summary>
    /// Enum형태의 리소스 이름의 List에 있는 오브젝트만 풀링을 위해 오브젝트를 새로 생성합니다. 
    /// </summary>
    /// <param name="listRequestPooling">풀링할 Enum 형태의 리소스 리스트</param>
    public void DoStartPooling(List<ENUM_Resource_Name> listRequestPooling, int iPoolingCount)
    {
        for (int i = 0; i < listRequestPooling.Count; i++)
        {
            ENUM_Resource_Name eResourceName = listRequestPooling[i];
            if (_queuePoolingDisable.ContainsKey(eResourceName) == false)
            {
                _queuePoolingDisable.Add(eResourceName, new Queue<Class_Resource>());
                _mapResourcePoolingCount.Add(eResourceName, 0);
            }

            for (int j = 0; j < iPoolingCount; j++)
            {
                Class_Resource pResource = MakeResource(eResourceName);
                p_iPopCount++;
                ProcReturnResource(pResource, true);
            }
        }
    }

    /// <summary>
    /// 사용한 모든 오브젝트를 강제로 리턴시킵니다.
    /// </summary>
    public void DoPushAll()
    {
        IEnumerator<KeyValuePair<int, Class_Resource>> pIter = _mapPoolingInstance.GetEnumerator();
        while (pIter.MoveNext())
        {
            ProcReturnResource(pIter.Current.Value, true);
        }
    }

    public Class_Resource GetOriginObject(Class_Resource pResource)
    {
        int hInstanceID = pResource.GetInstanceID();
        if (_mapPoolingInstance.ContainsKey(hInstanceID) == false ||
            _mapPoolingResourceType.ContainsKey(hInstanceID) == false)
        {
            //Debug.LogWarning(pResource.name + " Return fail!!");
            return null;
        }

        ENUM_Resource_Name eResourceName = _mapPoolingResourceType[hInstanceID];
        return _mapResourceOrigin[eResourceName];
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */



    protected override void OnMakeSingleton(out bool bIsGenearteGameObject)
    {
        bIsGenearteGameObject = true;

#if UNITY_EDITOR // 하이어라키뷰에 실시간 풀링 상황 모니터링을 위한 Update
        CManagerUpdateObject.instance.DoAddObject(this, true);
#endif
    }

    protected override void OnMakeGameObject(GameObject pObject)
    {
        base.OnMakeGameObject(pObject);

        DontDestroyOnLoad(pObject);
    }

    protected override void OnSceneUnloaded(Scene pScene)
    {
        base.OnSceneUnloaded(pScene);

        DoPushAll();
    }

#if UNITY_EDITOR // 하이어라키뷰에 실시간 풀링 상황 모니터링을 위한 Update

    protected override void OnReleaseSingleton()
    {
        base.OnReleaseSingleton();

        CManagerUpdateObject.instance.DoRemoveObject(this);
    }

    public void OnUpdate()
    {
        gameObject.name = p_strManagerName;
    }

    public bool IUpdateAble_IsRequireUpdate()
    {
        return gameObject.activeSelf;
    }

#endif

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private void ProcPooling_From_ResourcesFolder()
    {
        if (_bIsInit)
            return;
        _bIsInit = true;

        if (string.IsNullOrEmpty(p_strResourcesPath))
        {
            Debug.LogError("Error- Require Set ResourcesPath");
            return;
        }

        if (Application.isPlaying == false)
            return;

        GameObject[] arrResources = Resources.LoadAll<GameObject>(string.Format("{0}/", p_strResourcesPath));
        ProcInitManagerPooling(arrResources.ToList());
    }

    private void ProcInitManagerPooling(List<GameObject> listObject)
    {
        if (listObject.Count == 0)
        {
            Debug.Log("Init Fail. listObject.Count == 0");
            return;
        }

        System.Type pType_Enum = typeof(ENUM_Resource_Name);
        System.Type pType_Class = typeof(Class_Resource);
        string strEnumName = pType_Enum.Name;
        string strClassName = pType_Class.Name;

        _mapResourceOrigin.Clear();
        _mapResourceOriginCopy.Clear();
        for (int i = 0; i < listObject.Count; i++)
        {
            ENUM_Resource_Name eResourceName = default(ENUM_Resource_Name);
            try
            {
                eResourceName = (ENUM_Resource_Name)System.Enum.Parse(pType_Enum, listObject[i].name);
            }
            catch
            {
                try
                {
                    eResourceName = (ENUM_Resource_Name)((object)listObject[i].name);
                }
                catch
                {
                    Debug.Log("Error Pooling : " + listObject[i].name);
                }
                if (pType_Enum.IsEnum)
                    Debug.Log(string.Format("{0} is not in Enum {1}", listObject[i].name, strEnumName));
            }

            if (_mapResourceOrigin.ContainsKey(eResourceName))
                continue;

            GameObject pObjectOrigin = listObject[i].gameObject;
            Class_Resource pResourceOrigin = pObjectOrigin.GetComponent<Class_Resource>();
            if (pResourceOrigin == null)
                continue;
            _mapResourceOrigin.Add(eResourceName, pResourceOrigin);

            GameObject pObjectCopy = GameObject.Instantiate(pObjectOrigin);
            pObjectCopy.SetActive(false);
            Class_Resource pResource = pObjectCopy.GetComponent<Class_Resource>();
            if (pResource == null)
                pResource = pObjectCopy.AddComponent<Class_Resource>();

            if (_queuePoolingDisable.ContainsKey(eResourceName) == false)
            {
                _queuePoolingDisable.Add(eResourceName, new Queue<Class_Resource>());
                _mapResourcePoolingCount.Add(eResourceName, 0);
            }
            _mapResourceOriginCopy.Add(eResourceName, pResource);

            pResource.name = string.Format("{0}(Origin)", eResourceName);
            Transform pTransMake = pResource.transform;
            pTransMake.SetParent(transform);
            //pTransMake.DoResetTransform();
        }
    }

    private void ProcSetChild(ENUM_Resource_Name eResourceName, Class_Resource pObjectMake)
    {
        Transform pTransMake = pObjectMake.transform;
        pTransMake.SetParent(transform);
        //pTransMake.DoResetTransform();
        pObjectMake.name = string.Format("{0}_{1}", eResourceName, ++_mapResourcePoolingCount[eResourceName]);

        int hInstanceID = pObjectMake.GetInstanceID();
        _mapPoolingInstance.Add(hInstanceID, pObjectMake);
        _mapPoolingResourceType.Add(hInstanceID, eResourceName);
    }

    private Class_Resource MakeResource(ENUM_Resource_Name eResourceName)
    {
        if (_mapResourceOriginCopy.ContainsKey(eResourceName) == false)
            ProcPooling_From_ResourcesFolder();

        if (_mapResourceOriginCopy.ContainsKey(eResourceName) == false)
        {
            Debug.LogError("ManagerPool " + eResourceName + "Not Found Resources : " + eResourceName);
            return null;
        }

        Class_Resource pObjectMake = null;
        pObjectMake = Object.Instantiate(_mapResourceOriginCopy[eResourceName]);
        ProcSetChild(eResourceName, pObjectMake);

        if (p_EVENT_OnMakeResource != null)
            p_EVENT_OnMakeResource(eResourceName, pObjectMake);

        return pObjectMake;
    }

    private void ProcReturnResource(Class_Resource pResource, bool bSetPaents_ManagerObject)
    {
        if (pResource == null)
            return;

        if (pResource.gameObject.activeSelf)
            pResource.gameObject.SetActive(false);

        int hInstanceID = pResource.GetInstanceID();
        if (_mapPoolingInstance.ContainsKey(hInstanceID) == false ||
            _mapPoolingResourceType.ContainsKey(hInstanceID) == false)
            return;

        ENUM_Resource_Name eResourceName = _mapPoolingResourceType[hInstanceID];
        if (_queuePoolingDisable.ContainsKey(eResourceName) == false ||
            _queuePoolingDisable[eResourceName].Contains(pResource))
            return;

        if (bSetPaents_ManagerObject)
            pResource.transform.SetParent(transform);

        _queuePoolingDisable[eResourceName].Enqueue(pResource);

        if (p_EVENT_OnPushResource != null)
            p_EVENT_OnPushResource(eResourceName, pResource);

        p_iPopCount--;
    }

}

#region Test

public class 풀링_테스트
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

    [UnityTest] [Category("StrixLibrary")]
    public IEnumerator 풀링_기본테스트()
    {
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

    [UnityTest] [Category("StrixLibrary")]
    public IEnumerator 풀링_이벤트테스트()
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
        for(int i = 0; i < 3; i++)
        {
            ETestPoolingObjectName eTest = (ETestPoolingObjectName)i;
            GameObject pObjectOrigin_Test = new GameObject(eTest.ToString());
            pObjectOrigin_Test.gameObject.SetActive(false);
            pObjectOrigin_Test.AddComponent<TestPoolingObject>().eTestType = eTest;
            listObjectPooling.Add(pObjectOrigin_Test.gameObject);
        }

        CManagerPooling_InResources<ETestPoolingObjectName, TestPoolingObject> pPoolingManager = CManagerPooling_InResources<ETestPoolingObjectName, TestPoolingObject>.instance;
        pPoolingManager.DoInitPoolingObject(listObjectPooling);
        return pPoolingManager;
    }
}

#endregion