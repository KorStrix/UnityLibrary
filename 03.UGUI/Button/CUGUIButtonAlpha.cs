﻿#region Header
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
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CUGUIButtonAlpha : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
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
