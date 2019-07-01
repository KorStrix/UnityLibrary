#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-27 오후 9:05:30
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CTweenPosition : CTweenBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [DisplayName("트윈 시작 위치")]
    public Vector3 p_vecPosStart;
    [DisplayName("트윈 도착 위치")]
    public Vector3 p_vecPosDest;

    [DisplayName("Local Position으로 할지")]
    public bool p_bIsLocal;

    /* protected & private - Field declaration         */

    Transform _pTransformTarget;
    Vector3 _vecPos_Backup;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void Reset()
    {
        base.Reset();

        p_vecPosStart = transform.position;
        p_vecPosDest = transform.position;
    }

    protected override void OnSetTarget(GameObject pObjectNewTarget)
    {
        _pTransformTarget = pObjectNewTarget.transform;
    }

    protected override void OnTween(float fProgress_0_1)
    {
        if (p_bIsLocal)
            _pTransformTarget.localPosition = p_vecPosStart * (1f - fProgress_0_1) + p_vecPosDest * fProgress_0_1;
        else
            _pTransformTarget.position = p_vecPosStart * (1f - fProgress_0_1) + p_vecPosDest * fProgress_0_1;
    }

    public override void OnEditorButtonClick_SetStartValue_IsCurrentValue()
    {
        if (p_bIsLocal)
            p_vecPosStart = _pTransformTarget.localPosition;
        else
            p_vecPosStart = _pTransformTarget.position;
    }

    public override void OnEditorButtonClick_SetDestValue_IsCurrentValue()
    {
        if (p_bIsLocal)
            p_vecPosDest = _pTransformTarget.localPosition;
        else
            p_vecPosDest = _pTransformTarget.position;
    }

    public override void OnEditorButtonClick_SetCurrentValue_IsStartValue()
    {
        if (p_bIsLocal)
            _pTransformTarget.localPosition = p_vecPosStart;
        else
            _pTransformTarget.position = p_vecPosStart;
    }

    public override void OnEditorButtonClick_SetCurrentValue_IsDestValue()
    {
        if (p_bIsLocal)
            _pTransformTarget.localPosition = p_vecPosDest;
        else
            _pTransformTarget.position = p_vecPosDest;
    }

    public override void OnInitTween_EditorOnly()
    {
        _vecPos_Backup = _pTransformTarget.position;
    }

    public override void OnReleaseTween_EditorOnly()
    {
        _pTransformTarget.position = _vecPos_Backup;
    }

    public override object OnTween_EditorOnly(float fProgress_0_1)
    {
        return p_vecPosStart * (1f - fProgress_0_1) + p_vecPosDest * fProgress_0_1;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if(g_bIsDrawGizmo)
        {
            Gizmos.color = Color.cyan;

            Transform pTransformPraents = p_pObjectTarget.transform.parent;
            if (p_bIsLocal && pTransformPraents)
                Gizmos.DrawLine(pTransformPraents.TransformPoint(p_vecPosStart), pTransformPraents.TransformPoint(p_vecPosDest));
            else
                Gizmos.DrawLine(p_vecPosStart, p_vecPosDest);
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}