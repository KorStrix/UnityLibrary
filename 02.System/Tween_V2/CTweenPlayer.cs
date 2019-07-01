#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-02 오후 7:19:16
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static CTweenPlayer;

[System.Serializable]
abstract public class CTweenData
{
    public delegate void OnCreateYield(out CustomYieldInstruction pYield);
    public delegate void OnTweenEvent(ETweenDirection eTweenDirection, CTweenBase pTweener);

    public event OnTweenEvent p_Event_OnStartTween_InCludeArg;
    public event OnTweenEvent p_Event_OnFinishTween_InCludeArg;


    public bool p_bIsPlayingTween { get { return _bIsPlayingTween; } }
    public ETweenDirection p_eTweenDirection { get; private set; }
    public float p_fProgress_0_1 { get { return _fProgress_0_1; } }

    [DisplayName("트윈 대상")]
    public GameObject p_pObjectTarget = null;

    [Header("트윈 옵션")]
    [DisplayName("트윈 스타일")]
    public ETweenStyle p_eTweenStyle = ETweenStyle.Once;
    [DisplayName("재생시간")]
    public float p_fDuration = 1f;
    [DisplayName("애니메이션 커브")]
    public AnimationCurve p_pAnimationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
    [DisplayName("기본 트윈 재생 시 방향")]
    public ETweenDirection p_eTweenDirection_OnDefaultPlay = ETweenDirection.Forward;

    [Header("플레이 옵션")]
    [DisplayName("Enable시 자동 재생")]
    public bool p_bIsPlay_OnEnable = false;
    [DisplayName("Enable시 초기값으로")]
    public bool p_bIsReset_OnEnable = false;
    [DisplayName("트윈 전의 딜레이")]
    public float p_fFirstDelaySec = 0f;
    [DisplayName("트윈 후의 딜레이")]
    public float p_fAfterDelaySec = 0f;

    [Header("기타 옵션")]
    [DisplayName("Ignore TimeScale?")]
    public bool p_bIgnoreTimeScale = false;
    [DisplayName("물리 업데이트를 사용할 것인지")]
    public bool p_bUseFixedUpdate = false;

    [SerializeField]
    [DisplayName("현재 진행 상황 0 ~ 1", false)]
    protected float _fProgress_0_1;
    [SerializeField]
    [DisplayName("트윈 플레이 중인지?", false)]
    bool _bIsPlayingTween;

    Coroutine _pCoroutineTween;
    GameObject gameObject;

    bool _bIsFinishForward;
    bool _bIsFinishRevese;
    int _iTweenDirectionDelta = 1;

    // ========================================================================== //

    public Coroutine StartCoroutine(IEnumerator pCoroutine)
    {
        return null;
    }

    public void StopCoroutine(Coroutine pCoroutine)
    {

    }

    public void DoSetTarget(GameObject pObjectTarget)
    {
        if (pObjectTarget == null)
            pObjectTarget = gameObject;

        p_pObjectTarget = pObjectTarget;
        OnSetTarget(pObjectTarget);
    }

    public void DoStopTween()
    {
        _bIsPlayingTween = false;
        if (_pCoroutineTween != null)
            StopCoroutine(_pCoroutineTween);
    }

    public void DoInitTween(ETweenDirection eTweenDirection, bool bReset_Progress)
    {
        p_eTweenDirection = eTweenDirection;
        _iTweenDirectionDelta = eTweenDirection == ETweenDirection.Forward ? 1 : -1;

        _bIsFinishForward = false;
        _bIsFinishRevese = false;

        if (bReset_Progress)
            _fProgress_0_1 = eTweenDirection == ETweenDirection.Forward ? 0f : 1f;
    }

    public void DoPlayTween(bool bResetProgress = true)
    {
        StartTween(bResetProgress, p_eTweenDirection_OnDefaultPlay);
    }


    public void DoSetTweening(float fDeltaTime)
    {
        if (p_eTweenDirection == ETweenDirection.Forward && _fProgress_0_1 >= 1f)
            _bIsFinishForward = true;
        else if (p_eTweenDirection == ETweenDirection.Reverse && _fProgress_0_1 <= 0f)
            _bIsFinishRevese = true;

        if (p_fDuration == 0f)
        {
            Debug.LogWarning(" p_fDuration == 0f");
            return;
        }

        if (_bIsFinishForward)
        {
            switch (p_eTweenStyle)
            {
                case ETweenStyle.Loop:
                    {
                        _fProgress_0_1 = 0f;
                        _bIsFinishForward = false;
                    }
                    break;

                case ETweenStyle.PingPong:
                    {
                        p_eTweenDirection = ETweenDirection.Reverse;
                        _iTweenDirectionDelta *= -1;
                        _bIsFinishForward = false;
                    }
                    break;
            }
        }
        else if (_bIsFinishRevese)
        {
            switch (p_eTweenStyle)
            {
                case ETweenStyle.Loop:
                    {
                        _fProgress_0_1 = 1f;
                        _bIsFinishRevese = false;
                    }
                    break;

                case ETweenStyle.PingPong:
                    {
                        p_eTweenDirection = ETweenDirection.Forward;
                        _iTweenDirectionDelta *= -1;
                        _bIsFinishRevese = false;
                    }
                    break;
            }
        }

        if (_bIsFinishForward == false && _bIsFinishRevese == false)
        {
            _fProgress_0_1 += CalculateProgressDelta(fDeltaTime * _iTweenDirectionDelta);
            if (_fProgress_0_1 > 1f)
                _fProgress_0_1 = 1f;

            OnTween(p_pAnimationCurve.Evaluate(p_fProgress_0_1));
        }
    }

    // ========================================================================== //

    abstract protected void OnSetTarget(GameObject pObjectNewTarget);
    abstract protected void OnTween(float fProgress_0_1);
    virtual protected void OnTweenStart(ETweenDirection eTweenDirection) { }

    virtual protected float CalculateProgressDelta(float fDeltaTime)
    {
        return Mathf.Abs(1f / p_fDuration) * fDeltaTime;
    }

    // ========================================================================== //

    void StartTween(bool bReset_Progress, ETweenDirection eTweenDirection)
    {
        DoSetTarget(p_pObjectTarget);

        if (gameObject.activeInHierarchy == false)
            return;

        if (_bIsPlayingTween && eTweenDirection != p_eTweenDirection)
            DoStopTween();
        OnTweenStart(eTweenDirection);

        _bIsPlayingTween = true;
        if (p_bUseFixedUpdate)
        {
            DoInitTween(eTweenDirection, bReset_Progress);

            //if (p_Event_OnStartTween_InCludeArg != null)
            //    p_Event_OnStartTween_InCludeArg(eTweenDirection, this);
        }
        else
        {
            if (p_bIgnoreTimeScale)
                _pCoroutineTween = StartCoroutine(CoStartTween(bReset_Progress, eTweenDirection, OnCreate_YieldForSecond_Real, OnGetTimeDelta_Real));
            else
                _pCoroutineTween = StartCoroutine(CoStartTween(bReset_Progress, eTweenDirection, OnCreate_YieldForSecond, OnGetTimeDelta));
        }
    }

    private IEnumerator CoStartTween(bool bReset_Progress, ETweenDirection eTweenDirection, OnCreateYield OnCreatorYield, System.Func<float> OnGetDeltaTime)
    {
        if (p_fFirstDelaySec != 0f)
        {
            if (p_bIgnoreTimeScale)
                yield return new WaitForSecondsRealtime(p_fFirstDelaySec);
            else
                yield return YieldManager.GetWaitForSecond(p_fFirstDelaySec);
        }

        DoInitTween(eTweenDirection, bReset_Progress);

        //if (p_Event_OnStartTween_InCludeArg != null)
        //    p_Event_OnStartTween_InCludeArg(eTweenDirection, this);

        while (_bIsFinishForward == _bIsFinishRevese)
        {
            DoSetTweening(OnGetDeltaTime());

            CustomYieldInstruction pYield;
            OnCreatorYield(out pYield);

            yield return pYield;
        }

        if (p_fAfterDelaySec != 0f)
        {
            if (p_bIgnoreTimeScale)
                yield return new WaitForSecondsRealtime(p_fAfterDelaySec);
            else
                yield return YieldManager.GetWaitForSecond(p_fAfterDelaySec);
        }

        //if (p_Event_OnFinishTween != null)
        //    p_Event_OnFinishTween.Invoke();

        //if (p_Event_OnFinishTween_InCludeArg != null)
        //    p_Event_OnFinishTween_InCludeArg(eTweenDirection, this);

        _bIsPlayingTween = false;
    }


    // 이렇게 하면 안된다..
    //public WaitForSeconds_Custom OnCreate_YieldForSecond()
    //{
    //    return YieldManager.GetWaitForSecond_Custom(Time.deltaTime);
    //}

    //public WaitForSecondsRealtime OnCreate_YieldForSecond_Real()
    //{
    //    return YieldManager.GetWaitForSecondRealtime(Time.unscaledDeltaTime);
    //}

    public void OnCreate_YieldForSecond(out CustomYieldInstruction pReturn)
    {
        pReturn = new WaitForSeconds_Custom(Time.deltaTime);
    }

    public void OnCreate_YieldForSecond_Real(out CustomYieldInstruction pReturn)
    {
        pReturn = new WaitForSecondsRealtime(Time.unscaledDeltaTime);
    }

    public float OnGetTimeDelta()
    {
        return Time.deltaTime;
    }

    public float OnGetTimeDelta_Real()
    {
        return Time.unscaledDeltaTime;
    }

}

/// <summary>
/// 
/// </summary>
public class CTweenPlayer : MonoBehaviour
{
    /* const & readonly declaration             */

    static public bool g_bIsDrawGizmo = false;

    /* enum & struct declaration                */

    public enum ETweenDirection
    {
        Forward,
        Reverse
    }

    public enum ETweenStyle
    {
        Once,
        Loop,
        PingPong,
    }

    /* public - Field declaration            */

    public List<CTweenData> p_listTweenData = new List<CTweenData>();

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoPlayTween(bool bResetProgress = true)
    {
    }

    public void DoPlayTween_Forward(bool bResetProgress = true)
    {
    }

    public void DoPlayTween_Reverse(bool bResetProgress = true)
    {
    }


    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}