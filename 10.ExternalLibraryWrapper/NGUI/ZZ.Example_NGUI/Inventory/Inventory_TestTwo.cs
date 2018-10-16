#if NGUI
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_TestTwo : CNGUIInventoryBase<Inventory_TestTwo.SDataItem>
{
	public class SDataItem : IInventoryData<SDataItem>
	{
		string strImageName;

		public SDataItem(string strImageName)
		{
			this.strImageName = strImageName;
		}
		public string IInventoryData_GetImageName( )
		{
			throw new NotImplementedException();
		}
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		List<SDataItem> listData = new List<SDataItem>();
		listData.Add(new SDataItem("A"));
		listData.Add(new SDataItem("B"));

		DoInitInventory(listData);
	}

	protected override EInventoryOption OnInitInventory()
	{
		return EInventoryOption.MoveSlotItem;
	}
}
#endif