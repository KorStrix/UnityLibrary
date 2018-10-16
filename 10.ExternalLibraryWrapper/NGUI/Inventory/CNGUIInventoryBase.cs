#if NGUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public enum EInventorySlotState
{
	Fill,
	Fill_And_Lock,
	Empty
}

abstract public class CNGUIInventoryBase<CLASS_Data> : CObjectBase
	where CLASS_Data : class, IInventoryData<CLASS_Data>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	[System.Flags]
	public enum EInventoryOption
	{
		None = 0,
		MoveSlotItem = 1,
	}

	/* public - Variable declaration            */

	[SerializeField]
	private string _strOnEmptySprite = "";

	/* protected - Variable declaration         */

	protected Dictionary<int, CLASS_Data> _mapInventoryData = new Dictionary<int, CLASS_Data>();
	protected List<CNGUIInventorySlot> _listInventorySlot = new List<CNGUIInventorySlot>();

	protected CNGUIInventorySlot _pCurrentSelectSlot;
	protected CLASS_Data _pCurrentSelectData = null;

	/* private - Variable declaration           */

	private EInventoryOption _eOption = EInventoryOption.None;
	private int _iSlotCurrentIndex;	protected int p_iSlotcurrentIndex {  get { return _iSlotCurrentIndex; } }

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoInitInventory(List<CLASS_Data> listData)
	{
		_mapInventoryData.Clear();
		for (int i = 0; i < _listInventorySlot.Count; i++)
		{
			if (listData != null && i < listData.Count)
			{
				OnSlot_Fill(_listInventorySlot[i], listData[i]);
				_mapInventoryData.Add(i, listData[i]);
			}
			else
				OnSlot_Empty(_listInventorySlot[i]);
		}
	}

	public void DoInitInventory<ENUM_Key, CLASS_Info>(Dictionary<ENUM_Key, CLASS_Data> mapDataFill, Dictionary<ENUM_Key, CLASS_Info> mapInfoUnLock)
	{
		_mapInventoryData.Clear();
		List<KeyValuePair<ENUM_Key, CLASS_Data>> listDataFill = mapDataFill.ToList();
		for (int i = 0; i < _listInventorySlot.Count; i++)
		{
			if (i < listDataFill.Count)
			{
				ENUM_Key pEnumKey = listDataFill[i].Key;
				CLASS_Data pCurrentData = listDataFill[i].Value;

				if (mapInfoUnLock.ContainsKey(pEnumKey))
					OnSlot_Fill(_listInventorySlot[i], pCurrentData);
				else
					OnSlot_Fill_And_Lock(_listInventorySlot[i], pCurrentData);

				_mapInventoryData.Add(i, pCurrentData);
			}
			else
				OnSlot_Empty(_listInventorySlot[i]);
		}
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void EventOnClickForce(int iSlotIndex)
	{
		OnSlot_Click(iSlotIndex);
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	abstract protected EInventoryOption OnInitInventory();

	virtual protected void OnSlot_ClickIncludeData(int iSlotIndex, CLASS_Data pData, EInventorySlotState eSlotState) { }

	virtual protected void OnSlot_Click(int iSlotIndex)
	{
		_iSlotCurrentIndex = iSlotIndex;
		_pCurrentSelectData = EventGetSlotData(iSlotIndex);
		OnSlot_ClickIncludeData(iSlotIndex, _pCurrentSelectData, EventGetSlot(iSlotIndex).p_eSlotState);
	}

	virtual protected void OnSlot_Press(int iSlotIndex, bool bPress, CNGUIInventorySlot pSlot)
	{
		_iSlotCurrentIndex = iSlotIndex;
		if ((_eOption & EInventoryOption.MoveSlotItem) == EInventoryOption.MoveSlotItem)
		{
			if (bPress)
			{
				if (_mapInventoryData.ContainsKey(iSlotIndex) == false) return;

				_pCurrentSelectSlot = pSlot;
				_pCurrentSelectData = EventGetSlotData(iSlotIndex);
				pSlot.EventItem_ColliderOnOff(false);

				//DebugCustom.Log("Example", "Inventory", "Press On" + _pCurrentSelectData.IInventoryData_GetImageName( pSlot ), pSlot);
			}
			else
			{
				if (_pCurrentSelectData == null) return;

				pSlot.EventItem_ColliderOnOff(true);
				_mapInventoryData[_pCurrentSelectSlot.p_iSlotIndex] = _mapInventoryData[iSlotIndex];
				_mapInventoryData[iSlotIndex] = _pCurrentSelectData;

				OnSlot_Fill(pSlot, _pCurrentSelectData);
				OnSlot_Empty(_pCurrentSelectSlot);

				_pCurrentSelectData = null;
				_pCurrentSelectSlot = null;

				Debug.LogWarning( "Press Off" + pSlot.name, pSlot );
			}
		}
	}

	virtual protected void OnSlot_Fill(CNGUIInventorySlot pSlot, CLASS_Data pData)
	{
		//pSlot.DoSetItem(pData.IInventoryData_GetImageName( pSlot ) );
		pSlot.DoLockItem(false);
		if (_eOption != EInventoryOption.MoveSlotItem)
			pSlot.EventSlot_ColliderOnOff(true);
	}

	virtual protected void OnSlot_Fill_And_Lock(CNGUIInventorySlot pSlot, CLASS_Data pData)
	{
		//pSlot.DoSetItem(pData.IInventoryData_GetImageName( pSlot ) );
		pSlot.DoLockItem(true);
		if (_eOption != EInventoryOption.MoveSlotItem)
			pSlot.EventSlot_ColliderOnOff(true);
	}

	virtual protected void OnSlot_Empty(CNGUIInventorySlot pSlot)
	{
		pSlot.DoClearSlot(_strOnEmptySprite);
		if (_eOption != EInventoryOption.MoveSlotItem)
			pSlot.EventSlot_ColliderOnOff(false);
	}

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	protected CNGUIInventorySlot EventGetSlot(int iSlotIndex)
	{
		return _listInventorySlot[iSlotIndex];
	}

	protected CLASS_Data EventGetSlotData(int iSlotIndex)
	{
		if(_mapInventoryData.ContainsKey(iSlotIndex) == false)
		{
			Debug.LogWarning(iSlotIndex + "에 데이터가 없다!!");
			return null;
		}

		return _mapInventoryData[iSlotIndex];
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_eOption = OnInitInventory();

		GetComponentsInChildren(true, _listInventorySlot);
		//bool bAddClone = (_eOption & EInventoryOption.MoveSlotItem) == EInventoryOption.MoveSlotItem;
		for (int i = 0; i < _listInventorySlot.Count; i++)
			_listInventorySlot[i].p_EVENT_OnPress += OnSlot_Press;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
#endif