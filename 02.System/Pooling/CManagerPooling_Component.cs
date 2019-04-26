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

    public Class_GetType DoPop(Class_GetType pObjectCopyTarget, Vector3 vecPos)
    {
        Class_GetType pUnUsed = base.DoPop(pObjectCopyTarget);
        pUnUsed.transform.position = vecPos;
        return pUnUsed;
    }

    public Class_GetType DoPop(GameObject pObjectCopyTarget)
    {
        return DoPop(pObjectCopyTarget.GetComponent<Class_GetType>(), Vector3.zero);
    }

    public override void DoDestroyAll()
    {
        List<Class_GetType> listDestroyKey = new List<Class_GetType>();
        listDestroyKey.AddRange(_mapAllInstance.Keys);

        foreach (var pObject in listDestroyKey)
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

        OnPopComponent(pClassType);
    }

    protected override void OnPushObject(Class_GetType pClassType)
    {
        if (pClassType.gameObject != null && pClassType.gameObject.activeSelf)
            pClassType.gameObject.SetActive(false);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void OnPopComponent(Class_GetType pUnUsed)
    {
        CCompoEventTrigger_OnDisable pEventTrigger_AutoReturn = pUnUsed.GetComponent<CCompoEventTrigger_OnDisable>();
        if (pEventTrigger_AutoReturn != null)
        {
            pEventTrigger_AutoReturn.p_Event_OnDisable -= DoPush;
            pEventTrigger_AutoReturn.p_Event_OnDisable += DoPush;
        }
    }

    #endregion Private
}