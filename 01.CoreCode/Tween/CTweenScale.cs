#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-06-01 오전 11:56:06
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

public class CTweenScale : CTweenBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public Vector3 p_vecPosStart;
    public Vector3 p_vecPosDest;

    /* protected & private - Field declaration         */

    Transform _pTransformTarget;
    Vector3 _vecScale_Backup;


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void Reset()
    {
        base.Reset();

        p_vecPosStart = transform.localScale;
        p_vecPosDest = transform.localScale;
    }

    protected override void OnSetTarget(GameObject pObjectNewTarget)
    {
        _pTransformTarget = pObjectNewTarget.transform;
    }

    protected override void OnTween(float fProgress_0_1)
    {
        _pTransformTarget.localScale = p_vecPosStart * (1f - fProgress_0_1) + p_vecPosDest * fProgress_0_1;
    }

    public override void OnEditorButtonClick_SetStartValue_IsCurrentValue()
    {
        p_vecPosStart = _pTransformTarget.localScale;
    }

    public override void OnEditorButtonClick_SetDestValue_IsCurrentValue()
    {
        p_vecPosDest = _pTransformTarget.localScale;
    }

    public override void OnEditorButtonClick_SetCurrentValue_IsStartValue()
    {
        _pTransformTarget.localScale = p_vecPosStart;
    }

    public override void OnEditorButtonClick_SetCurrentValue_IsDestValue()
    {
        _pTransformTarget.localScale = p_vecPosDest;
    }

    public override void OnInitTween_EditorOnly()
    {
        _vecScale_Backup = _pTransformTarget.localScale;
    }

    public override void OnReleaseTween_EditorOnly()
    {
        _pTransformTarget.localScale = _vecScale_Backup;
    }

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