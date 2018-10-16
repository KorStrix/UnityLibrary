#if NGUI
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Manager : CManagerNGUIBase<Inventory_Manager, Inventory_Manager.EInventoryPopup>
{
	public enum EInventoryPopup
	{
		Inventory_TestOne,
		Inventory_TestTwo
	}
	
	protected override void OnUpdate()
	{
		base.OnUpdate();

		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			DoShowHide_Panel(EInventoryPopup.Inventory_TestOne, true);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			DoShowHide_Panel( EInventoryPopup.Inventory_TestTwo, true);
		}
	}

	protected override void OnDefaultPanelShow()
	{
	}
}
#endif