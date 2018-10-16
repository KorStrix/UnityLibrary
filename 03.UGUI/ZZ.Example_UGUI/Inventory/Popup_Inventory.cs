using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Inventory : CUGUIPanelHasInputBase<Popup_Inventory.EInput>
{
	public enum EInput
	{
		주무기, 보조무기
	}

	[Header( "여기에 아무 이미지를 넣어주시기 바랍니다." )]
	public List<Sprite> p_listSprite = new List<Sprite>();

	[Header( "디버그확인용" )]
	[SerializeField]
	private EInput _eInput = EInput.주무기; public EInput p_eInput {  get { return _eInput; } }

	public List<Inventory_Item.SDataInventory> p_list_Equip = new List<Inventory_Item.SDataInventory>();
	public List<Inventory_Item.SDataInventory> p_list_Item = new List<Inventory_Item.SDataInventory>();

	[GetComponentInChildren]
	private Inventory_Equip _pInventory_Equip = null;
	[GetComponentInChildren]
	private Inventory_Item _pInventory_Item = null;

	public void DoRefresh_Invetory()
	{
		_pInventory_Equip.DoRefresh_InventoryData();
		_pInventory_Item.DoRefresh_InventoryData();
	}

	public override void OnButtons_Click( EInput eButtonName )
	{
		_eInput = eButtonName;

		DoRefresh_Invetory();
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		if (p_listSprite.Count == 0)
		{
			Debug.LogWarning( "인스펙터에서 p_listSprite를 세팅해주세요", this );
			return;
		}

		int iCount = p_listSprite.Count;
		int iIndex = 0;

		// 장비 세팅
		p_list_Item.Add( new Inventory_Item.SDataInventory( "기본_칼", "기본",  EInput.주무기, p_listSprite[iIndex++] ) ); iIndex %= iCount;
		p_list_Item.Add( new Inventory_Item.SDataInventory( "일반_칼2", "일반", EInput.주무기, p_listSprite[iIndex++] ) ); iIndex %= iCount;
		p_list_Item.Add( new Inventory_Item.SDataInventory( "희귀_도끼", "희귀",EInput.주무기, p_listSprite[iIndex++] ) ); iIndex %= iCount;
		p_list_Item.Add( new Inventory_Item.SDataInventory( "영웅_방패", "영웅",EInput.보조무기, p_listSprite[iIndex++] ) ); iIndex %= iCount;
		p_list_Item.Add( new Inventory_Item.SDataInventory( "전설_방패", "전설", EInput.보조무기, p_listSprite[iIndex++] ) ); iIndex %= iCount;

		p_list_Equip.Add( p_list_Item[0] );
		p_list_Equip.Add( p_list_Item[4] );
	}

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		DoRefresh_Invetory();
	}
}
