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

public class SCManagerYield
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field
	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	static private Dictionary<float, WaitForSeconds> _mapYieldSeconds = new Dictionary<float, WaitForSeconds>();

	#endregion Field
	#region Public
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	static public WaitForSeconds GetWaitForSecond(float fSec)
	{
		if (_mapYieldSeconds.ContainsKey(fSec) == false)
			_mapYieldSeconds.Add(fSec, new WaitForSeconds(fSec));

		return _mapYieldSeconds[fSec];
	}

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
