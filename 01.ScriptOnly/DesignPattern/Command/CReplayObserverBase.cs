#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-19 오후 3:38:17
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReplayDataList<T> : List<ReplayDataList<T>.SReplayData>
{
    [System.Serializable]
    public struct SReplayData : IRecordData<T>
    {
        public T Data { get; private set; }
        public float fRecordTime { get; private set; }

        public SReplayData(T sData, float fRecordTime)
        {
            Data = sData; this.fRecordTime = fRecordTime;
        }

        public T IRecordData_GetData()
        {
            return Data;
        }

        public float IRecordData_GetRecordTime()
        {
            return fRecordTime;
        }
    }

    // ====================================================================================

    public void Add(T data,float fTime)
    {
        base.Add(new SReplayData(data, fTime));
    }

    public int Calculate_StartIndex(float fTime)
    {
        int iFindStartIndex = Count - 1;
        for (int i = 0; i < Count; i++)
        {
            if (base[i].fRecordTime >= fTime)
            {
                iFindStartIndex = i;
                break;
            }
        }

        return iFindStartIndex;
    }

    public T GetLastValue()
    {
        if (Count == 0)
            return default(T);
        else
            return base[Count - 1].Data;
    }
}

/// <summary>
/// 
/// </summary>
public abstract class CReplayObserverBase : CObjectBase, IReplayObserver
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EReplayManageType
    {
        Manager,
        Custom,
    }

    /* public - Field declaration            */

    [DisplayName("Replay Manage Type")]
    public EReplayManageType p_eReplayManageType = EReplayManageType.Manager;

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        CManagerReplay.instance.p_Event_OnChange_ReplayState.Subscribe += OnChange_ReplayState;
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        if(p_eReplayManageType == EReplayManageType.Manager)
            CManagerReplay.instance.DoAddObserver(this);
    }

    protected override void OnDisableObject(bool bIsQuitApplciation)
    {
        base.OnDisableObject(bIsQuitApplciation);

        if (p_eReplayManageType == EReplayManageType.Manager)
        {
            if (bIsQuitApplciation == false)
                CManagerReplay.instance.DoRemoveObserver(this);
        }
    }

    /* protected - [abstract & virtual]         */

    public abstract void IReplayObserver_Record_ReplayData(float fCurrentTime, ref bool bIsRecordData_DefaultValue_Is_False);
    public abstract void IReplayObserver_Clear_ReplayData();
    public abstract void IReplayObserver_Seek_Replay(bool bIsFirstFrame, float fTime);

    protected virtual void OnChange_ReplayState(CManagerReplay.EReplayState eReplayState) { }

    // ========================================================================== //

    #region Private

    #endregion Private
}