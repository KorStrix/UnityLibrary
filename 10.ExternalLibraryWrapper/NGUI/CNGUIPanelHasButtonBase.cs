#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header
#if NGUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class CNGUIPanelHasButtonBase<Enum_ButtonName> : CNGUIPanelBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	abstract public void OnClick_Buttons( Enum_ButtonName eButtonName );

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

		UIButton[] arrButton = GetComponentsInChildren<UIButton>();
		for(int i = 0; i < arrButton.Length; i++)
		{
			UIButton pButton = arrButton[i];
			Enum_ButtonName eButtonName;
			if (pButton.name.ConvertEnum_IgnoreError( out eButtonName))
				EventDelegate.Add( pButton.onClick, new EventDelegate( this, "OnClick_Buttons", eButtonName.GetHashCode() ) );
		}
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
   #endif