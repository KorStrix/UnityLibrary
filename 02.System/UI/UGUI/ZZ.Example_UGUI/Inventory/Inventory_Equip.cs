using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Equip : CInventoryBase<Inventory_Item.SDataInventory, Inventory_Slot_Equip >
{
	private Dictionary<Popup_Inventory.EInput, Inventory_Slot_Equip> _mapEquipSlot = new Dictionary<Popup_Inventory.EInput, Inventory_Slot_Equip>();
	private Popup_Inventory _pOwnerPopup;

	protected override void OnAwake()
	{
		base.OnAwake();

		_pOwnerPopup = GetComponentInParent<Popup_Inventory>();
		_mapEquipSlot.DoAddItem(_mapInventorySlot.Values);
	}

	protected override List<Inventory_Item.SDataInventory> GetInventoryData()
	{
		return _pOwnerPopup.p_list_Equip;
	}

	protected override EInventoryType GetInventoryType()
	{
		return EInventoryType.None;
	}

	protected override void OnInventory_RefreshInventoryData( List<Inventory_Item.SDataInventory> listData )
	{
		base.OnInventory_RefreshInventoryData( listData );

		for(int i = 0; i < listData.Count; i++)
		{
			if (_mapEquipSlot.ContainsKey_PrintOnError( listData[i].eEquipType ))
				_mapEquipSlot[listData[i].eEquipType].IInventorySlot_OnSetData( listData[i], null );
		}
	}
	
	protected override void OnInventory_ClickSlot( Inventory_Slot_Equip pInventorySlot, Inventory_Item.SDataInventory pData )
	{
		base.OnInventory_ClickSlot( pInventorySlot, pData );

		Debug.Log( pInventorySlot.name + pData.IInventoryData_GetImageName(), pInventorySlot );

		if (pData == null) return;

		pInventorySlot.IInventorySlot_OnSetData( null, null );
		if (_pOwnerPopup.p_list_Equip.Contains_PrintOnError( pData ))
			_pOwnerPopup.p_list_Equip.Remove( pData );

		_pOwnerPopup.DoRefresh_Invetory();
	}
}
