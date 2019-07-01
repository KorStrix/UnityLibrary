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
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent( typeof( CCompoEventSystemChecker ) )]
[RequireComponent( typeof( Canvas ) )]
[RequireComponent( typeof( CanvasScaler ) )]
[RequireComponent( typeof( GraphicRaycaster ) )]
abstract public class CManagerUGUIBase<Class_Instance, Enum_Panel_Name> : CManagerUIBase<Class_Instance, Enum_Panel_Name, CUGUIPanelBase, Button>
	where Class_Instance : CManagerUGUIBase<Class_Instance, Enum_Panel_Name>
	where Enum_Panel_Name : System.IFormattable, System.IConvertible, System.IComparable
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

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

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