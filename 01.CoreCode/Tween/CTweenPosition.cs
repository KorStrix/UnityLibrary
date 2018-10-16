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

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public static class CTweenPositionHelper
{
    static public void DoStartTween_Position(this Transform pTransStart, Transform pTransDestPos, float fDuration, UnityEngine.Events.UnityAction OnFinishTween)
    {
        CTweenPosition pTweenPos = pTransStart.GetComponent<CTweenPosition>();
        if (pTweenPos == null)
            pTweenPos = pTransStart.gameObject.AddComponent<CTweenPosition>();

        pTweenPos.p_vecPosStart = pTransStart.position;
        pTweenPos.p_vecPosDest = pTransDestPos.position;
        pTweenPos.p_fDuration = fDuration;

        pTweenPos.p_Event_OnFinishTween.AddListener(OnFinishTween);
        pTweenPos.DoPlayTween_Forward();
    }

    static public void DoStartTween_Position2D(this Transform pTransStart, Transform pTransDestPos, float fDuration, UnityEngine.Events.UnityAction OnFinishTween)
    {
        CTweenPosition pTweenPos = pTransStart.GetComponent<CTweenPosition>();
        if (pTweenPos == null)
            pTweenPos = pTransStart.gameObject.AddComponent<CTweenPosition>();

        Vector3 vecPos = pTransStart.position;
        vecPos.z = pTransDestPos.position.z;
        pTweenPos.p_vecPosStart = vecPos;
        pTweenPos.p_vecPosDest = pTransDestPos.position;
        pTweenPos.p_fDuration = fDuration;

        pTweenPos.p_Event_OnFinishTween.AddListener(OnFinishTween);
        pTweenPos.DoPlayTween_Forward();
    }
}

public class CTweenPosition : CTweenBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public Vector3 p_vecPosStart;
    public Vector3 p_vecPosDest;

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
        return Vector3.Lerp(p_vecPosStart, p_vecPosDest, fProgress_0_1);
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

    //protected override void OnDrawGizmos()
    //{
    //    base.OnDrawGizmos();

    //    GUIStyle style = new GUIStyle();
    //    style.normal.textColor = Color.cyan;
    //    Gizmos.color = Color.cyan;

    //    Vector3 vecStart = p_vecPosStart;
    //    Vector3 vecDest = p_vecPosDest;

    //    if (p_bIsLocal)
    //    {
    //        vecStart = transform.TransformPoint(p_vecPosStart);
    //        vecDest = transform.TransformPoint(p_vecPosDest);
    //    }

    //    Gizmos.DrawLine(vecStart, vecDest);

    //    UnityEditor.Handles.Label(vecStart, "TweenPos Start : " + name, style);
    //    Gizmos.DrawSphere(vecStart, 1f);

    //    UnityEditor.Handles.Label(vecDest, "TweenPos Dest : " + name, style);
    //    Gizmos.DrawSphere(vecDest, 1f);
    //}

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