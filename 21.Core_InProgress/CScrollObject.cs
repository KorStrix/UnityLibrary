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
using System.Collections;
using System.Collections.Generic;

public class CScrollObject : CObjectBase, IRandomItem
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	[Range(0, 100)][Header("랜덤확률")]
	public int p_iPercent = 50;

	public float p_fWidth = 0f;

	public string p_strName
	{
		get
		{
			if(_bIsRequireInit )
			{
				_bIsRequireInit = false;
				_strName = name;
			}

			return _strName;
		}
	}

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private string _strName;
	private bool _bIsRequireInit = true;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public int IRandomItem_GetPercent()
	{
		return p_iPercent;
	}

	public string IDictionaryItem_GetKey()
	{
		return p_strName;
	}

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
