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

public class CCompoRandomSprite : CCompoEventTrigger
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	[SerializeField]
	private List<Sprite> _listRandomSprite = new List<Sprite>();

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

	protected override void OnPlayEvent()
	{
		base.OnPlayEvent();

		Sprite pSprite = _listRandomSprite.GetRandom();
		UnityEngine.UI.Image pImage = GetComponent<UnityEngine.UI.Image>();
		if (pImage != null)
			pImage.sprite = pSprite;

		SpriteRenderer pSpriteRenderer = GetComponent<SpriteRenderer>();
		if(pSprite != null)
			pSpriteRenderer.sprite = pSprite;
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
