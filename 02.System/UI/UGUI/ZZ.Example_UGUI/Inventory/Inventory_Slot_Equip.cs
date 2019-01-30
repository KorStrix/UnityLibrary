#region Header
/* ============================================ 
 *	설계자 : 
 *	작성자 : KJH
 *	
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Inventory_Slot_Equip : CUGUIInventorySlot<Inventory_Slot_Equip, Inventory_Item.SDataInventory>, IDictionaryItem<Popup_Inventory.EInput>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EImage
	{
		Image_Icon,
		Image_Slot,
		Image_Rune,
	}

	#region Field
	/* public - Field declaration            */

	[SerializeField]
	private Popup_Inventory.EInput eInput = Popup_Inventory.EInput.보조무기;

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private GameObject _pGoImage_Icon;
	//private GameObject _pGoImage_Slot;
	//private GameObject _pGoImage_Rune;

	#endregion Field

	#region Public
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	#endregion Public

	// ========================================================================== //

	#region Protected
	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pGoImage_Icon = this.GetGameObject_InChildren( EImage.Image_Icon );
		//_pGoImage_Slot = GetGameObject( EImage.Image_Slot );
		//_pGoImage_Rune = GetGameObject( EImage.Image_Rune );
	}

	protected override bool OnSetDataOrNull_And_CheckIsValidData( Inventory_Item.SDataInventory pData )
	{
		base.OnSetDataOrNull_And_CheckIsValidData( pData );

		bool bExistNewData = pData != null;
		bool bExistAlreadyData = p_pInventoryData != null;
		if (bExistAlreadyData && bExistNewData )
		{
			Debug.Log( name + "bExistAlreadyData - Not Set Data " + pData);
			return false;
		}

		_pGoImage_Icon.SetActive( bExistNewData );

		//if(bExistNewData)
		//	DoEditImage( EImage.Image_Icon, pData.pSprite );
		//else
		//	DoEditImage( EImage.Image_Icon, null );

		return true;
	}

	public Popup_Inventory.EInput IDictionaryItem_GetKey()
	{
		return eInput;
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
