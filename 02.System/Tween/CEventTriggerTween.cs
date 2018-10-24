#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-06-01 오후 1:09:05
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

public class CEventTriggerTween : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EInputType_Tween
    {
        TweenStart,
        TweenFinish
    }

    /* public - Field declaration            */

    [Rename_Inspector("트리거 작동 조건")]
    public EInputType_Tween p_eInputType_Main = EInputType_Tween.TweenStart;
    [Rename_Inspector("트리거 작동 시 처음 딜레이")]
    public float p_fDelayTrigger = 0f;
    public UnityEngine.Events.UnityEvent p_listEvent_Main = new UnityEngine.Events.UnityEvent();

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */


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