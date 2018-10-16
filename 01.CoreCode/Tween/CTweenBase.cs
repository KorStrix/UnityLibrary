#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-27 오후 8:29:44
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public class WaitForSeconds_Custom : CustomYieldInstruction
{
    private float _fWaitSeconds;

    public WaitForSeconds_Custom(float fWaitSeconds)
    {
        _fWaitSeconds = Time.time + fWaitSeconds;
    }

    public override bool keepWaiting
    {
        get
        {
            return Time.time < _fWaitSeconds;
        }
    }
}

public class WaitForTween : CustomYieldInstruction
{
    CTweenBase _pTween;

    public WaitForTween(CTweenBase pTween)
    {
        _pTween = pTween;
    }


    public override bool keepWaiting
    {
        get
        {
            return _pTween.p_bIsPlayingTween;
        }
    }
}


abstract public class CTweenBase : CObjectBase
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
        PingPong_ReverseCurve,
    }

    [System.Serializable]
    public class STweenEventBase : IDictionaryItem<string>
    {
        public string strEvent;

        public string IDictionaryItem_GetKey()
        {
            return strEvent;
        }
    }

    /* public - Field declaration            */

    public delegate void OnCreateYield(out CustomYieldInstruction pYield);
    public delegate void OnTweenEvent(ETweenDirection eTweenDirection, GameObject pTweenObject);

    public event OnTweenEvent p_Event_OnStartTween_InCludeArg;
    public event OnTweenEvent p_Event_OnFinishTween_InCludeArg;

    [Rename_Inspector("디버깅 온")]
    public bool bIsDebug = false;

    [Rename_Inspector("트윈 대상")]
    public GameObject p_pObjectTarget = null;

    [Rename_Inspector("애니메이션 커브")]
    public AnimationCurve p_pAnimationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
    [Rename_Inspector("Ignore TimeScale?")]
    public bool p_bIgnoreTimeScale = false;
    [Rename_Inspector("재생시간")]
    public float p_fDuration = 1f;
    [Rename_Inspector("트윈 스타일")]
    public ETweenStyle p_eTweenStyle = ETweenStyle.Once;

    [Rename_Inspector("Enable 시 자동 재생할건지")]
    public bool p_bIsPlay_OnEnable = false;
    [Rename_Inspector("기본 트윈 재생 시 방향")]
    public ETweenDirection p_eTweenDirection_OnDefaultPlay = ETweenDirection.Forward;
    [Rename_Inspector("트윈 전의 딜레이")]
    public float p_fFirstDelaySec = 0f;

    [Rename_Inspector("물리 업데이트를 사용할 것인지")]
    public bool p_bUseFixedUpdate = false;

    public UnityEvent p_Event_OnFinishTween = new UnityEvent();

    public bool p_bIsPlayingTween { get { return _bIsPlayingTween; } }
    public ETweenDirection p_eTweenDirection { get; private set; }
    public float p_fProgress_0_1 { get { return _fProgress_0_1; } }

    /* protected & private - Field declaration         */

    [SerializeField]
    [Rename_Inspector("현재 진행 상황 0 ~ 1", false)]
    float _fProgress_0_1;
    [SerializeField]
    [Rename_Inspector("트윈 플레이 중인지?", false)]
    bool _bIsPlayingTween;

    AnimationCurve _pAnimationCurve_OnReverseCurvePlay;
    int _iTweenDirectionDelta;
    Coroutine _pCoroutineTween;

    bool _bIsFinishForward;
    bool _bIsFinishRevese;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoClearEventList()
    {
        p_Event_OnStartTween_InCludeArg = null;
        p_Event_OnFinishTween_InCludeArg = null;
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
        if (_pCoroutineTween != null)
            StopCoroutine(_pCoroutineTween);
    }

    public void DoPlayTween(bool bResetProgress = true)
    {
        StartTween(bResetProgress, p_eTweenDirection_OnDefaultPlay);
    }

    public void DoPlayTween_Forward(bool bResetProgress = true)
    {
        StartTween(bResetProgress, ETweenDirection.Forward);
    }

    public void DoPlayTween_Reverse(bool bResetProgress = true)
    {
        StartTween(bResetProgress, ETweenDirection.Reverse);
    }

    public void DoInitTween(ETweenDirection eTweenDirection, bool bReset_Progress)
    {
        p_eTweenDirection = eTweenDirection;
        _iTweenDirectionDelta = eTweenDirection == ETweenDirection.Forward ? 1 : -1;

        _bIsFinishForward = false;
        _bIsFinishRevese = false;

        if (bReset_Progress)
            _fProgress_0_1 = eTweenDirection == ETweenDirection.Forward ? 0f : 1f;

        if(p_eTweenStyle == ETweenStyle.PingPong_ReverseCurve)
        {
            Keyframe[] arrKeyframe = p_pAnimationCurve.keys;
            Keyframe[] arrKeyframeReverse = p_pAnimationCurve.keys;
            for (int i = 0; i < arrKeyframe.Length; i++)
            {
                Keyframe pKeyframe = arrKeyframe[i];
                arrKeyframeReverse[i].time = 1 - pKeyframe.time;
                arrKeyframeReverse[i].value = 1 - pKeyframe.value;
            }

            _pAnimationCurve_OnReverseCurvePlay = new AnimationCurve(arrKeyframeReverse);
        }
    }

    public void DoResetProgress()
    {
        _fProgress_0_1 = 0f;
    }

    public void DoSeekTweening(float fProgress_0_1)
    {
        OnTween(fProgress_0_1);
    }

    public void DoSetTweening(float fDeltaTime)
    {
        if (p_eTweenDirection == ETweenDirection.Forward && _fProgress_0_1 > 1f)
            _bIsFinishForward = true;
        else if (p_eTweenDirection == ETweenDirection.Reverse && _fProgress_0_1 < 0f)
            _bIsFinishRevese = true;

        if (p_fDuration == 0f)
        {
            Debug.LogWarning(name + " p_fDuration == 0f", this);
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

                case ETweenStyle.PingPong_ReverseCurve:
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

                case ETweenStyle.PingPong_ReverseCurve:
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
            _fProgress_0_1 += Mathf.Abs(1f / p_fDuration) * fDeltaTime * _iTweenDirectionDelta;

            if (p_eTweenStyle == ETweenStyle.PingPong_ReverseCurve && p_eTweenDirection == ETweenDirection.Reverse)
                OnTween(_pAnimationCurve_OnReverseCurvePlay.Evaluate(p_fProgress_0_1));
            else
                OnTween(p_pAnimationCurve.Evaluate(p_fProgress_0_1));
        }
    }


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnReset()
    {
        base.OnReset();

        p_pObjectTarget = gameObject;
        OnSetTarget(p_pObjectTarget);
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        _fProgress_0_1 = 0f;
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        if (p_pObjectTarget == null)
            OnSetTarget(gameObject);

        if (p_bIsPlay_OnEnable)
            DoPlayTween();
    }

    private void FixedUpdate()
    {
        if (p_bUseFixedUpdate == false)
            return;

        if (_bIsPlayingTween == false)
            return;

        if (_bIsFinishForward == _bIsFinishRevese)
        {
            DoSetTweening(Time.fixedDeltaTime);
            return;
        }

        if (p_Event_OnFinishTween != null)
            p_Event_OnFinishTween.Invoke();

        if (p_Event_OnFinishTween_InCludeArg != null)
            p_Event_OnFinishTween_InCludeArg(p_eTweenDirection, gameObject);

        _bIsPlayingTween = false;
    }

    /* protected - [abstract & virtual]         */

    abstract protected void OnSetTarget(GameObject pObjectNewTarget);
    abstract protected void OnTween(float fProgress_0_1);

    virtual protected void Reset() { }
    virtual protected void OnTweenStart(ETweenDirection eTweenDirection) { }
    virtual protected void OnTweenStart(string strTweenEvent, ETweenDirection eTweenDirection) { }

    virtual public object OnTween_EditorOnly(float fProgress_0_1) { return null; }
    virtual public void OnEditorButtonClick_SetStartValue_IsCurrentValue() { }
    virtual public void OnEditorButtonClick_SetDestValue_IsCurrentValue() { }
    virtual public void OnEditorButtonClick_SetCurrentValue_IsStartValue() { }
    virtual public void OnEditorButtonClick_SetCurrentValue_IsDestValue() { }

    virtual public void OnInitTween_EditorOnly() { }
    virtual public void OnReleaseTween_EditorOnly() { }

    virtual protected void OnDrawGizmosSelected() { }
    virtual protected void OnDrawGizmos()
    {
        if (g_bIsDrawGizmo == false)
            return;

        if (p_pObjectTarget == null)
            DoSetTarget(gameObject);

#if UNITY_EDITOR
        Vector3 vecPos = p_pObjectTarget.transform.position;

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.cyan;

        UnityEditor.Handles.Label(vecPos, "Tween Name : " + name, style);
        vecPos.y -= 3f;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(p_pObjectTarget.transform.position, 1f);
#endif
    }

    // ========================================================================== //

    #region Private

    void StartTween(bool bReset_Progress, ETweenDirection eTweenDirection)
    {
        DoSetTarget(p_pObjectTarget);

        if (_bIsPlayingTween && eTweenDirection != p_eTweenDirection)
            DoStopTween();
        OnTweenStart(eTweenDirection);

        _bIsPlayingTween = true;
        if (p_bUseFixedUpdate)
        {
            DoInitTween(eTweenDirection, bReset_Progress);

            if (p_Event_OnStartTween_InCludeArg != null)
                p_Event_OnStartTween_InCludeArg(eTweenDirection, gameObject);
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
                yield return new WaitForSeconds(p_fFirstDelaySec);
        }

        DoInitTween(eTweenDirection, bReset_Progress);

        if (p_Event_OnStartTween_InCludeArg != null)
            p_Event_OnStartTween_InCludeArg(eTweenDirection, gameObject);

        while (_bIsFinishForward == _bIsFinishRevese)
        {
            DoSetTweening(OnGetDeltaTime());

            CustomYieldInstruction pYield;
            OnCreatorYield(out pYield);

            yield return pYield;
        }

        if (p_Event_OnFinishTween != null)
            p_Event_OnFinishTween.Invoke();

        if (p_Event_OnFinishTween_InCludeArg != null)
            p_Event_OnFinishTween_InCludeArg(eTweenDirection, gameObject);

        _bIsPlayingTween = false;
    }

    // 이렇게 하면 안된다..
    //public WaitForSeconds_Custom OnCreate_YieldForSecond()
    //{
    //    return new WaitForSeconds_Custom(Time.deltaTime);
    //}

    //public WaitForSecondsRealtime OnCreate_YieldForSecond_Real()
    //{
    //    return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
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

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test