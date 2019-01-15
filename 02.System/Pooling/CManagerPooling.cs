#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-12-21 오전 10:13:20
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public class CManagerPooling<Class_GetType> : CSingletonNotMonoBase<CManagerPooling<Class_GetType>>
#if UNITY_EDITOR
    ,IUpdateAble // 하이어라키 실시간 풀링 상황 모니터링을 위한 UpdateAble
#endif
    where Class_GetType : UnityEngine.Component
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public bool p_bIsDebug = false;

    /* protected & private - Field declaration         */

    public Dictionary<Class_GetType, int> _mapAllInstance = new Dictionary<Class_GetType, int>();
    //public Dictionary<Class_GetType, GameObject> _mapRootObject = new Dictionary<Class_GetType, GameObject>();

    // 본래 LinkedList를 사용했으나, C#에선 LinkedList가 오히려 더 느리다..
    // https://stackoverflow.com/questions/5983059/why-is-a-linkedlist-generally-slower-than-a-list

    public Dictionary<int, HashSet<Class_GetType>> _mapUsed = new Dictionary<int, HashSet<Class_GetType>>();
    public Dictionary<int, List<Class_GetType>> _mapUnUsed = new Dictionary<int, List<Class_GetType>>();

    public Dictionary<int, int> _mapLayerBackup = new Dictionary<int, int>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoPrePooling(GameObject pObjectCopyTarget, int iCount)
    {
        if (pObjectCopyTarget == null)
            return;

        int iID = pObjectCopyTarget.GetHashCode();
        Add_NewObjectType(pObjectCopyTarget, iID);

        int iTotalCount = _mapUnUsed[iID].Count + _mapUnUsed[iID].Count;
        if (iTotalCount > iCount)
            return;

        LinkedList<Class_GetType> listTemp = new LinkedList<Class_GetType>();
        int iPoolingCount = iCount - iTotalCount;
        for (int i = 0; i < iPoolingCount; i++)
            listTemp.AddLast(DoPop(pObjectCopyTarget));

        foreach (var pPrePoolingObject in listTemp)
            DoPush(pPrePoolingObject);
    }

    public Class_GetType DoPop(GameObject pObjectCopyTarget, Vector3 vecPos, bool bAutoReturn_OnDisable = true)
    {
        if (pObjectCopyTarget == null)
            return null;

        int iID = pObjectCopyTarget.GetHashCode();
        Add_NewObjectType(pObjectCopyTarget, iID);

        Class_GetType pComponentUnUsed = Get_UnusedObject(pObjectCopyTarget, iID);
        if (p_bIsDebug)
            Debug.Log(name + " Pooling Simple Pop - " + pComponentUnUsed.name);

        CCompoEventTrigger_OnDisable pEventTrigger_AutoReturn = pComponentUnUsed.GetComponent<CCompoEventTrigger_OnDisable>();
        if(bAutoReturn_OnDisable)
        {
            pEventTrigger_AutoReturn.p_Event_OnDisable -= DoPush;
            pEventTrigger_AutoReturn.p_Event_OnDisable += DoPush;
        }
        else
            pEventTrigger_AutoReturn.p_Event_OnDisable -= DoPush;


        pComponentUnUsed.transform.position = vecPos;
        pComponentUnUsed.SetActive(true);
        return pComponentUnUsed;
    }

    public Class_GetType DoPop(GameObject pObjectCopyTarget, bool bAutoReturn_OnDisable = true)
    {
        return DoPop(pObjectCopyTarget, Vector3.zero, bAutoReturn_OnDisable);
    }

    public void DoPush(GameObject pObjectReturn)
    {
        DoPush(pObjectReturn.GetComponent<Class_GetType>());
    }

    public void DoPush(Class_GetType pClassType)
    {
        if (_mapAllInstance.ContainsKey(pClassType) == false)
            return;

        Remove_UsedList(pClassType);

        if (p_bIsDebug)
            Debug.Log("Pooling Simple Pushed - " + pClassType.name, this);

        if (pClassType.gameObject.activeSelf)
            pClassType.gameObject.SetActive(false);
    }

    public void DoPushAll()
    {
        foreach (var pObject in _mapAllInstance.Keys)
            DoPush(pObject);
    }

    public void DoDestroyAll()
    {
        var arrDestroy = _mapAllInstance.Keys.ToArray();
        foreach (var pObject in arrDestroy)
        {
            if(pObject != null)
                DestroyImmediate(pObject.gameObject);
        }

        _mapAllInstance.Clear();
        _mapUsed.Clear();
        _mapUnUsed.Clear();
        _mapLayerBackup.Clear();
    }

    public void Event_RemovePoolObject(GameObject pObjectDestroyed)
    {
        Class_GetType pObjectReturn = pObjectDestroyed.GetComponent<Class_GetType>();
        int iID = _mapAllInstance[pObjectReturn];

        if (_mapUsed.ContainsKey(iID) && _mapUsed[iID].Contains(pObjectReturn))
            _mapUsed[iID].Remove(pObjectReturn);

        _mapAllInstance.Remove(pObjectReturn);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnMakeSingleton(out bool bIsGenearteGameObject)
    {
        bIsGenearteGameObject = true;
#if UNITY_EDITOR
        strTypeName = typeof(Class_GetType).Name;

        CManagerUpdateObject.instance.DoAddObject(this, true);
#endif
    }

    protected override void OnMakeGameObject(GameObject pObject)
    {
        base.OnMakeGameObject(pObject);

        DontDestroyOnLoad(pObject);
    }

    protected override void OnSceneUnloaded(UnityEngine.SceneManagement.Scene pScene)
    {
        base.OnSceneUnloaded(pScene);

        DoDestroyAll();
    }

    string strTypeName;

#if UNITY_EDITOR // 하이어라키뷰에 실시간 풀링 상황 모니터링을 위한 Update

    protected override void OnReleaseSingleton()
    {
        base.OnReleaseSingleton();

        CManagerUpdateObject.instance.DoRemoveObject(this);
    }

    public void OnUpdate()
    {
        int iUseCount = 0;
        foreach (var pList in _mapUsed.Values)
            iUseCount += pList.Count;

        gameObject.name = string.Format("풀링<{0}>/ 총생산:{1} /사용중:{2}", strTypeName, _mapAllInstance.Count, iUseCount);
    }

    public bool IUpdateAble_IsRequireUpdate()
    {
        return gameObject.activeSelf;
    }

#endif

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void Add_NewObjectType(GameObject pObjectCopyTarget, int iID)
    {
        if (_mapUnUsed.ContainsKey(iID) == false)
        {
            _mapUsed.Add(iID, new HashSet<Class_GetType>());
            _mapUnUsed.Add(iID, new List<Class_GetType>());
            _mapLayerBackup.Add(iID, pObjectCopyTarget.layer);
        }
    }

    private Class_GetType Get_UnusedObject(GameObject pObjectCopyTarget, int iID)
    {
        Class_GetType pComponentUnUsed;
        if (_mapUnUsed[iID].Count != 0)
        {
            int iIndexLast = _mapUnUsed[iID].Count - 1;
            pComponentUnUsed = _mapUnUsed[iID][iIndexLast];
            _mapUnUsed[iID].RemoveAt(iIndexLast);
        }
        else
        {
            GameObject pObjectUnUsed = Instantiate(pObjectCopyTarget.gameObject);
            pObjectUnUsed.name = string.Format("{0}_{1}", pObjectCopyTarget.name, _mapUnUsed[iID].Count + _mapUsed[iID].Count);
            pObjectUnUsed.transform.SetParent(transform);

            CCompoEventTrigger_OnDisable pEventTrigger = pObjectUnUsed.AddComponent<CCompoEventTrigger_OnDisable>();
            pEventTrigger.p_Event_OnDestroy += Event_RemovePoolObject;

            pComponentUnUsed = pObjectUnUsed.GetComponent<Class_GetType>();
            if(pComponentUnUsed == null)
                Debug.LogError(name + " 풀링 매니져 에러 - pComponentNew == null, Origin Object : " + pObjectCopyTarget.name, pObjectCopyTarget);

            _mapAllInstance.Add(pComponentUnUsed, iID);
        }

        _mapUsed[iID].Add(pComponentUnUsed);
        return pComponentUnUsed;
    }

    private void Remove_UsedList(Class_GetType pObjectReturn)
    {
        int iID = _mapAllInstance[pObjectReturn];
        if (_mapUsed.ContainsKey(iID) && _mapUsed[iID].Contains(pObjectReturn))
        {
            _mapUsed[iID].Remove(pObjectReturn);

            if (_mapUnUsed.ContainsKey(iID))
                _mapUnUsed[iID].Add(pObjectReturn);

            if (pObjectReturn.gameObject.activeInHierarchy &&
                pObjectReturn.transform.parent != transform)
                pObjectReturn.transform.SetParent(transform);
        }
    }

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

public class CManagerPooling_Test
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
    public IEnumerator ManagerPoolingGeneric_IsWorking()
    {
        CManagerPooling<TestPoolingObject> pPoolingManager = CManagerPooling<TestPoolingObject>.instance;
        Dictionary <ETestPoolingObjectName, GameObject> mapObjectInstance = InitTest();

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

    private Dictionary<ETestPoolingObjectName, GameObject> InitTest()
    {
        TestPoolingObject.ResetActiveCount();

        Dictionary<ETestPoolingObjectName, GameObject> mapObjectPooling = new Dictionary<ETestPoolingObjectName, GameObject>();
        for (int i = 0; i < (int)ETestPoolingObjectName.Max; i++)
        {
            ETestPoolingObjectName eTest = (ETestPoolingObjectName)i;
            GameObject pObjectOrigin_Test = new GameObject(eTest.ToString());
            pObjectOrigin_Test.gameObject.SetActive(false);
            pObjectOrigin_Test.AddComponent<TestPoolingObject>().eTestType = eTest;
            mapObjectPooling.Add(eTest, pObjectOrigin_Test.gameObject);
        }

        return mapObjectPooling;
    }
}

#endif
#endregion Test