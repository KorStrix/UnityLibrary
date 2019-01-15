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

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public class CTweenMultiplePosition : CTweenBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    [System.Serializable]
    public class TweenData
    {
        [Rename_Inspector("시작 위치는 현재 위치로")]
        public bool bStartPosition_IsCurrentPosition = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideIf("bStartPosition_IsCurrentPosition")]
#endif
        [Rename_Inspector("시작 위치")]
        public Vector3 vecPosition_Start;
        [Rename_Inspector("도착 위치")]
        public Vector3 vecPosition_Dest;
        [Rename_Inspector("재생 시간")]
        public float fDurationSec;
    }

    /* public - Field declaration            */

    [Rename_Inspector("Is Local")]
    public bool p_bIsLocal = true;

    [SerializeField]
    [Rename_Inspector("Tween Start - 디버깅용", false)]
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

        _fProgress_0_1 = fDuration_PrevTotal.ProgressDelta_0_1(Calculate_TotalDuration());
    }


    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

class CTweenMultiple_Test : CTweenMultiplePosition
{
    [Test]
    public void Test_CalculateTween_Delta()
    {
        CTweenMultiple_Test pMultiplePosition;
        InitTweenTest(out pMultiplePosition);

        float fTotalTime = pMultiplePosition.Calculate_TotalDuration();
        float fProgress_0_1 = (fTotalTime / 2f) / fTotalTime;
        float fProgress_Calculated = 0f;

        pMultiplePosition.Calculate_CurrentTweenData(0f, out fProgress_Calculated);
        Assert.AreEqual(pMultiplePosition.p_iPlayIndex_Last, 0);

        float fProgress_Start_1 = pMultiplePosition.p_listTweenData[0].fDurationSec / fTotalTime;

        pMultiplePosition.Calculate_CurrentTweenData(fProgress_Start_1 + 0.001f, out fProgress_Calculated);
        Assert.AreEqual(pMultiplePosition.p_iPlayIndex_Last, 1);

        float fProgress_Start_2 = pMultiplePosition.p_listTweenData[1].fDurationSec / fTotalTime;
        fProgress_Start_2 += fProgress_Start_1;

        pMultiplePosition.Calculate_CurrentTweenData(fProgress_Start_2 + 0.001f, out fProgress_Calculated);
        Assert.AreEqual(pMultiplePosition.p_iPlayIndex_Last, 2);
    }

    [Test]
    public void Test_CalculateTween_Index()
    {
        CTweenMultiple_Test pMultiplePosition;
        InitTweenTest(out pMultiplePosition);

        float fDetlaTime = Time.deltaTime;
        for (int i = 0; i < 3; i++)
        {
            pMultiplePosition.CalculateProgress_ByIndex(i);
            pMultiplePosition.DoSetTweening(0f);
            Assert.AreEqual(pMultiplePosition.transform.position.ToString("F1"), pMultiplePosition.p_listTweenData[i].vecPosition_Start.ToString("F1"));
        }
    }

    private static void InitTweenTest(out CTweenMultiple_Test pMultiplePosition)
    {
        GameObject pObject = new GameObject();
        pMultiplePosition = pObject.AddComponent<CTweenMultiple_Test>();
        pMultiplePosition.transform.position = Vector3.zero;

        float fTotalTime = 0f;
        for (int i = 0; i < 3; i++)
        {
            TweenData pTweenData = new TweenData();
            pTweenData.vecPosition_Start = Vector3.one * i;
            pTweenData.vecPosition_Dest = Vector3.one * (i + 1);

            pTweenData.fDurationSec = i + 1;
            pMultiplePosition.p_listTweenData.Add(pTweenData);

            fTotalTime += pTweenData.fDurationSec;
        }
        pMultiplePosition.DoSetTarget(pObject);

        Assert.AreEqual(fTotalTime, pMultiplePosition.Calculate_TotalDuration());
    }
}

#endif
#endregion Test