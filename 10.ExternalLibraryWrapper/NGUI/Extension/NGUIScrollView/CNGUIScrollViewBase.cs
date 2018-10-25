#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : Strix, KJH
 *	작성자 : KJH
 *	
 *	기능 : 재사용 스크룔뷰를 쉽게 쓸수있는 클래스, 상속을 받고 써야한다.
   ============================================ */
#endregion Header
#if NGUI

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 제네릭 클래스로는 컴포넌트 찾기가 불가능
// 리플렉션을 쓰면 가능하긴하지만 그냥 인터페이스로 찾고 호출
interface IOnClickItemListener
{
	void IOnClick_Item(int iRealID, string strButtonName);
}

[RequireComponent(typeof(UIScrollView))]
[RequireComponent(typeof(UIWrapContent))]
public class CNGUIScrollViewBase<CLASS_Data, ENUM_Button> : CObjectBase, IOnClickItemListener
	where CLASS_Data : System.IComparable<CLASS_Data>
	where ENUM_Button : struct, System.IComparable, System.IConvertible
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field
	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private List<CLASS_Data> _listRealItemData = new List<CLASS_Data>();
	private List<CNGUIScrollViewItem> _listScrollViewItem = new List<CNGUIScrollViewItem>();
	private Dictionary<int, CNGUIScrollViewItem> _mapScrollViewItem = new Dictionary<int, CNGUIScrollViewItem>();
	private Dictionary<CLASS_Data, int> _mapData_To_RealID = new Dictionary<CLASS_Data, int>();

	private UIPanel _pPanel;
	private UIScrollView _pUIScrollView;
	private UIWrapContent _pUIWrapContent;

	private int _iCountScrollItem;
	private int _iCountRealItem;

	private Vector2 _v2SpringFirstPos;
	// private Vector2 _v2LastPosition;

	#endregion Field
	#region Public
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoInit(List<CLASS_Data> listData)
	{
		_listRealItemData = listData;
		_iCountRealItem = _listRealItemData.Count;

		EventResetPosition();
		EventSetSpringPosition(_v2SpringFirstPos);
	}

	public void DoAddItem(CLASS_Data sData)
	{
		if (_listRealItemData.Contains_PrintOnError(sData, true)) return;

		_listRealItemData.Add(sData);
		_iCountRealItem = _listRealItemData.Count;

		EventResetPosition();
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void IOnClick_Item(int iRealID, string strButtonName)
	{
		CLASS_Data pData = _listRealItemData[iRealID];
		ENUM_Button eButton = strButtonName.ConvertEnum<ENUM_Button>();

		OnClick_Item(pData, eButton);
	}

	#endregion Public
	// ========================================================================== //
	#region Protected
	/* protected - [abstract & virtual]         */

	protected void EventRemoveItem(CLASS_Data sData)
	{
		if (_listRealItemData.Contains_PrintOnError(sData) == false) return;

		int iRealID_RemoveItem = _mapData_To_RealID[sData];
		_mapData_To_RealID.Remove(sData);
		_listRealItemData.Remove(sData);

		_iCountRealItem = _listRealItemData.Count;
		_mapScrollViewItem[iRealID_RemoveItem].SetActive(false);

		EventResetPosition();
	}

	protected void EventResetPosition()
	{
		for (int i = 0; i < _iCountScrollItem; i++)
			_listScrollViewItem[i].SetActive(i < _iCountRealItem);

		int iMaxItem = Mathf.Max(1, _iCountRealItem - 1);
		_pUIWrapContent.minIndex = -iMaxItem;
		_pUIWrapContent.maxIndex = 0;

		_pUIWrapContent.SortAlphabetically();
		_pUIWrapContent.WrapContent();

		if (_iCountRealItem < _iCountScrollItem)
		{
			_pUIScrollView.ResetPosition();
			_pUIScrollView.RestrictWithinBounds(true);
		}

		ProcCheckDisableDrag();
	}

	protected void EventSetSpringPosition(Vector2 v2SpringPos, float fStrength = 8f)
	{
		SpringPanel.Begin(_pGameObjectCached, v2SpringPos, fStrength);
	}

	protected virtual void OnClick_Item(CLASS_Data sData, ENUM_Button eButton) { }
	protected virtual void OnSet_Item(CLASS_Data sData, CNGUIScrollViewItem pItem) { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	private void OnSetItem(GameObject pObj, int iWrapID, int iRealID)
	{
		if (_iCountRealItem == 0) return;

		iRealID = Mathf.Abs(iRealID);
		CNGUIScrollViewItem pItem = _listScrollViewItem[iWrapID];

		if (_listRealItemData.Count <= iRealID)
			iRealID = _listRealItemData.Count - 1;

		CLASS_Data pData = _listRealItemData[iRealID];

		pItem.DoSetRealID(iRealID);
		_mapScrollViewItem[iRealID] = pItem;
		_mapData_To_RealID[pData] = iRealID;

		OnSet_Item(pData, pItem);
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pPanel = GetComponent<UIPanel>();
		_pUIScrollView = GetComponent<UIScrollView>();

		_pUIWrapContent = GetComponentInChildren<UIWrapContent>();
		_pUIWrapContent.onInitializeItem = OnSetItem;

		GetComponentsInChildren<CNGUIScrollViewItem>(_listScrollViewItem);

		_iCountScrollItem = _listScrollViewItem.Count;
		_v2SpringFirstPos = _pPanel.transform.localPosition;
	}

	#endregion Protected
	// ========================================================================== //
	#region Private
	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void ProcCheckDisableDrag()
	{
		_pUIScrollView.disableDragIfFits = _iCountRealItem < _iCountScrollItem;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */
	#endregion Private
}
   #endif