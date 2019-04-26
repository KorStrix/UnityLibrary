#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-01-30 오후 1:20:23
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

public class CManagerPooling_Component<Class_GetType> : CManagerPoolingBase<CManagerPooling_Component<Class_GetType>, Class_GetType>
    where Class_GetType : Component
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */


    /* protected & private - Field declaration         */

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public Class_GetType DoPop(Class_GetType pObjectCopyTarget, Vector3 vecPos, bool bAutoReturn_OnDisable = true)
    {
        Class_GetType pUnUsed = base.DoPop(pObjectCopyTarget);
        CCompoEventTrigger_OnDisable pEventTrigger_AutoReturn = pUnUsed.GetComponent<CCompoEventTrigger_OnDisable>();
        if (bAutoReturn_OnDisable)
        {
            pEventTrigger_AutoReturn.p_Event_OnDisable -= DoPush;
            pEventTrigger_AutoReturn.p_Event_OnDisable += DoPush;
        }
        else
            pEventTrigger_AutoReturn.p_Event_OnDisable -= DoPush;

        pUnUsed.transform.position = vecPos;
        return pUnUsed;
    }


    public Class_GetType DoPop(GameObject pObjectCopyTarget, bool bAutoReturn_OnDisable = true)
    {
        return DoPop(pObjectCopyTarget.GetComponent<Class_GetType>(), Vector3.zero, bAutoReturn_OnDisable);
    }

    public override void DoDestroyAll()
    {
        var arrDestroy = _mapAllInstance.Keys.ToArray();
        foreach (var pObject in arrDestroy)
        {
            if (pObject != null)
                DestroyImmediate(pObject.gameObject);
        }

        base.DoDestroyAll();
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override Class_GetType OnCreateClass_WhenEmptyPool(Class_GetType pObjectCopyTarget, int iID)
    {
        GameObject pObjectUnUsed = Instantiate(pObjectCopyTarget.gameObject);
        pObjectUnUsed.name = string.Format("{0}_{1}", pObjectCopyTarget.name, _mapUnUsed[iID].Count + _mapUsed[iID].Count);
        pObjectUnUsed.transform.SetParent(transform);

        CCompoEventTrigger_OnDisable pEventTrigger = pObjectUnUsed.AddComponent<CCompoEventTrigger_OnDisable>();
        pEventTrigger.p_Event_OnDestroy += Event_RemovePoolObject;

        Class_GetType pComponentUnUsed = pObjectUnUsed.GetComponent<Class_GetType>();
        if (pComponentUnUsed == null)
            Debug.LogError(name + " 풀링 매니져 에러 - pComponentNew == null, Origin Object : " + pObjectCopyTarget.name, pObjectCopyTarget);

        return pComponentUnUsed;
    }

    protected override void OnPopObject(Class_GetType pClassType)
    {
        if(pClassType != null)
            pClassType.gameObject.SetActive(true);
    }

    protected override void OnPushObject(Class_GetType pClassType)
    {
        if (pClassType.gameObject != null && pClassType.gameObject.activeSelf)
            pClassType.gameObject.SetActive(false);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

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

#endif
#endregion Test