#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-19 오후 1:56:42
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class ReplayObserver_Command : CReplayObserverBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    /* protected & private - Field declaration         */

    [GetComponent]
    ICommandExecuter _pCommandExecuter = null;

    ReplayDataList<CommandExcuted> _listReplayData = new ReplayDataList<CommandExcuted>();
    ReplayDataList<CommandExcuted> _listReplayData_Temp = new ReplayDataList<CommandExcuted>();
    float _fPrevRecordTime;
    float _fPrevSeekTime;

    // Dictionary<string, CommandExecuteData> _pReplayData_Origin = new Dictionary<string, CommandExecuteData>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _pCommandExecuter.p_Event_OnExecuteCommand.Subscribe +=
            (arg) =>
            {
                var pCommandExecuted = arg.pCommandExecuted;

                if (pCommandExecuted.sInputValue.bIsAlwaysInput)
                {
                    // AlwaysInput은 별도로 처리해야 한다.
                }

                _listReplayData_Temp.Add(pCommandExecuted, pCommandExecuted.fExcuteTime);
            };
    }

    protected override void OnChange_ReplayState(CManagerReplay.EReplayState eReplayState)
    {
        base.OnChange_ReplayState(eReplayState);

        switch (eReplayState)
        {
            case CManagerReplay.EReplayState.Recording:
                _fPrevRecordTime = 0f;
                _pCommandExecuter.p_bEnableExecuter = true;
                break;

            case CManagerReplay.EReplayState.Replaying:
            case CManagerReplay.EReplayState.Replaying_Pause:
                _pCommandExecuter.p_bEnableExecuter = false;
                break;
        }
    }

    public override void IReplayObserver_Clear_ReplayData()
    {
        _listReplayData_Temp.Clear();
        _listReplayData.Clear();
    }

    public override void IReplayObserver_Record_ReplayData(float fCurrentTime, ref bool bIsRecordData_DefaultValue_Is_False)
    {
        int iOriginCount = _listReplayData.Count;
        for (int i = 0; i < _listReplayData_Temp.Count; i++)
        {
            ReplayDataList<CommandExcuted>.SReplayData pReplayData = _listReplayData_Temp[i];
            float fRecordTime = pReplayData.fRecordTime;
            if (_fPrevRecordTime <= fRecordTime && fRecordTime <= fCurrentTime)
                _listReplayData.Add(pReplayData);
        }
        _listReplayData_Temp.Clear();

        _fPrevRecordTime = fCurrentTime;
        bIsRecordData_DefaultValue_Is_False = iOriginCount != _listReplayData.Count;
    }

    public override void IReplayObserver_Seek_Replay(bool bIsFirstFrame, float fTime)
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log($"{GetType().GetFriendlyName()}  {nameof(IReplayObserver_Seek_Replay)} Contain Replay Data Count : {_listReplayData.Count}" + " bIsFirstFrame : " + bIsFirstFrame + " fTime : " + fTime);

        // 못찾을 경우 -1을 리턴
        int iFindStartIndex = _listReplayData.Calculate_StartIndex(fTime);
        iFindStartIndex -= 1;
        if (iFindStartIndex >= 0)
        {
            for (int i = iFindStartIndex; i < _listReplayData.Count; i++)
            {
                ReplayDataList<CommandExcuted>.SReplayData sReplayData = _listReplayData[i];
                if (_fPrevSeekTime <= sReplayData.fRecordTime)
                {
                    if (sReplayData.fRecordTime > fTime)
                        break;

                    sReplayData.IRecordData_GetData().DoExcute();
                }
            }
        }

        _fPrevSeekTime = fTime;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}