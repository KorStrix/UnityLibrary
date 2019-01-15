#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-11-14 오후 7:10:18
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class CManagerTimeScale : CSingletonDynamicMonoBase<CManagerTimeScale>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public CObserverSubject<float> p_Event_OnChangeTimeScale { get { return _Event_OnChangeTimeScale; } }
    public float p_fCurrentTimeScale { get; private set; }

    /* protected & private - Field declaration         */

    private CObserverSubject<float> _Event_OnChangeTimeScale = new CObserverSubject<float>();

    Stack<float> _stackTimeScale = new Stack<float>();
    Coroutine _pCoroutine_Fade;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoSet_PrevTimeScale()
    {
        if (_stackTimeScale.Count == 0)
            return;

        SetTimeScale(_stackTimeScale.Pop(), false);
    }

    public void DoAddTimeScale(float fAddTimeScale)
    {
        SetTimeScale(p_fCurrentTimeScale + fAddTimeScale);
    }

    public void DoSetTimeScale(float fTimeScale)
    {
        SetTimeScale(fTimeScale);
    }

    public void DoSetTimeScale_Fade(float fTimeScale, float fFadeDuration)
    {
        if (_pCoroutine_Fade != null)
            StopCoroutine(_pCoroutine_Fade);

        _pCoroutine_Fade = StartCoroutine(CoFadeSetTimeScale(fTimeScale, fFadeDuration));
    }

    public void DoSetTimeScale()
    {
        SetTimeScale(p_fCurrentTimeScale);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        DoSetTimeScale(1f);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void SetTimeScale(float fTimeScale, bool bEnqueueStack = true)
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log(name + " SetTimeScale : " + fTimeScale, this);

        Time.timeScale = fTimeScale;
        p_fCurrentTimeScale = fTimeScale;
        _Event_OnChangeTimeScale.DoNotify(fTimeScale);

        if(bEnqueueStack)
            _stackTimeScale.Push(fTimeScale);
    }

    IEnumerator CoFadeSetTimeScale(float fTimeScaleDset, float fDuration)
    {
        float fStartTimeScale = p_fCurrentTimeScale;
        float fElapseTime = 0f;
        while(fElapseTime < fDuration)
        {
            float fTimeScale = Mathf.Lerp(fStartTimeScale, fTimeScaleDset, fElapseTime / fDuration);
            SetTimeScale(fTimeScale);

            fElapseTime += Time.unscaledDeltaTime;
            yield return null;
        }

        SetTimeScale(fTimeScaleDset);
    }

    #endregion Private
}