#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-11-11 오후 12:54:37
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class CCompoEventTrigger_OnDisable : MonoBehaviour
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public event System.Action<GameObject> p_Event_OnDisable;
    public event System.Action<GameObject> p_Event_OnDestroy;

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    private void OnDisable()
    {
        if (p_Event_OnDisable != null)
            p_Event_OnDisable(gameObject);
    }

    private void OnDestroy()
    {
        if (p_Event_OnDestroy != null)
            p_Event_OnDestroy(gameObject);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}