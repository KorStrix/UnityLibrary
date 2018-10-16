#if NGUI
using UnityEngine;
using System.Collections;
using System;

public class CNGUITweenRotationExtend : CNGUITweenExtendBase<CNGUITweenRotationExtend.STweenRotationInfo>
{
    [System.Serializable]
    public class STweenRotationInfo : STweenInfoBase
    {
        public Vector3 vecFrom;
        public Vector3 vecTo;

		public override float GetFromToGap(bool bCalculate)
		{
			if(bCalculate)
				_fGap = Vector3.Distance( vecFrom, vecTo );

			return _fGap;
		}
	}

    public bool m_bQuaternionLerp = false;

    private Transform m_pTransform;

    public Transform cachedTransform { get { if (m_pTransform == null) m_pTransform = transform; return m_pTransform; } }

    [System.Obsolete("Use 'value' instead")]
    public Quaternion rotation { get { return this.value; } set { this.value = value; } }

    public Quaternion value { get { return cachedTransform.localRotation; } set { cachedTransform.localRotation = value; } }

    //=============================== [2. Start Overriding] =================================//
    #region Overriding

    protected override void UpdateTweenValue(float fFactor, bool bIsFinished)
    {
        value = m_bQuaternionLerp ? Quaternion.Slerp(Quaternion.Euler(m_pCurrentTweenInfo.vecFrom), Quaternion.Euler(m_pCurrentTweenInfo.vecTo), fFactor) :
            Quaternion.Euler(new Vector3(
            Mathf.Lerp(m_pCurrentTweenInfo.vecFrom.x, m_pCurrentTweenInfo.vecTo.x, fFactor),
            Mathf.Lerp(m_pCurrentTweenInfo.vecFrom.y, m_pCurrentTweenInfo.vecTo.y, fFactor),
            Mathf.Lerp(m_pCurrentTweenInfo.vecFrom.z, m_pCurrentTweenInfo.vecTo.z, fFactor)));
    }

    [ContextMenu("Set 'From' to current value")]
    public override void SetStartToCurrentValue()
    { if (m_pCurrentTweenInfo != null) m_pCurrentTweenInfo.vecFrom = value.eulerAngles; }

    [ContextMenu("Set 'To' to current value")]
    public override void SetEndToCurrentValue()
    { if (m_pCurrentTweenInfo != null) m_pCurrentTweenInfo.vecTo = value.eulerAngles; }

    #endregion
}
#endif