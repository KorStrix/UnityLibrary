#if Spine

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Spine.Unity;
using Spine;
using System;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class WaitFor_SpineAnimation : CustomYieldInstruction
{
    CSpineController _pController;

    string _strAnimation_Started;
    bool _bWaitForAnimation;
    float _fTimeFinish;

    public WaitFor_SpineAnimation(CSpineController pController)
    {
        _pController = pController;
        if (_pController.p_bIsPlaying)
        {
            _bWaitForAnimation = true;
            _strAnimation_Started = _pController.p_strCurrentAnimationName;
            _pController.p_pAnimation.state.Interrupt += State_Interrupt;
            _pController.p_pAnimation.state.Complete += State_Complete;

            _fTimeFinish = Time.time + _pController.p_fAnimationStartTime + Time.deltaTime;
        }
        else
            _bWaitForAnimation = false;
    }


    private void State_Interrupt(TrackEntry trackEntry)
    {
        _pController.p_pAnimation.state.Interrupt -= State_Interrupt;
        _pController.p_pAnimation.state.Complete -= State_Complete;
        _bWaitForAnimation = false;
    }

    private void State_Complete(TrackEntry trackEntry)
    {
        _pController.p_pAnimation.state.Complete -= State_Complete;
        _pController.p_pAnimation.state.Interrupt -= State_Interrupt;
        _bWaitForAnimation = false;
    }

    public override bool keepWaiting
    {
        get
        {
            if (string.IsNullOrEmpty(_strAnimation_Started) || string.IsNullOrEmpty(_pController.p_strCurrentAnimationName))
                return false;

            if (_strAnimation_Started != _pController.p_strCurrentAnimationName)
                return false;

            if (_pController.p_bIsLooping)
                return false;

            if (Time.time > _fTimeFinish)
                return false;

            return _bWaitForAnimation;
        }
    }
}

public class CSpineController : CObjectBase, IAnimationController
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    public event OnAnimationEvent p_Event_OnAnimationEvent;
    public event OnPlayAnimation p_Event_OnAnimation_Start;
    public event OnFinishAnimation p_Event_OnAnimation_Finish;

    public string p_strCurrentAnimationName { get { return p_pAnimation.AnimationName; } }

    [Rename_Inspector("애니메이션 이벤트 이름 출력")]
    public bool p_bPrintAnimationEvent = false;
    [Rename_Inspector("플레이 할 애니메이션 이름 출력")]
    public bool p_bPrintAnimationName = false;

    [GetComponentInChildren]
    public SkeletonAnimation p_pAnimation { get; protected set; }

    public int p_iWaitQueueCount { get { return _queueAnimationWait.Count; } }
    public bool p_bIsPlaying { get; private set; }
    public bool p_bIsLooping { get; private set; }
    public float p_fAnimationStartTime { get; private set; }
    public float p_fProgress0_1
    {
        get
        {
            TrackEntry pTrackEntry = p_pAnimation.state.GetCurrent(0);
            if (string.IsNullOrEmpty(p_pAnimation.AnimationName))
                return 0f;

            return pTrackEntry.AnimationTime / pTrackEntry.AnimationEnd;
        }
    }

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    Queue<string> _queueAnimationWait = new Queue<string>();

    private SkeletonData _pSkeletonData;
    // private Skeleton _pSkeleton;

    private Dictionary<string, OnFinishAnimation> _mapOnFinishAnimation = new Dictionary<string, OnFinishAnimation>();
    private System.Action _OnFinishAnimation_Continudly;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoSet_TimeScale(float fTimeScale)
    {
        if (p_pAnimation != null)
            p_pAnimation.timeScale = fTimeScale;
    }

    public bool DoAddQueue_AnimationContinuedly<ENUM_ANIMATION_NAME>(ENUM_ANIMATION_NAME eAnimName)
    {
        string strAnimName = eAnimName.ToString();
        bool bSuccess = _pSkeletonData.FindAnimation(strAnimName) != null;
        if (bSuccess)
            _queueAnimationWait.Enqueue(strAnimName);

        return bSuccess;
    }

    public void DoClear_AnimationContinuedly()
    {
        _queueAnimationWait.Clear();
    }

    public void DoSeekAnimation<ENUM_ANIMATION_NAME>(ENUM_ANIMATION_NAME eAnimName, float fProgress_0_1)
    {
        if (_pSkeletonData == null)
            return;

        string strAnimName = eAnimName.ToString();
        bool bSuccess = _pSkeletonData.FindAnimation(strAnimName) != null;
        if (bSuccess)
        {
            p_pAnimation.loop = false;
            p_pAnimation.AnimationName = "";
            p_pAnimation.AnimationName = strAnimName;
            TrackEntry pTrackEntry = p_pAnimation.state.GetCurrent(0);
            pTrackEntry.TrackTime = pTrackEntry.AnimationEnd * fProgress_0_1;

            if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
                Debug.Log(name + " Seek Animation Name : " + strAnimName, this);
        }
        else
        {
            if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
                Debug.LogError(name + " Spine - NotFoundAnimation - " + strAnimName, this);
        }
    }

    /// <summary>
    /// 현재 플레이하는 애니메이션 이름과 재생하고자 할 애니메이션 이름이 같을 경우 안될 수 있음
    /// </summary>
    /// <typeparam name="ENUM_ANIM_NAME"></typeparam>
    /// <param name="eAnimName"></param>
    /// <returns></returns>
    public bool DoPlayAnimation<ENUM_ANIMATION_NAME>(ENUM_ANIMATION_NAME eAnimName)
    {
        if (_pSkeletonData == null || eAnimName == null)
            return false;

        string strAnimName = eAnimName.ToString();
        bool bSuccess = _pSkeletonData.FindAnimation(strAnimName) != null;
        if (bSuccess)
        {
            p_pAnimation.loop = false;
            p_pAnimation.state.Complete -= OnFinish_AnimationState;
            Excute_OnFinishAnimation(p_pAnimation.AnimationName, true);

            // 스파인 API에서 애니메이션 이름이 같으면 동작을 안한다.
            if (p_pAnimation.AnimationName == strAnimName)
                p_pAnimation.AnimationName = "";

            p_pAnimation.AnimationName = strAnimName;
            p_pAnimation.state.Event -= OnCall_AnimationEvent;
            p_pAnimation.state.Event += OnCall_AnimationEvent;
            p_pAnimation.state.Complete += OnFinish_AnimationState;

            p_fAnimationStartTime = Time.time;
            p_bIsPlaying = true;
            if (p_bPrintAnimationName)
                Debug.Log(name + " Play Spine Animation Name : " + strAnimName + " Current Animation : " + p_pAnimation.state.GetCurrent(0).Animation.Name, this);


            if (p_Event_OnAnimation_Start != null)
                p_Event_OnAnimation_Start(strAnimName);
        }
        else
        {
            if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
                Debug.LogError(name + " Spine - NotFoundAnimation - " + strAnimName, this);
        }
        p_bIsLooping = false;

        return bSuccess;
    }

    public void DoPlayAnimation_Continuedly<ENUM_ANIMATION_NAME>(System.Action OnFinishAnimationAll, params ENUM_ANIMATION_NAME[] arrAnimName)
        where ENUM_ANIMATION_NAME : IConvertible, IComparable
    {
        DoPlayAnimation(arrAnimName[0]);

        for (int i = 1; i < arrAnimName.Length; i++)
            DoAddQueue_AnimationContinuedly(arrAnimName[i]);

        _OnFinishAnimation_Continudly = OnFinishAnimationAll;
    }

    public void DoPlayAnimation_ForceChange_OnSameAnimation<ENUM_ANIMATION_NAME>(ENUM_ANIMATION_NAME eAnimName, OnFinishAnimation OnFinishAnimation = null)
        where ENUM_ANIMATION_NAME : IConvertible, IComparable
    {
        p_pAnimation.AnimationName = "";
        DoPlayAnimation(eAnimName, OnFinishAnimation);
    }

    public bool DoPlayAnimation<ENUM_ANIM_NAME>(ENUM_ANIM_NAME eAnimName, OnFinishAnimation OnFinishAnimation)
            where ENUM_ANIM_NAME : System.IConvertible, System.IComparable
    {
        bool bSuccess = DoPlayAnimation(eAnimName);
        if (bSuccess)
        {
            if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
                Debug.Log(name + " Play Spine Animation Name : " + p_pAnimation.state.GetCurrent(0).Animation.Name + " Current Animation : " + p_pAnimation.state.GetCurrent(0).Animation.Name, this);

            Add_OnFinishAnimation(eAnimName.ToString(), OnFinishAnimation);
        }

        return bSuccess;
    }

    public bool DoPlayAnimation_Loop<ENUM_ANIM_NAME>(ENUM_ANIM_NAME eAnimName)
        where ENUM_ANIM_NAME : System.IConvertible, System.IComparable
    {
        if (eAnimName == null)
        {
            Debug.LogError(name + "DoPlayAnimation_Loop Animation Name is Null", this);
            return false;
        }

        string strAnimName = eAnimName.ToString();

        if (_pSkeletonData == null)
        {
            EventOnAwake();
            if (_pSkeletonData == null)
            {
                Debug.LogWarning(name + " _pSkeletonData == null", this);
                return false;
            }
        }

        bool bSuccess = _pSkeletonData.FindAnimation(strAnimName) != null;
        if (bSuccess)
        {
            // 애니메이션이 같으면 루프 설정을 무시하기때문에 일부러 틀린 애니메이션 삽입
            if (p_pAnimation.AnimationName == strAnimName && p_pAnimation.loop)
                p_pAnimation.AnimationName = "";

            p_pAnimation.loop = true;
            p_pAnimation.AnimationName = strAnimName;

            p_pAnimation.state.Complete -= OnFinish_AnimationState;
            p_pAnimation.state.Event -= OnCall_AnimationEvent;
            p_pAnimation.state.Event += OnCall_AnimationEvent;

            if (p_bPrintAnimationName)
                Debug.Log(name + " Play Spine Animation Name : " + strAnimName);

            p_fAnimationStartTime = Time.time;
            p_bIsPlaying = true;
            p_bIsLooping = true;

            if (p_Event_OnAnimation_Start != null)
                p_Event_OnAnimation_Start(strAnimName);
        }

        return bSuccess;
    }

    public bool DoCheckIsPlaying<ENUM_ANIMATION_NAME>(ENUM_ANIMATION_NAME eAnimName)
        where ENUM_ANIMATION_NAME : IConvertible, IComparable
    {
        if (eAnimName == null)
            return false;

        if (CheckDebugFilter(EDebugFilter.Debug_Level_LowLevel))
            Debug.Log(name + " DoCheckIsPlaying - p_pAnimation.AnimationName : " + p_pAnimation.AnimationName + "// eAnimName : " + eAnimName, this);

        return p_pAnimation.AnimationName == eAnimName.ToString();
    }


    public void DoSetOrderInLayer(int iOrder)
    {
        MeshRenderer pRenderer = p_pAnimation.GetComponent<MeshRenderer>();
        pRenderer.sortingOrder = iOrder;
    }

    public void DoResetAnimationEvent()
    {
        p_Event_OnAnimationEvent = null;
    }

    public void DoStopAnimation()
    {
        p_pAnimation.AnimationName = "";
        p_bIsPlaying = false;
    }

    public void DoSetAnimationSpeed(float fSpeed)
    {
        p_pAnimation.timeScale = fSpeed;
    }

    public string GetCurrentAnimation()
    {
        return p_pAnimation.AnimationName;
    }

    /* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        if (p_pAnimation.SkeletonDataAsset == null)
        {
            Debug.LogWarning(name + "스켈레톤 데이터 에셋이 없다", this);
            return;
        }

        p_pAnimation.Initialize(false);
        _pSkeletonData = p_pAnimation.SkeletonDataAsset.GetSkeletonData(false);
        // _pSkeleton = p_pAnimation.skeleton;
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

    private void OnCall_AnimationEvent(TrackEntry trackEntry, Spine.Event e)
    {
        string strKeyName = e.Data.Name;
        if (p_Event_OnAnimationEvent != null)
            p_Event_OnAnimationEvent(trackEntry.Animation.Name, strKeyName);

        if (p_bPrintAnimationEvent)
            Debug.Log(name + " Animation Event Name : " + strKeyName, this);
    }

    private void OnFinish_AnimationState(TrackEntry trackEntry)
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log(name + " State_End - Animation : " + trackEntry.Animation.Name + "// Current Animation : " + p_pAnimation.state.GetCurrent(0).Animation.Name, this);

        if (trackEntry.Animation.Name != p_pAnimation.state.GetCurrent(0).Animation.Name)
            return;

        p_pAnimation.state.Complete -= OnFinish_AnimationState;
        p_bIsPlaying = false;

        string strAnimationName = trackEntry.Animation.Name;
        Excute_OnFinishAnimation(strAnimationName, false);

        if (_queueAnimationWait.Count == 0)
        {
            if (_OnFinishAnimation_Continudly != null)
            {
                if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
                    Debug.Log(name + " State_End - Animation : " + trackEntry.Animation.Name + "// Current OnFinish Animation : " + _OnFinishAnimation_Continudly.Method.Name + "// Current Animation : " + p_pAnimation.state.GetCurrent(0).Animation.Name, this);

                var OnFinishAnimationBackup = _OnFinishAnimation_Continudly;
                _OnFinishAnimation_Continudly = null;
                OnFinishAnimationBackup();
            }
        }
        else
        {
            DoPlayAnimation(_queueAnimationWait.Dequeue());
        }
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

    private void Excute_OnFinishAnimation(string strAnimation, bool bIsInterrupted)
    {
        if (string.IsNullOrEmpty(strAnimation))
            return;

        if (_mapOnFinishAnimation.ContainsKey(strAnimation))
        {
            var OnFinishAnimation = _mapOnFinishAnimation[strAnimation];
            _mapOnFinishAnimation.Remove(strAnimation);
            OnFinishAnimation(strAnimation, bIsInterrupted);

            if (CheckDebugFilter(EDebugFilter.Debug_Level_LowLevel))
                Debug.Log(name + " Excute_OnFinishAnimation - Animation : " + strAnimation + "// bIsInterrupted Animation : " + bIsInterrupted + "// OnFinishAnimation : " + OnFinishAnimation.Method.Name, this);
        }

        if (p_Event_OnAnimation_Finish != null)
            p_Event_OnAnimation_Finish(strAnimation, bIsInterrupted);
    }

    private void Add_OnFinishAnimation(string strAnimationName, OnFinishAnimation OnFinishAnimation)
    {
        if (OnFinishAnimation != null)
        {
            if (_mapOnFinishAnimation.ContainsKey(strAnimationName))
                _mapOnFinishAnimation[strAnimationName] = OnFinishAnimation;
            else
                _mapOnFinishAnimation.Add(strAnimationName, OnFinishAnimation);
        }
        else
        {
            if (_mapOnFinishAnimation.ContainsKey(strAnimationName))
                _mapOnFinishAnimation.Remove(strAnimationName);
        }

    }
}
#endif