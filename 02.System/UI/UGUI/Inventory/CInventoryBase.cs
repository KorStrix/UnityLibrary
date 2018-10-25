#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public interface IInventory<Class_Data, Class_Slot>
	where Class_Data : IInventoryData<Class_Data>
	where Class_Slot : IInventorySlot<Class_Slot, Class_Data>
{
    void IInventory_OnClickSlot(Class_Slot pInventorySlot);
    void IInventory_OnPressSlot(Class_Slot pInventorySlot, bool bPressDown);

    void IInventory_RefreshData( );
}

public interface IInventorySlot<Class_Slot, Class_Data>
	where Class_Slot : IInventorySlot<Class_Slot, Class_Data>
	where Class_Data : IInventoryData<Class_Data>
{
	Class_Data p_pInventoryData { get; }

	void IInventorySlot_DoInit( IInventory<Class_Data, Class_Slot> pInventoryOwner );

	void IInventorySlot_OnSetData( Class_Data pData, string strImageName );
	void IInventorySlot_OnFillSlot( bool bEnable );
	void IInventorySlot_OnClickSlot( bool bIsCurrentSelectedSlot );
    void IInventorySlot_OnPressSlot(bool bIsCurrentSelectedSlot, bool bPressDown);
}

public interface IInventoryData<Class_Data>
	where Class_Data : IInventoryData<Class_Data>
{
	string IInventoryData_GetImageName( );
}

public abstract class CInventoryBase<CLASS_Data, CLASS_Slot> : CSingletonMonoBase<CInventoryBase<CLASS_Data, CLASS_Slot>>, IInventory<CLASS_Data, CLASS_Slot>
	where CLASS_Data : class, IInventoryData<CLASS_Data>
	where CLASS_Slot : CObjectBase, IInventorySlot<CLASS_Slot, CLASS_Data>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EInventoryType
	{
		None,
		Scroll,
		Page,
		Page_Auto
	}

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	protected Dictionary<int, CLASS_Slot> _mapInventorySlot = new Dictionary<int, CLASS_Slot>();
	protected List<CLASS_Slot> _listInventorySlot = new List<CLASS_Slot>();

	/* private - Field declaration           */

	private int _iMaxPage;	public int p_iMaxPage {  get { return _iMaxPage; } }
	private int _iCurPage = 1;  public int p_iCurPage { get { return _iCurPage; } }

	private int _iMaxSlotCount;
	private int _iTotalDataCount;

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoRefresh_InventoryData() { IInventory_RefreshData(); }
	public void DoPrevPage()	{ EventPrevPage(); }
	public void DoNextPage() { EventNextPage(); }
	public void DoSetMaxPage(int iMaxPage) { EventSetMaxPage(iMaxPage); }
	public void DoAddMaxPage( int iAddMaxPage ) { EventAddMaxPage( iAddMaxPage ); }
	public void DoSetPage( int iPage ) { EventSetPage( iPage ); }

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void IInventory_OnClickSlot( CLASS_Slot pInventorySlot )
	{
		if(pInventorySlot != null)
		{
			CLASS_Data pData = pInventorySlot.p_pInventoryData;
			OnInventory_ClickSlot( pInventorySlot, pData );
		}

		_mapInventorySlot.Values.ToList( _listInventorySlot );
		for (int i = 0; i < _listInventorySlot.Count; i++)
			_listInventorySlot[i].IInventorySlot_OnClickSlot( _listInventorySlot[i] == pInventorySlot );
	}

    public void IInventory_OnPressSlot(CLASS_Slot pInventorySlot, bool bPressDown)
    {
        if (pInventorySlot != null)
        {
            CLASS_Data pData = pInventorySlot.p_pInventoryData;
            OnInventory_PressSlot(pInventorySlot, pData, bPressDown);
        }

        _mapInventorySlot.Values.ToList(_listInventorySlot);
        for (int i = 0; i < _listInventorySlot.Count; i++)
            _listInventorySlot[i].IInventorySlot_OnPressSlot(_listInventorySlot[i] == pInventorySlot, bPressDown);
    }

    public void IInventory_RefreshData()
	{
		_iTotalDataCount = GetInventoryData().Count;

		EInventoryType eInventoryType = GetInventoryType();
		switch (eInventoryType)
		{
			case EInventoryType.Scroll:
				ProcInit_InventoryData( GetInventoryData() );
				break;

			case EInventoryType.Page:
			case EInventoryType.Page_Auto:
				ProcInit_InventoryData_Page( _iCurPage, eInventoryType == EInventoryType.Page_Auto );
				break;
		}

		// IInventory_OnClickSlot( null );
		OnInventory_RefreshInventoryData( GetInventoryData() );
	}

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	abstract protected List<CLASS_Data> GetInventoryData();
	abstract protected EInventoryType GetInventoryType();

	virtual protected void OnInventory_ClickSlot( CLASS_Slot pInventorySlot, CLASS_Data pData ) { }
    virtual protected void OnInventory_PressSlot(CLASS_Slot pInventorySlot, CLASS_Data pData, bool bPressDown) { }
    virtual protected void OnInventory_SetData( CLASS_Slot pSlot, CLASS_Data pData ) { }
	virtual protected void OnInventory_SetPage( int iCurPage, int iMaxPage ) { }
	virtual protected void OnInventory_RefreshInventoryData( List<CLASS_Data> listData ) { }
	
	protected void EventSetMaxPage( int iMaxPage )
	{
		_iMaxPage = iMaxPage;
	}

	protected void EventAddMaxPage( int iAddMaxPage )
	{
		_iMaxPage += iAddMaxPage;
	}

	protected void EventPrevPage()
	{
		_iCurPage--;

		EventSetPage( GetPageClamped( _iCurPage ) );
	}

	protected void EventNextPage()
	{
		_iCurPage++;

		EventSetPage( GetPageClamped( _iCurPage ) );
	}

	protected void EventSetPage( int iPage )
	{
		ProcInit_InventoryData_Page( iPage );
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		ProcInit_InventorySlot();
	}

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void ProcInit_Page_Auto()
	{
		EventSetMaxPage( 1 );

		if(_iMaxSlotCount == 0)
		{
			Debug.LogWarning( name + "Error InventoryBase _iMaxSlotCount == 0", this );
			return;
		}

		int i = 1;
		while ((_iTotalDataCount - 1) >= i)
		{
			int iPercent = i % _iMaxSlotCount;
			if (iPercent == 0)
			{
				EventAddMaxPage( 1 );
			}

			i++;
		}
	}

	private void ProcInit_InventorySlot()
	{
		IInventory<CLASS_Data, CLASS_Slot> pInventory = this;
		CLASS_Slot[] arrInventoryslot = GetComponentsInChildren<CLASS_Slot>( true );
		int iLen = arrInventoryslot.Length;
		for (int i = 0; i < iLen; i++)
		{
			CLASS_Slot pSlot = arrInventoryslot[i];
			if (_mapInventorySlot.ContainsKey( i ))
			{
				Debug.LogWarning( "이미 _mapInventorySlot 에 " + pSlot + " 가 있습니다..." );
			}
			else
			{
				_mapInventorySlot.Add( i, pSlot );

				pSlot.EventOnAwake();
				pSlot.IInventorySlot_DoInit( pInventory );
				pSlot.IInventorySlot_OnFillSlot( false );
			}
		}

		_iMaxSlotCount = iLen;
	}

	private void ProcInit_InventoryData( List<CLASS_Data> listInventoryData )
	{
		int iCount_InventoryData = listInventoryData.Count;
		for (int i = 0; i < _mapInventorySlot.Count; i++)
		{
			CLASS_Data sInfoData = null;
			CLASS_Slot pSlot = _mapInventorySlot[i];
			bool bContains_InventoryData = i < iCount_InventoryData;
			pSlot.IInventorySlot_OnFillSlot( bContains_InventoryData );
			if (bContains_InventoryData)
			{
				sInfoData = listInventoryData[i];
				pSlot.IInventorySlot_OnSetData(sInfoData, sInfoData.IInventoryData_GetImageName());
				OnInventory_SetData(pSlot, sInfoData);
			}
			else
			{
				pSlot.IInventorySlot_OnSetData(null, null);
			}
		}
	}

    List<CLASS_Data> _listNote = new List<CLASS_Data>();
    private void ProcInit_InventoryData_Page( int iPage, bool bPageAuto = false )
	{
		int iPrev_PageSlotCount = Mathf.Max( 0, iPage - 1 ) * _iMaxSlotCount;
		int iNext_PageSlotCount = iPage * _iMaxSlotCount;

		List<CLASS_Data> listInventoryData = GetInventoryData();
        List<CLASS_Data> listInventoryData_Page = _listNote;
        listInventoryData_Page.Clear();

        for (int i = iPrev_PageSlotCount; i <= iNext_PageSlotCount; i++)
		{
			if (i < _iTotalDataCount)
			{
				CLASS_Data sInfoData = listInventoryData[i];
				listInventoryData_Page.Add( sInfoData );
			}
			else
				break;
		}

		ProcInit_InventoryData( listInventoryData_Page );
        OnInventory_SetPage( iPage, _iMaxPage );
		_iCurPage = iPage;

        if (bPageAuto)
            ProcInit_Page_Auto();
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

    private int GetPageClamped( int iPage )
	{
		return Mathf.Clamp( iPage, 1, _iMaxPage );
	}

    #endregion Private
}
