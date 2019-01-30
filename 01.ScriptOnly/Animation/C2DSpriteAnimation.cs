#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-10-06 오후 8:03:25
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class C2DSpriteAnimation : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [Rename_Inspector("FPS")]
    public float p_fFPS = 30.0f;
    [Rename_Inspector("반복 재생 할 것인지")]
    public bool p_bIsLoop = true;
    [Rename_Inspector("Enable 시 자동 재생할 것인지")]
    public bool p_bIsPlay_OnEnable = true;

    [Rename_Inspector("애니메이션 이미지 목록")]
    public Sprite[] p_arrFrames;

    /* protected & private - Field declaration         */

    [GetComponent]
    SpriteRenderer _pSpriteRenderer = null;

    int _iFrameIndex;
    System.Action _OnFinishAnimation;
    bool _bIsPlayAnimation;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Button("Play Animation")]
#endif
    public void DoPlayAnimation_ForEditor()
    {
        DoPlayAnimation(null);
        StartCoroutine(OnEnableObjectCoroutine());
    }

    public void DoPlayAnimation()
    {
        DoPlayAnimation(null);
    }

    public void DoPlayAnimation(System.Action OnFinishAnimation)
    {
        EventOnAwake();

        _OnFinishAnimation = OnFinishAnimation;
        _bIsPlayAnimation = true;
    }
    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        if (p_bIsPlay_OnEnable)
            _bIsPlayAnimation = true;

        while(true)
        {
            if(_bIsPlayAnimation)
            {
                while (true)
                {
                    bool bIsFinishAnimation = CalculateNextAnimation();

                    yield return YieldManager.GetWaitForSecond(1 / p_fFPS);

                    if (bIsFinishAnimation && p_bIsLoop == false)
                        break;
                }

                OnFinishAnimation();
            }
            else
            {
                yield return null;
            }
        }

    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

#region Private

    bool CalculateNextAnimation()
    {
        _pSpriteRenderer.sprite = p_arrFrames[_iFrameIndex];

        bool bIsFinishAnimation = _iFrameIndex + 1 == p_arrFrames.Length;
        _iFrameIndex = (_iFrameIndex + 1) % p_arrFrames.Length;

        return bIsFinishAnimation;

    }

    void OnFinishAnimation()
    {
        if (_OnFinishAnimation != null)
        {
            System.Action OnFinishAnimation = _OnFinishAnimation;
            _OnFinishAnimation = null;
            OnFinishAnimation();
        }

        _bIsPlayAnimation = false;
    }

#endregion Private
}