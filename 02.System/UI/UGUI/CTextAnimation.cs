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
using System.Text;

[RequireComponent((typeof(CTextWrapper)))]
public class CTextAnimation : CObjectBase, IPoolingUIObject
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public ObservableCollection<CTextAnimation> p_Event_OnFinishAnimation { get; private set; } = new ObservableCollection<CTextAnimation>();

    [GetComponent]
    public CTextWrapper p_pTextComponent { get; private set; }

    public string p_strText => p_pTextComponent.text;

    [DisplayName("지속시간")]
    public float p_fDuration = 1f;
    [DisplayName("반복 유무")]
    public bool p_bIsLoop = true;
    [DisplayName("Ignore TimeScale")]
    public bool p_bIsIgnore_TimeScale = false;

    /* protected & private - Field declaration         */

    StringBuilder _pStrBuilder = new StringBuilder();
    Coroutine _pCoroutine_Animation;
    string strAnimationText;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoPlayTextAnimation()
    {
        DoPlayTextAnimation(p_strText);
    }

    public void DoPlayTextAnimation(string strText)
    {
        DoStopTextAnimation();
        if (string.IsNullOrEmpty(strText))
            return;
        strAnimationText = strText;

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

    private void OnEnable()
    {
        DoPlayTextAnimation();
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    IEnumerator CoTextAnimation()
    {
        do
        {
            _pStrBuilder.Length = 0;

            int iIndex = 0;
            while (iIndex < strAnimationText.Length)
            {
                _pStrBuilder.Append(strAnimationText[iIndex]);
                p_pTextComponent.text = _pStrBuilder.ToString();

                iIndex++;
                yield return YieldManager.GetWaitForSecond(p_fDuration / strAnimationText.Length);
            }

            p_Event_OnFinishAnimation.DoNotify(this);
            yield return null;

        } while (p_bIsLoop);
    }

    IEnumerator CoTextAnimation_Ignore_TimeScale()
    {
        do
        {
            _pStrBuilder.Length = 0;

            int iIndex = 0;
            while (iIndex < strAnimationText.Length)
            {
                _pStrBuilder.Append(strAnimationText[iIndex]);
                p_pTextComponent.text = _pStrBuilder.ToString();

                iIndex++;
                yield return new WaitForSecondsRealtime(p_fDuration / strAnimationText.Length);

            }

            p_Event_OnFinishAnimation.DoNotify(this);
            yield return null;

        } while (p_bIsLoop) ;
    }

    #endregion Private
}