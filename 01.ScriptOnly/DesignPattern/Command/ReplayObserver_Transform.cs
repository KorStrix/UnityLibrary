#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-19 오후 1:50:04
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class ReplayObserver_Transform : CReplayObserverBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [DisplayName("Record Position")]
    public bool p_bIsRecord_Position = true;
    [DisplayName("Record Rotation")]
    public bool p_bIsRecord_Rotation = true;
    [DisplayName("Record Scale")]
    public bool p_bIsRecord_Scale = false;

    /* protected & private - Field declaration         */

    [GetComponent]
    Transform _pTransform_Target = null;

    ReplayDataList<Vector3> _listRecord_Position = new ReplayDataList<Vector3>();
    ReplayDataList<Quaternion> _listRecord_Rotation = new ReplayDataList<Quaternion>();
    ReplayDataList<Vector3> _listRecord_Scale = new ReplayDataList<Vector3>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    // ========================================================================== //

    /* protected - Override & Unity API         */

    public override void IReplayObserver_Record_ReplayData(float fCurrentTime, ref bool bIsRecordData_DefaultValue_Is_False)
    {
        bool bIsDebug = CheckDebugFilter(EDebugFilter.Debug_Level_Core);

        if (p_bIsRecord_Position)
        {
            Vector3 vecPosition = _pTransform_Target.position;
            if (_listRecord_Position.GetLastValue() != vecPosition)
            {
                _listRecord_Position.Add(vecPosition, fCurrentTime);
                bIsRecordData_DefaultValue_Is_False = true;

                if (bIsDebug)
                    Debug.Log(name + "Record Position " + vecPosition);
            }
        }

        if (p_bIsRecord_Rotation)
        {
            Quaternion rotation = _pTransform_Target.rotation;
            if (_listRecord_Rotation.GetLastValue() != rotation)
            {
                _listRecord_Rotation.Add(rotation, fCurrentTime);
                bIsRecordData_DefaultValue_Is_False = true;

                if (bIsDebug)
                    Debug.Log(name + "Record Position " + rotation);
            }
        }

        if (p_bIsRecord_Scale)
        {
            Vector3 vecScale = _pTransform_Target.localScale;
            if (_listRecord_Scale.GetLastValue() != vecScale)
            {
                _listRecord_Scale.Add(vecScale, fCurrentTime);
                bIsRecordData_DefaultValue_Is_False = true;

                if (bIsDebug)
                    Debug.Log(name + "Record Position " + vecScale);
            }
        }
    }

    public override void IReplayObserver_Clear_ReplayData()
    {
        _listRecord_Position.Clear();
        _listRecord_Rotation.Clear();
        _listRecord_Scale.Clear();
    }

    public override void IReplayObserver_Seek_Replay(bool bIsFirstFrame, float fTime)
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log($"{GetType().GetFriendlyName()} {nameof(IReplayObserver_Seek_Replay)} Contain Replay Data Position Count : {_listRecord_Position.Count} _listRecord_Rotation Count : {_listRecord_Rotation.Count}");

        if (p_bIsRecord_Position)
            SeekReplay(fTime, _listRecord_Position, Replay_Position, Vector3.Lerp);

        if (p_bIsRecord_Rotation)
            SeekReplay(fTime, _listRecord_Rotation, Replay_Rotation, Quaternion.Lerp);

        if (p_bIsRecord_Scale)
            SeekReplay(fTime, _listRecord_Scale, Replay_Scale, Vector3.Lerp);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void SeekReplay<T>(float fTime, ReplayDataList<T> listRecordData, System.Action<T> OnReplay, System.Func<T, T, float, T> OnLerp) 
    {
        int iFindStartIndex = listRecordData.Calculate_StartIndex(fTime);
        if (iFindStartIndex != listRecordData.Count - 1)
        {
            var pNextRecordData = listRecordData[iFindStartIndex + 1];
            var pCurrentRecordData = listRecordData[iFindStartIndex];

            float fProgress = fTime.Convert_ThisValue_To_Delta_0_1(pNextRecordData.IRecordData_GetRecordTime(), pCurrentRecordData.IRecordData_GetRecordTime());
            OnReplay(OnLerp(pCurrentRecordData.IRecordData_GetData(), pNextRecordData.IRecordData_GetData(), fProgress));
        }
        else
            OnReplay(listRecordData[iFindStartIndex].IRecordData_GetData());
    }

    void Replay_Position(Vector3 vecData)
    {
        _pTransform_Target.position = vecData;
    }

    void Replay_Rotation(Quaternion rotData)
    {
        _pTransform_Target.rotation = rotData;
    }

    void Replay_Scale(Vector3 vecData)
    {
        _pTransform_Target.localScale = vecData;
    }

    #endregion Private
}