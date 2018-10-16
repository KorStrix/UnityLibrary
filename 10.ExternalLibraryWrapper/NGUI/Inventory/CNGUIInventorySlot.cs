#if NGUI
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CNGUIInventorySlot : CObjectBase, IComparer<CNGUIInventorySlot>//, IInventorySlot<
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public event System.Action<int, bool, CNGUIInventorySlot> p_EVENT_OnPress;

	[SerializeField]
	private UISprite _pSprite_Background = null;
	[SerializeField]
	private UISprite _pSprite_Item = null;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Collider _pColliderSlot;
	private Collider _pColliderItem;
	private EInventorySlotState _eSlotState; public EInventorySlotState p_eSlotState {  get { return _eSlotState; } }

	public int p_iSlotIndex;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetItem(string strSpriteName)
	{
		_pSprite_Item.spriteName = strSpriteName;
		_eSlotState = EInventorySlotState.Fill;
	}

	public void DoLockItem(bool bLock)
	{
		Color pColor = Color.white;
		if (bLock)
		{
			_eSlotState = EInventorySlotState.Fill_And_Lock;
			pColor.a = 0.5f;
		}
		else
			_eSlotState = EInventorySlotState.Fill;

		_pSprite_Item.color = pColor;
	}

	public void DoClearSlot(string strSpriteName)
	{
		_pSprite_Item.spriteName = strSpriteName;
		EventItem_ColliderOnOff(false);
		_eSlotState = EInventorySlotState.Empty;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void EventSlot_ColliderOnOff(bool bOn)
	{
		if(_pColliderSlot != null)
			_pColliderSlot.enabled = bOn;
	}

	public void EventItem_ColliderOnOff(bool bOn)
	{
		if(_pColliderItem != null)
			_pColliderItem.enabled = bOn;
	}

	public void EventSetDragDropItem()
	{
		_pSprite_Item.gameObject.AddComponent<UIDragDropItem>();
		CCompoEventTrigger pEventTrigger = _pSprite_Item.gameObject.AddComponent<CCompoEventTrigger>();
		pEventTrigger.p_eInputType_Main = CCompoEventTrigger.EInputType.OnPress;
		pEventTrigger.p_OnPress += OnPress;

		NGUITools.AddWidgetCollider(_pSprite_Item.gameObject);
		_pColliderItem = _pSprite_Item.GetComponent<Collider>();
	}

	public int Compare(CNGUIInventorySlot x, CNGUIInventorySlot y)
	{
		if (x.p_iSlotIndex < y.p_iSlotIndex)
			return -1;
		else if (x.p_iSlotIndex > y.p_iSlotIndex)
			return 1;
		else
			return 0;
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	private void Reset()
	{
		Debug.Log("Reset!!" + name, this);

		if (Application.isEditor == false) return;

		if (GetComponent<BoxCollider>() == null)
			NGUITools.AddWidgetCollider(gameObject);
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		if (_pSprite_Background == null)
		{
			_pSprite_Background = GetComponent<UISprite>();
			if (_pSprite_Background == null)
				_pSprite_Background = gameObject.AddComponent<UISprite>();
		}

		_pColliderSlot = _pSprite_Background.GetComponent<Collider>();
		if (_pSprite_Item == null)
		{
			Debug.Log("pSprite_Item을 세팅해주시기 바랍니다.", this);
		}
	}

	private void OnDrop()
	{
		p_EVENT_OnPress(p_iSlotIndex, false, this);
	}

	// ========================================================================== //

	/* private - [Proc] Function             
	   로직을 처리(Process Local logic)           */

	private void OnPress(bool bPress)
	{
		if (p_EVENT_OnPress != null)
			p_EVENT_OnPress(p_iSlotIndex, bPress, this);
	}

	/* private - Other[Find, Calculate] Func 
	   찾기, 계산등 단순 로직(Simpe logic)         */
}
#endif