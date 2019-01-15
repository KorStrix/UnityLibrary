#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-12-26 오후 4:41:00
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

[RequireComponent(typeof(Text))]
public class CUGUITextAnimation : MonoBehaviour
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public Text p_pTextComponent { get; private set; }

    public string[] p_arrTextAnimation = new string[2];

    [Rename_Inspector("Duration")]
    public float p_fDuration = 1f;
    [Rename_Inspector("Loop")]
    public bool p_bIsLoop = true;
    [Rename_Inspector("Ignore TimeScale")]
    public bool p_bIsIgnore_TimeScale = false;

    /* protected & private - Field declaration         */

    Coroutine _pCoroutine_Animation;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoPlayTextAnimation()
    {
        DoStopTextAnimation();

        if (p_bIsIgnore_TimeScale)
            _pCoroutine_Animation = StartCoroutine(CoTextAnimation_Ignore_TimeScale());
        else
            _pCoroutine_Animation = StartCoroutine(CoTextAnimation());
    }

    public void DoStopTextAnimation()
    {
        if(_pCoroutine_Animation != null)
            StopCoroutine(_pCoroutine_Animation);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    private void Awake()
    {
        p_pTextComponent = GetComponent<Text>();
    }

    private void OnEnable()
    {
        DoPlayTextAnimation();
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    IEnumerator CoTextAnimation()
    {
        while(p_bIsLoop)
        {
            int iIndex = 0;
            while (iIndex < p_arrTextAnimation.Length)
            {
                p_pTextComponent.text = p_arrTextAnimation[iIndex];

                iIndex++;
                yield return new WaitForSeconds(p_fDuration / p_arrTextAnimation.Length);
            }
        }
    }

    IEnumerator CoTextAnimation_Ignore_TimeScale()
    {
        while (p_bIsLoop)
        {
            int iIndex = 0;
            while (iIndex < p_arrTextAnimation.Length)
            {
                p_pTextComponent.text = p_arrTextAnimation[iIndex];

                iIndex++;
                yield return new WaitForSecondsRealtime(p_fDuration / p_arrTextAnimation.Length);
            }
        }
    }

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test