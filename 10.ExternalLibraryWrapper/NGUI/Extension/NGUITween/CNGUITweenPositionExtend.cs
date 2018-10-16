#if NGUI
using UnityEngine;
using System.Collections.Generic;
using System;

public class CNGUITweenPositionExtend : CNGUITweenExtendBase<CNGUITweenPositionExtend.STweenPositionInfo>
{
    [System.Serializable]
    public class STweenPositionInfo : STweenInfoBase
    {
        public Vector3 vecFrom;
        public Vector3 vecTo;


		public override float GetFromToGap(bool bCalculate = false)
		{
			if(bCalculate)
				_fGap = Vector3.Distance( vecFrom, vecTo );

			return _fGap;
		}
	}
    [HideInInspector]
    public bool worldSpace = false;
    
    private Transform m_pTransform;
    private UIRect m_pRect;
    
    public Transform cachedTransform { get { if (m_pTransform == null) m_pTransform = transform; return m_pTransform; } }

    [System.Obsolete("Use 'value' instead")]
    public Vector3 position { get { return this.value; } set { this.value = value; } }

    public Vector3 value
    {
        get
        {
            return worldSpace ? cachedTransform.position : cachedTransform.localPosition;
        }
        set
        {
            if (m_pRect == null || !m_pRect.isAnchored || worldSpace)
            {
                if (worldSpace) cachedTransform.position = value;
                else cachedTransform.localPosition = value;
            }
            else
            {
                value -= cachedTransform.localPosition;
                NGUIMath.MoveRect(m_pRect, value.x, value.y);
            }
        }
    }

    //=============================== [2. Start Overriding] =================================//
    #region Overriding

    void Awake() { m_pRect = GetComponent<UIRect>(); }

    protected override void UpdateTweenValue(float fFactor, bool bIsFinished)
    {
		Vector3 vecOriginPos = value;
		value = m_pCurrentTweenInfo.vecFrom * (1f - fFactor) + m_pCurrentTweenInfo.vecTo * fFactor;

		if (_bCheckTweenAmount)
		{
			float fGap = Vector3.Distance( vecOriginPos, value );

			int iLoopCountOrigin = (int)(p_fTweenAmount / m_pCurrentTweenInfo.GetFromToGap());
			p_fTweenAmount += fGap;
			int iLoopCountCurrent = (int)(p_fTweenAmount / m_pCurrentTweenInfo.GetFromToGap());

			if (iLoopCountOrigin != iLoopCountCurrent)
				p_fTweenAmount = iLoopCountCurrent * m_pCurrentTweenInfo.GetFromToGap();
		}
	}

	[ContextMenu("Set 'From' to current value")]
    public override void SetStartToCurrentValue()
    { if(m_pCurrentTweenInfo != null) m_pCurrentTweenInfo.vecFrom = value; }

    [ContextMenu("Set 'To' to current value")]
    public override void SetEndToCurrentValue()
    { if (m_pCurrentTweenInfo != null) m_pCurrentTweenInfo.vecTo = value; }

    #endregion
}
#endif