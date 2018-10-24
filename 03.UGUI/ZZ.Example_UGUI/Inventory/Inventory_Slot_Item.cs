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

public class Inventory_Slot_Item : CUGUIInventorySlot<Inventory_Slot_Item, Inventory_Item.SDataInventory>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EImage
	{
		Image_Icon,
		Image_Selected,
		Image_Equip,
		Image_New,
	}

	#region Field
	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private GameObject _pGoImage_Icon;
	private GameObject _pGoImage_Selected;
	private GameObject _pGoImage_Equip;
	private GameObject _pGoImage_New;

	private Popup_Inventory _pPopupOwner;

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

		//_pGoImage_Icon = this.GetGameObject( EImage.Image_Icon );
		//_pGoImage_Selected = this.GetGameObject( EImage.Image_Selected );
		//_pGoImage_Equip = this.GetGameObject( EImage.Image_Equip );
		//_pGoImage_New = this.GetGameObject( EImage.Image_New );

		//_pPopupOwner = GetComponentInParent<Popup_Inventory>();
	}

	public override void IInventorySlot_OnFillSlot( bool bEnable )
	{
		base.IInventorySlot_OnFillSlot( bEnable );

		_pGoImage_Icon.SetActive( bEnable );
		_pGoImage_Selected.SetActive( false );
		_pGoImage_Equip.SetActive( _pPopupOwner.p_list_Equip.Contains( p_pInventoryData ) );
		_pGoImage_New.SetActive( false );

		//if(GetSiblingIndex() == 1)
		//	Debug.Log( name + "IInventorySlot_OnEnableSlot" + bEnable );
	}

	protected override bool OnSetDataOrNull_And_CheckIsValidData( Inventory_Item.SDataInventory pData )
	{
		base.OnSetDataOrNull_And_CheckIsValidData( pData );

		if (pData == null) return false;

		//DoEditImage( "Image_Icon", pData.pSprite );
		//if (GetSiblingIndex() == 1)
		//	Debug.Log( name + "OnSetData" + pData.ToString() );

		return true;
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
