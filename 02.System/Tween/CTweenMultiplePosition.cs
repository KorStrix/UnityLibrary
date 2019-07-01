#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-01-02 오후 5:10:25
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CTweenMultiplePosition : CTweenBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    [System.Serializable]
    public class TweenData
    {
        [DisplayName("시작 위치는 현재 위치로")]
        public bool bStartPosition_IsCurrentPosition = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideIf("bStartPosition_IsCurrentPosition")]
#endif
        [DisplayName("시작 위치")]
        public Vector3 vecPosition_Start;
        [DisplayName("도착 위치")]
        public Vector3 vecPosition_Dest;
        [DisplayName("재생 시간")]
        public float fDurationSec;
    }

    /* public - Field declaration            */

    [DisplayName("Is Local")]
    public bool p_bIsLocal = true;

    [SerializeField]
    [DisplayName("Tween Start - 디버깅용", false)]
    Vector3 _vecTweenStartPos;

    public List<TweenData> p_listTweenData = new List<TweenData>();

    /* protected & private - Field declaration         */

    Transform _pTransformTarget;
    Vector3 _vecPos_Backup;

    float _fTotalDuration;
    int _iPlayIndex_Last; public int p_iPlayIndex_Last { get { return _iPlayIndex_Last; } }

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Button("Add Tween Data")]
#endif
    public void DoAddTweenData()
    {
        TweenData pTweenData = new TweenData();

        if(p_bIsLocal)
            pTweenData.vecPosition_Dest = transform.localPosition;
        else
            pTweenData.vecPosition_Dest = transform.position;

        pTweenData.fDurationSec = 1f;

        p_listTweenData.Add(pTweenData);
    }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Button("Play Test Tween")]
#endif
    public void DoPlayTween_Index_InMultiple(int iIndex)
    {
        if (iIndex > p_listTweenData.Count)
            return;

        CalculateProgress_ByIndex(iIndex);
        DoPlayTween_Forward(false);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    public override bool Check_Editor_IsDrawButton_SetTweenData()
    {
        return false;
    }

    protected override void Reset()
    {
        base.Reset();

        TweenData pTweenData = new TweenData();
        pTweenData.vecPosition_Start = transform.position;
        pTweenData.vecPosition_Dest = transform.position;

        p_listTweenData.Add(pTweenData);
    }

    protected override float CalculateProgressDelta(float fDeltaTime)
    {
        return Mathf.Abs(1f / _fTotalDuration) * fDeltaTime;
    }

    protected override void OnTweenStart(ETweenDirection eTweenDirection)
    {
        _fTotalDuration = Calculate_TotalDuration();
        _iPlayIndex_Last = -1;
    }

    protected override void OnSetTarget(GameObject pObjectNewTarget)
    {
        _pTransformTarget = pObjectNewTarget.transform;
    }

    protected override void OnTween(float fProgress_0_1)
    {
        float fProgress_Calculated;
        TweenData sTweenData = Calculate_CurrentTweenData(fProgress_0_1, out fProgress_Calculated);
        Vector3 vecPosition = _vecTweenStartPos * (1f - fProgress_Calculated) + sTweenData.vecPosition_Dest * fProgress_Calculated;

        if (p_bIsLocal)
            _pTransformTarget.localPosition = vecPosition;
        else
            _pTransformTarget.position = vecPosition;
    }

    public override void OnInitTween_EditorOnly()
    {
        _vecPos_Backup = _pTransformTarget.position;
    }

    public override void OnReleaseTween_EditorOnly()
    {
        _pTransformTarget.position = _vecPos_Backup;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    protected float Calculate_TotalDuration()
    {
        _fTotalDuration = 0f;
        for (int i = 0; i < p_listTweenData.Count; i++)
            _fTotalDuration += p_listTweenData[i].fDurationSec;

        return _fTotalDuration;
    }

    protected TweenData Calculate_CurrentTweenData(float fProgress_0_1, out float fProgress_0_1_Calculated)
    {
        fProgress_0_1_Calculated = 0f;
        float fCurrentPlayTime = fProgress_0_1 * _fTotalDuration;
        float fCalculateDuration_Prev = 0f;
        float fCalculateDuration = p_listTweenData[0].fDurationSec;
        int iIndex = -1;
        for (int i = 0; i < p_listTweenData.Count; i++)
        {
            if (fCurrentPlayTime <= fCalculateDuration)
            {
                iIndex = i;

                float fCurrentDataDuration = fCalculateDuration - fCalculateDuration_Prev;
                fProgress_0_1_Calculated = ((fCurrentPlayTime - fCalculateDuration_Prev) / fCurrentDataDuration);
                break;
            }
            else
            {
                if(i != p_listTweenData.Count - 1)
                {
                    fCalculateDuration_Prev = fCalculateDuration;
                    fCalculateDuration += p_listTweenData[i + 1].fDurationSec;
                }
            }
        }

        if(iIndex > p_listTweenData.Count)
        {
            Debug.LogError(name + " iIndex: " + iIndex + "fCalculateDuration_Prev : " + fCalculateDuration_Prev + " fCalculateDuration : " + fCalculateDuration, this);
            return p_listTweenData[_iPlayIndex_Last];
        }

        if (_iPlayIndex_Last != iIndex)
        {
            _iPlayIndex_Last = iIndex;

            if (p_listTweenData[iIndex].bStartPosition_IsCurrentPosition)
                _vecTweenStartPos = p_bIsLocal ? _pTransformTarget.localPosition : _pTransformTarget.position;
            else
                _vecTweenStartPos = p_listTweenData[iIndex].vecPosition_Start;
        }

        return p_listTweenData[_iPlayIndex_Last];
    }

    protected void CalculateProgress_ByIndex(int iIndex)
    {
        float fDuration_PrevTotal = 0f;
        for (int i = 0; i < iIndex; i++)
            fDuration_PrevTotal += p_listTweenData[i].fDurationSec;

        _fProgress_0_1 = fDuration_PrevTotal.Convert_ThisValue_To_Delta_0_1(Calculate_TotalDuration());
    }


    #endregion Private
}