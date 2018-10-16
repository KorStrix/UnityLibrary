#if NGUI
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_TestOne : CNGUIInventoryBase<Inventory_TestOne.SDataItem>
{
	public class SDataItem : IInventoryData<SDataItem>
	{
		string strSpriteName;

		public SDataItem(string strSpriteName)
		{
			this.strSpriteName = strSpriteName;
		}

		public string IInventoryData_GetImageName( )
		{
			return strSpriteName;
		}
	}

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		List<SDataItem> listData = new List<SDataItem>();
		listData.Add( new SDataItem( "NGUI" ) );
		listData.Add( new SDataItem( "Glow" ) );

		DoInitInventory( listData );
	}

	protected override EInventoryOption OnInitInventory()
	{
		return EInventoryOption.None;
	}
}
#endif