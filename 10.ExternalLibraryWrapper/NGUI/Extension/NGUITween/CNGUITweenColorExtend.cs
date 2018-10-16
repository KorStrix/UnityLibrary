#if NGUI
using UnityEngine;
using System.Collections;
using System;

public class CNGUITweenColorExtend : CNGUITweenExtendBase<CNGUITweenColorExtend.STweenColorInfo>
{
    [System.Serializable]
    public class STweenColorInfo : STweenInfoBase
    {
        public Color pColorFrom;
        public Color pColorTo;

		public override float GetFromToGap(bool bCaclulate)
		{
			if(bCaclulate)
			{
				float fFrom = pColorFrom.a + pColorFrom.r + pColorFrom.g + pColorFrom.b;
				float fTo = pColorTo.a + pColorTo.r + pColorTo.g + pColorTo.b;

				_fGap = Mathf.Abs( fFrom - fTo );
			}

			return _fGap;
		}
	}

    bool mCached = false;
    UIWidget mWidget;
    Material mMat;
    Light mLight;
    SpriteRenderer mSr;

    void Cache()
    {
        mCached = true;
        mWidget = GetComponent<UIWidget>();
        if (mWidget != null) return;

        mSr = GetComponent<SpriteRenderer>();
        if (mSr != null) return;

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
        Renderer ren = renderer;
#else
        Renderer ren = GetComponent<Renderer>();
#endif
        if (ren != null)
        {
            mMat = ren.material;
            return;
        }

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
        mLight = light;
#else
        mLight = GetComponent<Light>();
#endif
        if (mLight == null) mWidget = GetComponentInChildren<UIWidget>();
    }

    [System.Obsolete("Use 'value' instead")]
    public Color color { get { return this.value; } set { this.value = value; } }

    /// <summary>
    /// Tween's current value.
    /// </summary>

    public Color value
    {
        get
        {
            if (!mCached) Cache();
            if (mWidget != null) return mWidget.color;
            if (mMat != null) return mMat.color;
            if (mSr != null) return mSr.color;
            if (mLight != null) return mLight.color;
            return Color.black;
        }
        set
        {
            if (!mCached) Cache();
            if (mWidget != null) mWidget.color = value;
            else if (mMat != null) mMat.color = value;
            else if (mSr != null) mSr.color = value;
            else if (mLight != null)
            {
                mLight.color = value;
                mLight.enabled = (value.r + value.g + value.b) > 0.01f;
            }
        }
    }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void UpdateTweenValue(float fFactor, bool bIsFinished)
    {
        value = Color.Lerp(m_pCurrentTweenInfo.pColorFrom, m_pCurrentTweenInfo.pColorTo, fFactor);
    }
    
    [ContextMenu("Set 'From' to current value")]
    public override void SetStartToCurrentValue()
    { if(m_pCurrentTweenInfo != null) m_pCurrentTweenInfo.pColorFrom = value; }

    [ContextMenu("Set 'To' to current value")]
    public override void SetEndToCurrentValue()
    { if (m_pCurrentTweenInfo != null) m_pCurrentTweenInfo.pColorTo = value; }
}
#endif