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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CUIObjectBase : CObjectBase, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, /*IDragHandler,*/ IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    #region Field

    /* public - Field declaration            */

    public bool bPrintDebug = false;

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

    // ========================== [ Division ] ========================== //

    virtual protected void OnUIClick()
    {
        if (bPrintDebug)
            Debug.Log("OnUIClick");
    }

    virtual protected void OnUIPress( bool bPress )
    {
        if (bPrintDebug)
            Debug.Log("OnUIPress bPress : " + bPress);
    }

    virtual protected void OnUIHover( bool bHover )
    {
        if (bPrintDebug)
            Debug.Log("OnUIHover bHover : " + bHover);
    }

    virtual protected void OnUIDrag( bool bIsDrag )
    {
        if (bPrintDebug)
            Debug.Log("OnUIDrag bIsDrag : " + bIsDrag);
    }

    virtual protected void OnUIDrop( GameObject pObjectOnDrop )
    {
        if (bPrintDebug)
            Debug.Log("OnUIDrop pObjectOnDrop : " + pObjectOnDrop.name);
    }

    /* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

    /* protected - Override & Unity API         */

    // For NGUI
    public void OnClick() { OnUIClick(); }
	public void OnPress( bool bPress ) { OnUIPress( bPress ); }
	public void OnDragStart() { OnUIDrag( true ); }
	public void OnDragEnd() { OnUIDrag( false ); }
	public void OnDrop( GameObject pObjectDrop ){ OnUIDrop( pObjectDrop ); }
	public void OnDrag( PointerEventData eventData ) { OnUIDrag( true ); }
	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	public void OnPointerClick( PointerEventData eventData )
	{
		OnClick();
	}

	public void OnPointerDown( PointerEventData eventData )
	{
		OnPress( true );
	}

	public void OnPointerUp( PointerEventData eventData )
	{
		OnPress( false );
	}

	public void OnDrag( GameObject pObject )
	{
		OnUIDrag( true );
	}

	public void OnEndDrag( PointerEventData eventData )
	{
		OnUIDrag( false );
	}

	public void OnDrop( PointerEventData eventData )
	{
		OnUIDrop( eventData.pointerDrag );
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnUIHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnUIHover(false);
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

    #endregion Private
}
