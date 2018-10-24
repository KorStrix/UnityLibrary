#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header
#if NGUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CUIScrollViewBase<CLASS_Data> : CObjectBase
	where CLASS_Data : class, IInventoryData<CLASS_Data>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	protected Dictionary<int, CLASS_Data> _mapInventoryData = new Dictionary<int, CLASS_Data>();
	protected List<CNGUIInventorySlot> _listInventorySlot = new List<CNGUIInventorySlot>();

	protected CNGUIInventorySlot _pCurrentSelectSlot;
	protected CLASS_Data _pCurrentSelectData = null;

	/* private - Field declaration           */

	private int _iSlotCurrentIndex; protected int p_iSlotcurrentIndex { get { return _iSlotCurrentIndex; } }

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoInitInventory( List<CLASS_Data> listData )
	{
		_mapInventoryData.Clear();
		for (int i = 0; i < _listInventorySlot.Count; i++)
		{
			if (listData != null && i < listData.Count)
			{
				//Debug.Log(listData[i].IInventoryData_GetSpriteName());
				OnSlot_Fill( _listInventorySlot[i], listData[i] );
				_mapInventoryData.Add( i, listData[i] );
			}
			else
				OnSlot_Empty( _listInventorySlot[i] );
		}
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	virtual protected void OnSlot_Click( int iSlotIndex )
	{
		_iSlotCurrentIndex = iSlotIndex;
		_pCurrentSelectData = EventGetSlotData( iSlotIndex );
		//OnSlot_ClickIncludeData( iSlotIndex, _pCurrentSelectData, EventGetSlot( iSlotIndex ).p_eSlotState );
	}

	virtual protected void OnSlot_Press( int iSlotIndex, bool bPress, CNGUIInventorySlot pSlot )
	{
		_iSlotCurrentIndex = iSlotIndex;
		//if ((_eOption & EInventoryOption.MoveSlotItem) == EInventoryOption.MoveSlotItem)
		//{
		//	if (bPress)
		//	{
		//		if (_mapInventoryData.ContainsKey( iSlotIndex ) == false) return;

		//		_pCurrentSelectSlot = pSlot;
		//		_pCurrentSelectData = EventGetSlotData( iSlotIndex );
		//		pSlot.EventItem_ColliderOnOff( false );

		//		DebugCustom.Log( "Example", "Inventory", "Press On" + _pCurrentSelectData.IInventoryData_GetImageName(), pSlot );
		//	}
		//	else
		//	{
		//		if (_pCurrentSelectData == null) return;

		//		pSlot.EventItem_ColliderOnOff( true );
		//		_mapInventoryData[_pCurrentSelectSlot.p_iSlotIndex] = _mapInventoryData[iSlotIndex];
		//		_mapInventoryData[iSlotIndex] = _pCurrentSelectData;

		//		OnSlot_Fill( pSlot, _pCurrentSelectData );
		//		OnSlot_Empty( _pCurrentSelectSlot );

		//		_pCurrentSelectData = null;
		//		_pCurrentSelectSlot = null;

		//		DebugCustom.Log( "Example", "Inventory", "Press Off" + pSlot.name, pSlot );
		//	}
		//}
	}

	virtual protected void OnSlot_Fill( CNGUIInventorySlot pSlot, CLASS_Data pData )
	{
		//pSlot.DoSetItem( pData.IInventoryData_GetImageName( pSlot ) );
		pSlot.DoLockItem( false );
		//if (_eOption != EInventoryOption.MoveSlotItem)
		//	pSlot.EventSlot_ColliderOnOff( true );
	}
	
	virtual protected void OnSlot_Empty( CNGUIInventorySlot pSlot )
	{
		//pSlot.DoClearSlot( _strOnEmptySprite );
		//if (_eOption != EInventoryOption.MoveSlotItem)
		//	pSlot.EventSlot_ColliderOnOff( false );
	}

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	protected CLASS_Data EventGetSlotData( int iSlotIndex )
	{
		if (_mapInventoryData.ContainsKey( iSlotIndex ) == false)
		{
			Debug.LogWarning( iSlotIndex + "에 데이터가 없다!!" );
			return null;
		}

		return _mapInventoryData[iSlotIndex];
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		GetComponentsInChildren( true, _listInventorySlot );
		for (int i = 0; i < _listInventorySlot.Count; i++)
			_listInventorySlot[i].p_EVENT_OnPress += OnSlot_Press;
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
   #endif