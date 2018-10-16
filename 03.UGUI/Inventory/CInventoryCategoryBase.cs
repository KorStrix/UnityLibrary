#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : KJH
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CInventoryCategoryBase<ENUM_CATEGORY, CLASS_DATA, CLASS_SLOT> : CInventoryBase<CLASS_DATA, CLASS_SLOT>
	where ENUM_CATEGORY : System.IFormattable, System.IConvertible, System.IComparable
	where CLASS_DATA : class, IInventoryData<CLASS_DATA>
	where CLASS_SLOT : CObjectBase, IInventorySlot<CLASS_SLOT, CLASS_DATA>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private ENUM_CATEGORY _eCurrentCategory; public ENUM_CATEGORY p_eCurrentCategory { get { return _eCurrentCategory; } }

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void OnSetTab(ENUM_CATEGORY eTab)
	{
		_eCurrentCategory = eTab;

		DoRefresh_InventoryData();
	}

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	protected abstract void OnSetInventoryCategory(ENUM_CATEGORY eCategory, CLASS_DATA sInfoData, CLASS_SLOT pSlot);

	protected abstract List<CLASS_DATA> GetInventoryData_Category();

	protected abstract ENUM_CATEGORY GetDefaultCategory();

	protected abstract bool GetEquals_InventoryDataCategory(ENUM_CATEGORY eCategory, CLASS_DATA sInfoData);


	protected virtual void OnSetInventoryCategory(ENUM_CATEGORY eCategory) { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override List<CLASS_DATA> GetInventoryData()
	{
		List<CLASS_DATA> listInventoryDataTab = new List<CLASS_DATA>();
		List<CLASS_DATA> listInventoryData = GetInventoryData_Category();

		int iCount = listInventoryData.Count;
		for (int i = 0; i < iCount; i++)
		{
			CLASS_DATA sInfoData = listInventoryData[i];

			if (GetEquals_InventoryDataCategory(_eCurrentCategory, sInfoData))
				listInventoryDataTab.Add(sInfoData);
		}

		OnSetInventoryCategory(_eCurrentCategory);

		return listInventoryDataTab;
	}

    protected override void OnInventory_SetData(CLASS_SLOT pSlot, CLASS_DATA sInfoData)
	{
        base.OnInventory_SetData(pSlot, sInfoData);

		OnSetInventoryCategory(_eCurrentCategory, sInfoData, pSlot);
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		_eCurrentCategory = GetDefaultCategory();
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
