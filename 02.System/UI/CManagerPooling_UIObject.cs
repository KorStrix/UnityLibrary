#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-04 오후 7:20:24
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IPoolingUIObject
{
}

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Canvas))]
public class CManagerPooling_UIObject<Class_DERIVED> : CSingletonMonoBase<Class_DERIVED>
    where Class_DERIVED : CManagerPooling_UIObject<Class_DERIVED>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    /* protected & private - Field declaration         */

    Dictionary<KeyValuePair<System.Type, string>, GameObject> _mapIngameUIObject_Origin = new Dictionary<KeyValuePair<System.Type, string>, GameObject>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public Class_InGameUIObject DoPop<Class_InGameUIObject>(string strObjectName)
        where Class_InGameUIObject : CObjectBase, IPoolingUIObject
    {
        GameObject pInGameUIObject_Origin = _mapIngameUIObject_Origin[new KeyValuePair<System.Type, string>(typeof(Class_InGameUIObject), strObjectName)];

        Class_InGameUIObject pInGameUIObject = CManagerPooling_Component<Class_InGameUIObject>.instance.DoPop(pInGameUIObject_Origin);
        pInGameUIObject.transform.SetParent(transform);
        pInGameUIObject.transform.DoResetTransform();

        return pInGameUIObject;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        Transform[] arrTransform = GetComponentsInChildren<Transform>(true);
        for(int i = 0; i < arrTransform.Length; i++)
        {
            GameObject pObject = arrTransform[i].gameObject;

            IPoolingUIObject[] arrInGameUIObject = pObject.GetComponents<IPoolingUIObject>();
            for (int j = 0; j < arrInGameUIObject.Length; j++)
            {
                _mapIngameUIObject_Origin.Add(new KeyValuePair<System.Type, string>(arrInGameUIObject[j].GetType(), pObject.name), pObject);
                pObject.SetActive(false);
            }
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}