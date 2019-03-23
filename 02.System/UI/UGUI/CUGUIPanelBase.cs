#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent( typeof( Canvas ))]
public class CUGUIPanelBase : CUIObjectBase, IUIPanel
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    #region Field

    /* public - Field declaration            */

    [Rename_Inspector("항상 보여줄 건지 유무")]
    public bool p_bIsAlwaysShow = false;
	[Rename_Inspector("SortOrder를 고정 시킬 건지 유무")]
	public bool p_bIsFixedSortOrder = false;
	
	bool IUIPanel.p_bIsAlwaysShow
	{
		get
		{
			return p_bIsAlwaysShow;
		}
	}

	bool IUIPanel.p_bIsFixedSortOrder
	{
		get
		{
			return p_bIsFixedSortOrder;
		}
	}

	public int p_iHashCode
	{
		get
		{
			return _iHashCode;
		}
	}

	public IManagerUI p_pManagerUI
	{
		get
		{
			return _pManagerUI;
		}
	}

    public class CObserverShowHide : CObserverSubject<CUGUIPanelBase, bool>
    {
        public delegate void OnPanelShowHide(CUGUIPanelBase pUIPanel, bool bIsShow);

        public void DoRegist_ShowHide(OnPanelShowHide OnPanelShowHide)
        {
            DoRegist_Listener(new Action<CUGUIPanelBase, bool>(OnPanelShowHide));
        }

        public void DoNotify_ShowHide(CUGUIPanelBase pUIPanel, bool bIsShow)
        {
            DoNotify(pUIPanel, bIsShow);
        }
    }

    public CObserverShowHide p_Event_OnShowHide { get; private set; } = new CObserverShowHide();

    /* protected - Field declaration         */

    protected Dictionary<string, Text> _mapText = new Dictionary<string, Text>();

    /* private - Field declaration           */

    private IManagerUI _pManagerUI;
	private int _iHashCode;

    Text[] _arrChildText;

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoShow_UGUIPanel()
	{
		_pManagerUI.IManagerUI_ShowHide_Panel( _iHashCode, true );
	}

	public void DoShow_UGUIPanel(System.Action OnFinishAnimation)
	{
		_pManagerUI.IManagerUI_ShowHide_Panel( _iHashCode, true, OnFinishAnimation );
	}

	public void DoHide_UGUIPanel()
	{
		_pManagerUI.IManagerUI_ShowHide_Panel( _iHashCode, false );
	}

	public void DoHide_UGUIPanel( System.Action OnFinishAnimation )
	{
		_pManagerUI.IManagerUI_ShowHide_Panel( _iHashCode, false, OnFinishAnimation );
	}

	public void IUIPanel_Init( IManagerUI pManagerUI, int iHashCode )
	{
		_pManagerUI = pManagerUI;
		_iHashCode = iHashCode;
	}

	public void IUIPanel_SetOrder( int iSortOrder )
	{
		transform.SetSiblingIndex( iSortOrder );
	}

	public IEnumerator IUIPanel_OnShowPanel_PlayingAnimation( int iSortOrder )
	{
		yield return StartCoroutine( OnShowPanel_PlayingAnimation( iSortOrder ) );
	}

	public IEnumerator IUIPanel_OnHidePanel_PlayingAnimation()
	{
		yield return StartCoroutine( OnHidePanel_PlayingAnimation() );
    }

    public void IUIPanel_OnShow()
    {
        p_Event_OnShowHide.DoNotify_ShowHide(this, true);
        OnShowPanel();
    }

    public void IUIPanel_OnHide()
    {
        p_Event_OnShowHide.DoNotify_ShowHide(this, false);
        OnHidePanel();
    }

    public Text GetText_OrNull<TextName>(TextName strTextName)
    {
        string strText = strTextName.ToString();
        if (_mapText.ContainsKey(strText) == false)
        {
            for (int i = 0; i < _arrChildText.Length; i++)
            {
                Text pText = _arrChildText[i];
                if (pText.name == strText)
                {
                    _mapText.Add(pText.name, pText);
                    break;
                }
            }
        }

        return _mapText[strText];
    }

    /* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

    #endregion Public

    // ========================================================================== //

    #region Protected

    /* protected - [abstract & virtual]         */

    virtual protected IEnumerator OnShowPanel_PlayingAnimation( int iSortOrder ) { yield return null; }
	virtual protected IEnumerator OnHidePanel_PlayingAnimation() { yield return null; }

    virtual protected void OnShowPanel() { }
    virtual protected void OnHidePanel() { }

    /* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _arrChildText = GetComponentsInChildren<Text>();
    }

    #endregion Protected

    // ========================================================================== //

    #region Private

    /* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

    #endregion Private
}
