#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
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

//public class CUGUIInventoryPageBase<CLASS_DATA> : CUGUIInventoryBase<CLASS_DATA>
//	where CLASS_DATA : CInventoryData_Test
//{
//	/* const & readonly declaration             */

//	/* enum & struct declaration                */

//	#region Field

//	/* public - Field declaration            */

//	/* protected - Field declaration         */

//	/* private - Field declaration           */

//	private int _iMaxPage;
//	private int _iCurPage;

//	#endregion Field

//	#region Public

//	// ========================================================================== //

//	/* public - [Do] Function
//     * 외부 객체가 호출(For External class call)*/

//	public void DoInitInventoryPage(Dictionary<int, CLASS_DATA> mapItemData, bool bAutoLimitPage = false)
//	{
//		Dictionary<int, CLASS_DATA> mapItemDataPage = new Dictionary<int, CLASS_DATA>();

//		int i = 0;
//		var pIter = mapItemData.GetEnumerator();
//		while (pIter.MoveNext())
//		{
//			var pCurrent = pIter.Current;

//			int iRealID = pCurrent.Key;
//			CLASS_DATA pData = pCurrent.Value;

//			int iCurPageCountSlot = _iCurPage * _iMaxSlot;
//			int iPageItemCount = (_iCurPage - 1) * _iMaxSlot;

//			if (iCurPageCountSlot > i && i >= iPageItemCount)
//				mapItemDataPage.Add(iRealID, pData);

//			i++;
//		}

//		DoInitInventory(mapItemDataPage);
//	}

//	public void DoPrevPage()
//	{
//		EventPrevPage();
//	}

//	public void DoNextPage()
//	{
//		EventNextPage();
//	}

//	public void DoAddMaxPage(int iAddMaxPage)
//	{
//		EventAddMaxPage(iAddMaxPage);
//	}

//	public void DoSetPage(int iPage)
//	{
//		EventSetPage(iPage);
//	}

//	public void DoSetPage_Force(int iPage)
//	{
//		EventSetPage_Force(iPage);
//	}

//	/* public - [Event] Function             
//       프랜드 객체가 호출(For Friend class call)*/

//	protected void EventSetMaxPage(int iMaxPage)
//	{
//		_iMaxPage = iMaxPage;
//	}

//	protected void EventAddMaxPage(int iAddMaxPage)
//	{
//		_iMaxPage += iAddMaxPage;
//	}

//	protected void EventPrevPage()
//	{
//		_iCurPage--;

//		EventSetPage(GetPageClamped(_iCurPage));
//	}

//	protected void EventNextPage()
//	{
//		_iCurPage++;
//		EventSetPage(GetPageClamped(_iCurPage));
//	}

//	protected void EventSetPage_Force(int iPage)
//	{
//		ProcSetPage(iPage);
//		OnSetPage(iPage, _iMaxPage);
//	}

//	protected void EventSetPage(int iPage)
//	{
//		ProcSetPage(iPage);
//		OnSetPage(_iCurPage, _iMaxPage);
//	}

//	#endregion Public

//	// ========================================================================== //

//	#region Protected

//	/* protected - [abstract & virtual]         */

//	protected virtual void OnSetPage(int iPage, int iMaxPage) { }

//	/* protected - [Event] Function           
//       자식 객체가 호출(For Child class call)		*/

//	/* protected - Override & Unity API         */

//	#endregion Protected

//	// ========================================================================== //

//	#region Private

//	/* private - [Proc] Function             
//       로직을 처리(Process Local logic)           */

//	private int GetPageClamped(int iPage)
//	{
//		return Mathf.Clamp(iPage, 1, _iMaxPage);
//	}

//	private void ProcSetPage(int iPage)
//	{
//		_iCurPage = iPage;
//	}

//	/* private - Other[Find, Calculate] Func 
//       찾기, 계산등 단순 로직(Simpe logic)         */

//	#endregion Private
//}
