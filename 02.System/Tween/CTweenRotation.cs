#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-28 오후 2:56:28
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CTweenRotationHelper
{
    static public void DoStartTween_Rotation(this Transform pTransStart, Transform pTransDestRot, float fDuration, UnityEngine.Events.UnityAction OnFinishTween)
    {
        CTweenRotation pTweenRot = pTransStart.GetComponent<CTweenRotation>();
        if (pTweenRot == null)
            pTweenRot = pTransStart.gameObject.AddComponent<CTweenRotation>();

        pTweenRot.p_vecRotStart = pTransStart.rotation.eulerAngles;
        pTweenRot.p_vecRotDest = pTransDestRot.rotation.eulerAngles;
        pTweenRot.p_fDuration = fDuration;

        pTweenRot.p_Event_OnFinishTween.AddListener(OnFinishTween);
        pTweenRot.DoPlayTween_Forward();
    }

    static public void DoStartTween_Rotation(this Transform pTransStart, Vector3 vecAngleEulerDest_Offset, float fDuration, UnityEngine.Events.UnityAction OnFinishTween)
    {
        CTweenRotation pTweenRot = pTransStart.GetComponent<CTweenRotation>();
        if (pTweenRot == null)
            pTweenRot = pTransStart.gameObject.AddComponent<CTweenRotation>();

        pTweenRot.p_vecRotStart = pTransStart.rotation.eulerAngles;
        pTweenRot.p_vecRotDest = pTweenRot.p_vecRotStart + vecAngleEulerDest_Offset;
        pTweenRot.p_fDuration = fDuration;

        pTweenRot.p_Event_OnFinishTween.AddListener(OnFinishTween);
        pTweenRot.DoPlayTween_Forward();
    }
}

public class CTweenRotation : CTweenBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [DisplayName("벡터 보간을 사용할 것인지 ? (false면 쿼터니언 보간)")]
    public bool p_bUseSlerp = true;

    [DisplayNameAttribute("시작")]
    public Vector3 p_vecRotStart;
    [DisplayNameAttribute("도달")]
    public Vector3 p_vecRotDest;

    public bool p_bIsLocal;

    /* protected & private - Field declaration         */

    Transform _pTransformTarget;
    // Vector3 _vecOriginPos;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void Reset()
    {
        base.Reset();

        p_vecRotStart = transform.rotation.eulerAngles;
        p_vecRotDest = transform.rotation.eulerAngles;
    }

    protected override void OnSetTarget(GameObject pObjectNewTarget)
    {
        _pTransformTarget = pObjectNewTarget.transform;
        // _vecOriginPos = _pTransformTarget.position;
    }

    protected override void OnTween(float fProgress_0_1)
    {
        if(p_bUseSlerp)
        {
            if (p_bIsLocal)
                _pTransformTarget.localRotation = Quaternion.Euler(Vector3.Slerp(p_vecRotStart, p_vecRotDest, fProgress_0_1));
            else
                _pTransformTarget.rotation = Quaternion.Euler(Vector3.Slerp(p_vecRotStart, p_vecRotDest, fProgress_0_1));
        }
        else
        {
            if (p_bIsLocal)
                _pTransformTarget.localRotation = Quaternion.Lerp(Quaternion.Euler(p_vecRotStart), Quaternion.Euler(p_vecRotDest), fProgress_0_1);
            else
                _pTransformTarget.rotation = Quaternion.Lerp(Quaternion.Euler(p_vecRotStart), Quaternion.Euler(p_vecRotDest), fProgress_0_1);
        }
    }

    public override void OnEditorButtonClick_SetStartValue_IsCurrentValue()
    {
        if (p_bIsLocal)
            p_vecRotStart = _pTransformTarget.localRotation.eulerAngles;
        else
            p_vecRotStart = _pTransformTarget.rotation.eulerAngles;
    }

    public override void OnEditorButtonClick_SetDestValue_IsCurrentValue()
    {
        if (p_bIsLocal)
            p_vecRotDest = _pTransformTarget.localRotation.eulerAngles;
        else
            p_vecRotDest = _pTransformTarget.rotation.eulerAngles;
    }

    public override void OnEditorButtonClick_SetCurrentValue_IsStartValue()
    {
        if (p_bIsLocal)
            _pTransformTarget.localRotation = Quaternion.Euler(p_vecRotStart);
        else
            _pTransformTarget.rotation = Quaternion.Euler(p_vecRotStart);
    }

    public override void OnEditorButtonClick_SetCurrentValue_IsDestValue()
    {
        if (p_bIsLocal)
            _pTransformTarget.localRotation = Quaternion.Euler(p_vecRotDest);
        else
            _pTransformTarget.rotation = Quaternion.Euler(p_vecRotDest);
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (g_bIsDrawGizmo)
        {
            Vector3 vecPos = p_pObjectTarget.transform.position;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(vecPos, vecPos + (Quaternion.Euler(p_vecRotStart) * p_pObjectTarget.transform.forward) * 10f);

            Gizmos.DrawLine(vecPos, vecPos + (Quaternion.Euler(p_vecRotDest) * p_pObjectTarget.transform.forward) * 10f);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.cyan;

            UnityEditor.Handles.Label(vecPos + (Quaternion.Euler(p_vecRotStart) * p_pObjectTarget.transform.forward) * 10f, "Tween Rotation Start", style);
            UnityEditor.Handles.Label(vecPos + (Quaternion.Euler(p_vecRotDest) * p_pObjectTarget.transform.forward) * 10f, "Tween Rotation Dest", style);
        }
    }
#endif

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}