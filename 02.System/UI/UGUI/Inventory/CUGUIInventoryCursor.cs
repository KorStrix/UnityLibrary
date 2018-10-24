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
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CUGUIInventoryCursor : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private Camera _pCamera;
	private Canvas _pRootCanvas;
	private RectTransform _pRecTransRootCanvas;

	private Image _pImage;

	// private Vector3 _vecInitPos;

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoUpdatePosition(Vector3 v3CursorPos)
	{
		Vector2 v2ConvertPos = Vector3.zero;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(_pRecTransRootCanvas, v3CursorPos, _pCamera, out v2ConvertPos);

        transform.position = _pRecTransRootCanvas.TransformPoint(v2ConvertPos);
		gameObject.SetActive(true);
	}

	public void DoSetImage(Sprite pSprite)
	{
		EventSetImage(pSprite);
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	private void EventSetImage(Sprite pSprite)
	{
		_pImage.sprite = pSprite;
		_pImage.SetNativeSize();
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

        this.GetComponentInChildren(out _pImage);
		_pImage.raycastTarget = false;

		_pRootCanvas = GetComponentInParent<Canvas>();
		_pRecTransRootCanvas = (RectTransform)_pRootCanvas.transform;
		_pCamera = _pRootCanvas.worldCamera;

		gameObject.SetActive(false);
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
