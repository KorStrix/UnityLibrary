#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-19 오후 1:57:05
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IRecordData<T>
{
    float IRecordData_GetRecordTime();
    T IRecordData_GetData();
}

public interface IReplayObserver
{
    void IReplayObserver_Record_ReplayData(float fCurrentTime, ref bool bIsRecordData_DefaultValue_Is_False);
    void IReplayObserver_Clear_ReplayData();
    void IReplayObserver_Seek_Replay(bool bIsFirstSeek, float fTime);
}

/// <summary>
/// 
/// </summary>
public class CManagerReplay : CSingletonDynamicMonoBase<CManagerReplay>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum ERecordWhen
    {
        OnAwake,
        OnEnable,
        CustomCall,
    }

    public enum EReplayState
    {
        Nothing,
        Recording,
        Replaying,
        Replaying_Pause,
    }

    /* public - Field declaration            */

    public ObservableCollection<EReplayState> p_Event_OnChange_ReplayState { get; private set; } = new ObservableCollection<EReplayState>();

    public struct Replay_Arg
    {
        float fRemainTime_Origin;
        float fCurrentPlayTime;

        public Replay_Arg(float fRemainTime_Origin, float fCurrentPlayTime)
        {
            this.fRemainTime_Origin = fRemainTime_Origin;
            this.fCurrentPlayTime = fCurrentPlayTime;
        }
    }

    /// <summary>
    /// RemainTime Origin, CurrentPlayTime
    /// </summary>
    public ObservableCollection<Replay_Arg> p_Event_OnPlayingReplay { get; private set; } = new ObservableCollection<Replay_Arg>();

    public bool p_bIsPlayingReplay
    {
        get
        {
            return _pCoroutineReplay != null;
        }
    }

    public bool p_bIsRecording { get; set; }
    public float p_fRecordStartTime { get; private set; }

    public ERecordWhen p_eRecordWhen = ERecordWhen.CustomCall;

    /* protected & private - Field declaration         */

    EReplayState _eReplayState = EReplayState.Nothing;
    List<IReplayObserver> listReplayObserver = new List<IReplayObserver>();
    Coroutine _pCoroutineReplay;
    bool _bIsRecord_OnFinishReplay;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoStart_Recording()
    {
        ChangeReplayState(EReplayState.Recording);
        StartRecording(true);
    }

    public void DoStart_Replay(bool bIsRecord_OnFinishReplay = true)
    {
        _bIsRecord_OnFinishReplay = bIsRecord_OnFinishReplay;
        p_bIsRecording = false;
        _pCoroutineReplay = StartCoroutine(CoReplaying(p_fRecordStartTime));
    }

    public void DoSeek_Replay(float fTime)
    {
        if (_eReplayState != EReplayState.Replaying_Pause)
            ChangeReplayState(EReplayState.Replaying_Pause);
        p_bIsRecording = false;

        for (int i = 0; i < listReplayObserver.Count; i++)
            listReplayObserver[i].IReplayObserver_Seek_Replay(true, fTime);
    }

    public void DoSeek_Replay_0_1(float fProgress_0_1)
    {
        DoSeek_Replay(fProgress_0_1.Convert_Delta_0_1_To_Min_Max(Time.time, p_fRecordStartTime));
    }

    public void DoClear_RecordData()
    {
        for (int i = 0; i < listReplayObserver.Count; i++)
            listReplayObserver[i].IReplayObserver_Clear_ReplayData();
    }

    public void DoStop_Recording()
    {
        p_bIsRecording = false;
    }

    public void DoStop_Replay()
    {
        if(_pCoroutineReplay != null)
            StopCoroutine(_pCoroutineReplay);
        OnFinish_Replay();
    }

    public void DoStartReplay_When_FewTimeAgo(float fTimeAgo, bool bIsRecord_OnFinishReplay = true)
    {
        _bIsRecord_OnFinishReplay = bIsRecord_OnFinishReplay;
        float fStartTime = Time.time - fTimeAgo;
        if (fStartTime < 0f)
            fStartTime = 0f;

        StartCoroutine(CoReplaying(fStartTime));
    }

    public void DoAddObserver(IReplayObserver pObserver)
    {
        if (listReplayObserver.Contains(pObserver) == false)
            listReplayObserver.Add(pObserver);
    }

    public void DoRemoveObserver(IReplayObserver pObserver)
    {
        if (listReplayObserver.Contains(pObserver))
            listReplayObserver.Remove(pObserver);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        if (p_eRecordWhen == ERecordWhen.OnAwake)
            DoStart_Recording();
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        if (p_eRecordWhen == ERecordWhen.OnEnable)
            DoStart_Recording();
    }

    public override void OnUpdate(float fTimeScale_Individual)
    {
        base.OnUpdate(fTimeScale_Individual);

        float fCurrentTime = Time.time;
        if(p_bIsRecording)
        {
            bool bIsRecord;
            for (int i = 0; i < listReplayObserver.Count; i++)
            {
                bIsRecord = false;
                listReplayObserver[i].IReplayObserver_Record_ReplayData(fCurrentTime, ref bIsRecord);
            }
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    IEnumerator CoReplaying(float fStartTime)
    {
        ChangeReplayState(EReplayState.Replaying);

        float fRemainTimeOrigin = Time.time - fStartTime;
        float fRemainTime = fRemainTimeOrigin;

        p_Event_OnPlayingReplay.DoNotify(new Replay_Arg(fRemainTimeOrigin, 0f));
        bool bIsFirstFrame = true;
        while (fRemainTime > 0f)
        {
            float fReplayTime = fStartTime + fRemainTimeOrigin - fRemainTime;
            for (int i = 0; i < listReplayObserver.Count; i++)
                listReplayObserver[i].IReplayObserver_Seek_Replay(bIsFirstFrame, fReplayTime);

            p_Event_OnPlayingReplay.DoNotify(new Replay_Arg(fRemainTimeOrigin, fReplayTime));
            bIsFirstFrame = false;
            fRemainTime -= Time.deltaTime;

            yield return null;
        }

        p_Event_OnPlayingReplay.DoNotify(new Replay_Arg(fRemainTimeOrigin, fRemainTimeOrigin));
        OnFinish_Replay();
    }

    void OnFinish_Replay()
    {
        _pCoroutineReplay = null;
        if (_bIsRecord_OnFinishReplay)
        {
            StartRecording(true);
            ChangeReplayState(EReplayState.Recording);
        }
        else
        {
            ChangeReplayState(EReplayState.Nothing);
        }
    }

    void ChangeReplayState(EReplayState eReplayState)
    {
        if (_eReplayState == eReplayState)
            return;

        _eReplayState = eReplayState;
        p_Event_OnChange_ReplayState.DoNotify(_eReplayState);
    }

    void StartRecording(bool bResetRecordStartTime)
    {
        p_bIsRecording = true;

        if(bResetRecordStartTime)
            p_fRecordStartTime = Time.time;
    }

    #endregion Private
}