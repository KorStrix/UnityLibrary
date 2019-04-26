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

public class CCompoRandomPos : CCompoEventTrigger
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field
	/* public - Field declaration            */

	public Vector3 _vecRandom_Min = new Vector3(-10f, -10f, 0f);
	public Vector3 _vecRandom_Max = new Vector3(10f, 10f, 0f);

	/* protected - Field declaration         */

	/* private - Field declaration           */

	[Header("디버깅용")]
    [Rename_Inspector("랜덤 결과", false)]
	[SerializeField]
	private Vector3 _vecPos = Vector3.zero;

    [GetComponent(bIsPrint_OnNotFound = false)]
    RectTransform _pRectTransform = null;

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

    protected override void OnPlayEvent()
	{
		base.OnPlayEvent();

		_vecPos = PrimitiveHelper.RandomRange(_vecRandom_Min, _vecRandom_Max);

        if (_pRectTransform)
            _pRectTransform.localPosition = _vecPos;
        else
            transform.localPosition = _vecPos;
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
