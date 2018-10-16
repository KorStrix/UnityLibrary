#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
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
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class CUGUIInventorySlot<Class_Slot, Class_Data> : CUGUIObjectBase, IInventorySlot<Class_Slot, Class_Data>, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    where Class_Slot : CUGUIInventorySlot<Class_Slot, Class_Data>
	where Class_Data : IInventoryData<Class_Data>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private IInventory<Class_Data, Class_Slot> _pInventory;	public IInventory<Class_Data, Class_Slot> p_pIventoryOwner {  get { return _pInventory; } }
	private Class_Slot _pSlot;
	private Class_Data _pData;

	public Class_Data p_pInventoryData
	{
		get
		{
			return _pData;
		}
	}

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void IInventorySlot_DoInit( IInventory<Class_Data, Class_Slot> pInventoryOwner )
	{
		EventOnAwake();

		_pInventory = pInventoryOwner;
	}

	public void IInventorySlot_OnSetData( Class_Data pData, string strImageName )
	{
		if(OnSetDataOrNull_And_CheckIsValidData( pData ))
			_pData = pData;
	}

    #endregion Public

    // ========================================================================== //

    #region Protected

    /* protected - [abstract & virtual]         */

    public virtual void IInventorySlot_OnFillSlot( bool bIsFillData ) { }
	public virtual void IInventorySlot_OnClickSlot( bool bIsCurrentSelectedSlot ) { }
    public virtual void IInventorySlot_OnPressSlot(bool bIsCurrentSelectedSlot, bool bPressDown) { }
    protected virtual bool OnSetDataOrNull_And_CheckIsValidData( Class_Data pData ) { return true; }

	protected override void OnAwake()
	{
		base.OnAwake();

		_pSlot = this as Class_Slot;
	}

    protected override void OnUIClick()
    {
        base.OnUIClick();

        _pInventory.IInventory_OnClickSlot(_pSlot);
    }

    protected override void OnUIPress(bool bPress)
    {
        base.OnUIPress(bPress);

        _pInventory.IInventory_OnPressSlot(_pSlot, bPress);
    }

    /* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

    #endregion Protected

    // ========================================================================== //

    #region Private

    /* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

    #endregion Private
}
