#if NGUI
using UnityEngine;
using System.Collections.Generic;
using System;

public class CNGUITweenAlphaExtend : CNGUITweenExtendBase<CNGUITweenAlphaExtend.STweenAlphaInfo>
{
    [System.Serializable]
    public class STweenAlphaInfo : STweenInfoBase
    {
        public float fFrom;
        public float fTo;

		public override float GetFromToGap(bool bCalculate)
		{
			if(bCalculate)
				_fGap = Mathf.Abs( fFrom - fTo );

			return _fGap;
		}
	}
    
    private UIRect m_pRect;
    private Material m_pMaterialTarget;
    private SpriteRenderer m_pSpriteRenderer;
	private UnityEngine.UI.MaskableGraphic _pImage;
	private UnityEngine.UI.MaskableGraphic[] _arrImage_UGUI;

	private bool m_bCached = false;


    [System.Obsolete("Use 'value' instead")]
    public float alpha { get { return this.value; } set { this.value = value; } }

    public virtual float value
    {
        get
        {
            if (!m_bCached) Cache();
            if (m_pRect != null) return m_pRect.alpha;
            if (m_pSpriteRenderer != null) return m_pSpriteRenderer.color.a;
			if (_pImage != null) return _pImage.color.a;

			return m_pMaterialTarget != null ? m_pMaterialTarget.color.a : 1f;
        }
        set
        {
            if (!m_bCached) Cache();

            if (m_pRect != null)
            {
                m_pRect.alpha = value;
            }
            else if (m_pSpriteRenderer != null)
            {
                Color c = m_pSpriteRenderer.color;
                c.a = value;
                m_pSpriteRenderer.color = c;
            }
            else if (m_pMaterialTarget != null)
            {
                Color c = m_pMaterialTarget.color;
                c.a = value;
                m_pMaterialTarget.color = c;
            }
			else if(_pImage != null)
			{
				Color c = _pImage.color;
				c.a = value;
				_pImage.color = c;

				if(_arrImage_UGUI != null)
				{
					for (int i = 0; i < _arrImage_UGUI.Length; i++)
					{
						Color cChildren = _arrImage_UGUI[i].color;
						cChildren.a = value;
						_arrImage_UGUI[i].color = cChildren;
					}
				}
			}
		}
    }

    protected virtual void Cache()
    {
        m_bCached = true;
        m_pRect = GetComponent<UIRect>();
        m_pSpriteRenderer = GetComponent<SpriteRenderer>();
		_pImage = GetComponent<UnityEngine.UI.MaskableGraphic>();
		_arrImage_UGUI = GetComponentsInChildren<UnityEngine.UI.MaskableGraphic>();

		if (m_pRect == null && m_pSpriteRenderer == null)
        {
            Renderer ren = GetComponent<Renderer>();
            if (ren != null) m_pMaterialTarget = ren.material;
            if (m_pMaterialTarget == null) m_pRect = GetComponentInChildren<UIRect>();
        }
    }

    //=============================== [2. Start Overriding] =================================//
    #region Overriding

    protected override void UpdateTweenValue(float fFactor, bool bIsFinished)
    {
        value = Mathf.Lerp(m_pCurrentTweenInfo.fFrom, m_pCurrentTweenInfo.fTo, fFactor);
    }

    public override void SetStartToCurrentValue()
    { if(m_pCurrentTweenInfo != null) m_pCurrentTweenInfo.fFrom = value; }

    public override void SetEndToCurrentValue()
    { if(m_pCurrentTweenInfo != null) m_pCurrentTweenInfo.fTo = value; }

    #endregion
}
#endif