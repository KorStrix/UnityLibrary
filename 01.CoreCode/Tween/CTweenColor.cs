#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-28 오후 4:44:42
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if TMPro
using TMPro;
#endif

#if Spine
using Spine.Unity;
#endif

public class CTweenColor : CTweenBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public Color pColor_Start = Color.white;
    public Color pColor_Dest = Color.black;

    /* protected & private - Field declaration         */

    SpriteRenderer _pSpriteRenderer = null;
    Renderer _pRenderer = null;
    Graphic _pUIElement = null;
    Light _pLight = null;

#if TMPro
    TextMeshPro _pTMPro = null;
#endif

#if Spine
    SkeletonAnimation _pSpineAnimation = null;

    CCompo_SpineColorControl _pSpineColor;

#endif

    Color _pColor_Backup;
    Material _pMaterial_Backup;

    public Color p_pColor
    {
        get
        {
#if Spine
            if(_pSpineAnimation)
            {
                if (_pSpineColor == null)
                {
                    _pSpineColor = _pSpineAnimation.GetComponent<CCompo_SpineColorControl>();
                    if(_pSpineColor == null)
                        _pSpineColor = _pSpineAnimation.gameObject.AddComponent<CCompo_SpineColorControl>();
                }

                if (_pSpineColor)
                    return _pSpineColor.GetColor();
            }
#endif

#if TMPro
            if (_pTMPro)
                return _pTMPro.color;
#endif

            if (_pSpriteRenderer)
                return _pSpriteRenderer.color;

            if (_pRenderer)
                return _pRenderer.material.color;

            if (_pUIElement)
                return _pUIElement.color;

            if (_pLight)
                return _pLight.color;


            return Color.white;
        }

        private set
        {
#if Spine
            if (_pSpineAnimation)
            {
                if (_pSpineColor == null)
                {
                    _pSpineColor = _pSpineAnimation.GetComponent<CCompo_SpineColorControl>();
                    if (_pSpineColor == null)
                        _pSpineColor = _pSpineAnimation.gameObject.AddComponent<CCompo_SpineColorControl>();
                }

                if (_pSpineColor)
                {
                    _pSpineColor.DoSetColor(value);
                    return;
                }
            }
#endif

#if TMPro
            if (_pTMPro)
            {
                _pTMPro.color = value;
                return;
            }
#endif

            if (_pSpriteRenderer)
            {
                _pSpriteRenderer.color = value;
                return;
            }

            if (_pRenderer)
            {
                _pRenderer.material.color = value;
                return;
            }

            if (_pUIElement)
            {
                _pUIElement.color = value;
                return;
            }

            if (_pLight)
            {
                _pLight.color = value;
                return;
            }

        }
    }

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    public override void OnEditorButtonClick_SetStartValue_IsCurrentValue()
    {
        EventOnAwake();
        pColor_Start = p_pColor;
    }

    public override void OnEditorButtonClick_SetDestValue_IsCurrentValue()
    {
        EventOnAwake();
        pColor_Dest = p_pColor;
    }

    public override void OnEditorButtonClick_SetCurrentValue_IsStartValue()
    {
        EventOnAwake();
        p_pColor = pColor_Start;
    }

    public override void OnEditorButtonClick_SetCurrentValue_IsDestValue()
    {
        EventOnAwake();
        p_pColor = pColor_Dest;
    }

    protected override void OnSetTarget(GameObject pObjectNewTarget)
    {
        _pSpriteRenderer = pObjectNewTarget.GetComponent<SpriteRenderer>();
        _pRenderer = pObjectNewTarget.GetComponent<Renderer>();
        _pUIElement = pObjectNewTarget.GetComponent<Graphic>();
        _pLight = pObjectNewTarget.GetComponent<Light>();

#if Spine
        _pSpineAnimation = pObjectNewTarget.GetComponent<SkeletonAnimation>();
#endif

#if TMPro
        _pTMPro = pObjectNewTarget.GetComponent<TextMeshPro>();
#endif
    }

    protected override void OnTween(float fProgress_0_1)
    {
        p_pColor = Color.Lerp(pColor_Start, pColor_Dest, fProgress_0_1);
    }

    public override void OnInitTween_EditorOnly()
    {
        OnAwake();

        _pColor_Backup = p_pColor;
        if (_pRenderer)
        {
            _pMaterial_Backup = _pRenderer.sharedMaterial;
            _pRenderer.material = new Material(_pMaterial_Backup);
            _pRenderer.sharedMaterials = new Material[] { _pMaterial_Backup };
            return;
        }
    }

    public override void OnReleaseTween_EditorOnly()
    {
        if(_pRenderer)
            _pRenderer.material = _pMaterial_Backup;

        p_pColor = _pColor_Backup;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

#region Private

#endregion Private
}