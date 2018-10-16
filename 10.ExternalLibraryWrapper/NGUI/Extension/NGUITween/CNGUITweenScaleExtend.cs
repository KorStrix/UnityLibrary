#if NGUI
using UnityEngine;
using System.Collections;
using System;

public class CNGUITweenScaleExtend : CNGUITweenExtendBase<CNGUITweenScaleExtend.STweenScaleInfo>
{
    [System.Serializable]
    public class STweenScaleInfo : STweenInfoBase
    {
        public Vector3 vecFrom;
        public Vector3 vecTo;

		public override float GetFromToGap( bool bCalculate )
		{
			if(bCalculate)
				_fGap = Vector3.Distance( vecFrom, vecTo );

			return _fGap;
		}
	}

    public bool m_bUpdateTable = false;

    private Transform m_pTransform;
    private UITable m_Table;

    public Transform cachedTransform { get { if (m_pTransform == null) m_pTransform = transform; return m_pTransform; } }

    public Vector3 value { get { return cachedTransform.localScale; } set { cachedTransform.localScale = value; } }

    [System.Obsolete("Use 'value' instead")]
    public Vector3 scale { get { return this.value; } set { this.value = value; } }

    //=============================== [2. Start Overriding] =================================//
    #region Overriding
        
    protected override void UpdateTweenValue(float fFactor, bool bIsFinished)
    {
        value = m_pCurrentTweenInfo.vecFrom * (1f - fFactor) + m_pCurrentTweenInfo.vecTo * fFactor;

        if (m_bUpdateTable)
        {
            if (m_Table == null)
            {
                m_Table = NGUITools.FindInParents<UITable>(gameObject);
                if (m_Table == null) { m_bUpdateTable = false; return; }
            }
            m_Table.repositionNow = true;
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