using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Item : CInventoryBase<Inventory_Item.SDataInventory, Inventory_Slot_Item>
{
	public class SDataInventory : IInventoryData<SDataInventory>
	{
		public string str아이템이름;
		public Sprite pSprite;
		public string str등급이름;
		public Popup_Inventory.EInput eEquipType;

		public SDataInventory(string str아이템이름, string str등급이름, Popup_Inventory.EInput eEquipType, Sprite pSprite)
		{
			this.str아이템이름 = str아이템이름;
			this.eEquipType = eEquipType;
			this.str등급이름 = str등급이름;
			this.pSprite = pSprite;
		}

		public string IInventoryData_GetImageName( )
		{			
			return null;
		}
	}

	private Popup_Inventory _pOwnerPopup;
	private List<SDataInventory> _listDataNote = new List<SDataInventory>();

	protected override void OnAwake()
	{
		base.OnAwake();

		_pOwnerPopup = GetComponentInParent<Popup_Inventory>();
	}

	protected override void OnInventory_ClickSlot( Inventory_Slot_Item pInventorySlot, SDataInventory pData )
	{
		base.OnInventory_ClickSlot( pInventorySlot, pData );

		Debug.Log( pInventorySlot.name + pData.IInventoryData_GetImageName(), pInventorySlot );

		if (pData == null) return;

		bool bIsRequireRefresh = true;
		for(int i = 0; i < _pOwnerPopup.p_list_Equip.Count; i++)
		{
			if (_pOwnerPopup.p_list_Equip[i].eEquipType == pData.eEquipType)
			{
				bIsRequireRefresh = false;
				break;
			}
		}

		if(bIsRequireRefresh)
		{
			_pOwnerPopup.p_list_Equip.Add( pData );
			_pOwnerPopup.DoRefresh_Invetory();
		}
	}

	protected override List<SDataInventory> GetInventoryData()
	{
		_listDataNote.Clear();
		Popup_Inventory.EInput eInput = _pOwnerPopup.p_eInput;
		for(int i = 0;i < _pOwnerPopup.p_list_Item.Count; i++)
		{
			if(eInput == _pOwnerPopup.p_list_Item[i].eEquipType )
				_listDataNote.Add( _pOwnerPopup.p_list_Item[i] );
		}

		return _listDataNote;
	}

	protected override EInventoryType GetInventoryType()
	{
		return EInventoryType.Page;
	}
}
