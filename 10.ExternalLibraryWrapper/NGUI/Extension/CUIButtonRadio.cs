#if NGUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */
   
public class CUIButtonRadio : CNGUIPanelBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */
	
	[SerializeField]	[HideInInspector]
	public int _iRadioGroup;
	[SerializeField]	[HideInInspector]
	public int _iRadioIndex;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	static private Dictionary<int, List<CUIButtonRadio>> g_mapButtonRadioList = new Dictionary<int, List<CUIButtonRadio>>();
	static private Dictionary<int, CUIButtonRadio> g_mapButtonRadioSelect = new Dictionary<int, CUIButtonRadio>();

	private string _strSpriteName_OnPress;
	private string _strSpriteName_OnNormal;
	private UIButton _pButtonTarget;
	private UISprite _pSpriteTarget;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoClick_RadioButton()
	{
		_pSpriteTarget.spriteName = _strSpriteName_OnPress;

		List<CUIButtonRadio> listButtonRadio = g_mapButtonRadioList[_iRadioGroup];
		for (int i = 0; i < listButtonRadio.Count; i++)
		{
			CUIButtonRadio pUIButtonRadio = listButtonRadio[i];
			if (pUIButtonRadio != this)
			{
				if (pUIButtonRadio._pButtonTarget == null)
					pUIButtonRadio.EventOnAwake();

				if (pUIButtonRadio._pButtonTarget.isEnabled == false) continue;

				pUIButtonRadio._pSpriteTarget.spriteName = pUIButtonRadio._strSpriteName_OnNormal;
			}
		}

		g_mapButtonRadioSelect[_iRadioGroup] = this;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pButtonTarget = GetComponent<UIButton>();
		_pSpriteTarget = _pButtonTarget.tweenTarget.GetComponent<UISprite>();
		_strSpriteName_OnNormal = _pButtonTarget.normalSprite;
		_strSpriteName_OnPress = _pButtonTarget.pressedSprite;

		g_mapButtonRadioList.Clear();

		if (g_mapButtonRadioList.ContainsKey( _iRadioGroup ) == false)
		{
			List<CUIButtonRadio> listButtonRadio = new List<CUIButtonRadio>();
			g_mapButtonRadioList.Add( _iRadioGroup, listButtonRadio );

			Transform pTransParents = transform.parent;
			pTransParents.GetComponentsInChildren( listButtonRadio );
			g_mapButtonRadioSelect[_iRadioGroup] = null;
		}
	}

	private void OnSelect(bool bSelect)
	{
		if (bSelect == false && g_mapButtonRadioSelect[_iRadioGroup] == this)
			_pSpriteTarget.spriteName = _strSpriteName_OnPress;
	}

	protected override void OnUIHover( bool bHover )
	{
		base.OnUIHover( bHover );

		if (g_mapButtonRadioSelect[_iRadioGroup] == this)
			_pSpriteTarget.spriteName = _strSpriteName_OnPress;
	}

	protected override void OnUIClick()
	{
		base.OnUIClick();

		DoClick_RadioButton();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
#endif