#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-08-01 오후 5:59:09
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

#if Spine
using Spine.Unity;
#endif

public class CTweenLightIntensity : CTweenBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public float p_fIntensity_Start = 1f;
    public float p_fIntensity_Dest = 0f;

    /* protected & private - Field declaration         */

    float _fIntensityBackup;
    Light _pLight = null;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    public override void OnEditorButtonClick_SetStartValue_IsCurrentValue()
    {
        EventOnAwake();
        p_fIntensity_Start = _pLight.intensity;
    }

    public override void OnEditorButtonClick_SetDestValue_IsCurrentValue()
    {
        EventOnAwake();
        p_fIntensity_Dest = _pLight.intensity;
    }

    public override void OnEditorButtonClick_SetCurrentValue_IsStartValue()
    {
        EventOnAwake();
        _pLight.intensity = p_fIntensity_Start;
    }

    public override void OnEditorButtonClick_SetCurrentValue_IsDestValue()
    {
        EventOnAwake();
        _pLight.intensity = p_fIntensity_Dest;
    }

    protected override void OnSetTarget(GameObject pObjectNewTarget)
    {
        _pLight = pObjectNewTarget.GetComponent<Light>();
    }

    protected override void OnTween(float fProgress_0_1)
    {
        _pLight.intensity = Mathf.Lerp(p_fIntensity_Start, p_fIntensity_Dest, fProgress_0_1);
    }

    public override void OnInitTween_EditorOnly()
    {
        OnAwake();
        _fIntensityBackup = _pLight.intensity;
    }

    public override void OnReleaseTween_EditorOnly()
    {
        _pLight.intensity = _fIntensityBackup;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}