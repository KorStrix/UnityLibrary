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
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public interface IUGUIScrollItemData
{
	void IScrollData_Update( CUGUIScrollItem pScrollItem );
	int IScrollData_GetSortOrder();
}

public class CUGUIScrollItem : CUIObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private IUGUIScrollItemData _pScrollData;

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void EventSetScrollData( IUGUIScrollItemData pScrollData)
	{
		_pScrollData = pScrollData;
		_pScrollData.IScrollData_Update(this);
	}

	public void EventInitScrollItem<Enum_ButtonName>( System.Action<CUGUIScrollItem, IUGUIScrollItemData, Enum_ButtonName> OnClick_ScrollItemButton)
	{
		Button[] arrButton = GetComponentsInChildren<Button>();
		for (int i = 0; i < arrButton.Length; i++)
		{
			Button pButton = arrButton[i];
			pButton.onClick.RemoveAllListeners();

			Enum_ButtonName eButtonName;
			if (pButton.name.ConvertEnum( out eButtonName ))
				pButton.onClick.AddListener( delegate { OnClick_ScrollItemButton( this, _pScrollData, eButtonName ); } );
		}
	}

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
