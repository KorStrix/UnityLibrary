#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-06-08 오전 11:41:40
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

public class CManagerPoolingSimple : CSingletonDynamicMonoBase<CManagerPoolingSimple>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public bool p_bIsDebug = false;

    /* protected & private - Field declaration         */

    public Dictionary<GameObject, int> _mapAllInstance = new Dictionary<GameObject, int>();
    public Dictionary<int, LinkedList<GameObject>> _mapUsed = new Dictionary<int, LinkedList<GameObject>>();
    public Dictionary<int, LinkedList<GameObject>> _mapUnUsed = new Dictionary<int, LinkedList<GameObject>>();

    public Dictionary<int, int> _mapLayerBackup = new Dictionary<int, int>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoPrePooling(GameObject pObjectCopyTarget, int iCount)
    {
        if (pObjectCopyTarget == null)
            return;

        int iID = pObjectCopyTarget.GetHashCode();
        if (_mapUnUsed.ContainsKey(iID) == false)
        {
            _mapUsed.Add(iID, new LinkedList<GameObject>());
            _mapUnUsed.Add(iID, new LinkedList<GameObject>());
        }

        int iTotalCount = _mapUnUsed[iID].Count + _mapUnUsed[iID].Count;
        if (iTotalCount > iCount)
            return;

        LinkedList<GameObject> listTemp = new LinkedList<GameObject>();
        int iPoolingCount = iCount - iTotalCount;
        for (int i = 0; i < iPoolingCount; i++)
            listTemp.AddLast(DoPop(pObjectCopyTarget));

        foreach(var pPrePoolingObject in listTemp)
            DoPush(pPrePoolingObject);
    }

    public GameObject DoPop(GameObject pObjectCopyTarget, Vector3 vecPos, bool bAutoReturn_OnDisable = true)
    {
        if (pObjectCopyTarget == null)
            return null;

        int iID = pObjectCopyTarget.GetHashCode();
        if (_mapUnUsed.ContainsKey(iID) == false)
        {
            _mapUsed.Add(iID, new LinkedList<GameObject>());
            _mapUnUsed.Add(iID, new LinkedList<GameObject>());
            _mapLayerBackup.Add(iID, pObjectCopyTarget.layer);
        }

        GameObject pObjectUnUsed = null;
        if (_mapUnUsed[iID].Count != 0)
        {
            pObjectUnUsed = _mapUnUsed[iID].First.Value;
            _mapUnUsed[iID].RemoveFirst();
        }
        else
        {
            pObjectUnUsed = Instantiate(pObjectCopyTarget);
            pObjectUnUsed.name = string.Format("{0}_{1}", pObjectCopyTarget.name, _mapUnUsed[iID].Count + _mapUsed[iID].Count);
            pObjectUnUsed.transform.SetParent(transform);

            CCompoEventTrigger pEventTrigger = pObjectUnUsed.AddComponent<CCompoEventTrigger>();
            // pEventTrigger.p_bIsDebuging = true;
            pEventTrigger.p_eConditionType = CCompoEventTrigger.EConditionTypeFlags.OnDisable;
            pEventTrigger.p_Event_IncludeThisObject += DoPush;

            _mapAllInstance.Add(pObjectUnUsed, iID);
        }

        if (p_bIsDebug)
            Debug.Log("Pooling Simple Pop - " + pObjectUnUsed.name, this);

        CCompoEventTrigger pEventTrigger_AutoReturn = pObjectUnUsed.GetComponent<CCompoEventTrigger>();
        if (bAutoReturn_OnDisable)
            pEventTrigger_AutoReturn.p_eConditionType = CCompoEventTrigger.EConditionTypeFlags.OnDisable;
        else
            pEventTrigger_AutoReturn.p_eConditionType = CCompoEventTrigger.EConditionTypeFlags.None;

        pObjectUnUsed.transform.position = vecPos;
        pObjectUnUsed.SetActive(true);
        pObjectUnUsed.layer = _mapLayerBackup[iID];
        _mapUsed[iID].AddLast(pObjectUnUsed);
        return pObjectUnUsed;
    }

    public GameObject DoPop(GameObject pObjectCopyTarget, bool bAutoReturn_OnDisable = true)
    {
        return DoPop(pObjectCopyTarget, Vector3.zero, bAutoReturn_OnDisable);
    }

    public void DoPush(GameObject pObjectReturn)
    {
        if (_mapAllInstance.ContainsKey(pObjectReturn) == false)
            return;

        int iID = _mapAllInstance[pObjectReturn];

        if (_mapUsed.ContainsKey(iID))
            _mapUsed[iID].Remove(pObjectReturn);

        if (_mapUnUsed.ContainsKey(iID))
            _mapUnUsed[iID].AddLast(pObjectReturn);

        if (p_bIsDebug)
            Debug.Log("Pooling Simple Pushed - " + pObjectReturn.name, this);

        if (pObjectReturn.activeSelf)
            pObjectReturn.SetActive(false);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

#if UNITY_EDITOR
    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        int iUseCount = 0;
        foreach(var pList in _mapUsed.Values)
            iUseCount += pList.Count;

        name = string.Format("풀링매니져_심플/ 총생산:{0} /사용중:{1}", _mapAllInstance.Count, iUseCount);
    }
#endif

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test