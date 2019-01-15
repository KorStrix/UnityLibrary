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
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class SCManagerIndicator : CSingletonNotMonoBase<SCManagerIndicator>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIObject
	{
		Indicator_Nanum_InGame,
		Indicator_Nanum_UI
	}

	public enum EInidicatorType
	{
		World,
		UI, // UI의 경우 빌드 후 CanvasScaler에 의해 모양이 이상해 질수 있다.
	}

	/* public - Field declaration            */

		/* protected - Field declaration         */

	/* private - Field declaration           */
	
	static protected Camera _pCamera_InGame;

	static protected RectTransform _pRectTransform_CanvasScaler;

	static protected EInidicatorType _eType;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/
	 
	static public void DoInitIndicator()
	{
		CManagerPooling_InResources<EUIObject, CIndicator>.instance.p_EVENT_OnMakeResource += Instance_p_EVENT_OnMakeResource;
	}

	static public void DoStartPooling( int iPoolingCount )
	{
		CManagerPooling_InResources<EUIObject, CIndicator>.instance.DoStartPooling( iPoolingCount );
	}

	private static void Instance_p_EVENT_OnMakeResource( EUIObject arg1, CIndicator arg2 )
	{
		arg2.p_Event_OnDisable.AddListener( OnPopIndciator );
	}

	static public CIndicator DoPop( string strText, EUIObject eUIObject = EUIObject.Indicator_Nanum_InGame )
	{
		CIndicator pIndicator = CManagerPooling_InResources<EUIObject, CIndicator>.instance.DoPop( eUIObject );
		pIndicator.DoSetText( strText );

		return pIndicator;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

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

	static private void OnPopIndciator( CIndicator pIndicator )
	{
		CManagerPooling_InResources<EUIObject, CIndicator>.instance.DoPush( pIndicator );
	}

	static private Vector3 ProcConvertPosition_World_To_UI( Transform pTransform, Vector3 vecPos )
	{
		Vector2 vecWorldCamPos = _pCamera_InGame.WorldToViewportPoint( vecPos );

		if (_eType == EInidicatorType.UI)
		{
			vecWorldCamPos.x *= _pRectTransform_CanvasScaler.sizeDelta.x;
			vecWorldCamPos.y *= _pRectTransform_CanvasScaler.sizeDelta.y;

			vecWorldCamPos.x -= _pRectTransform_CanvasScaler.sizeDelta.x * _pRectTransform_CanvasScaler.pivot.x;
			vecWorldCamPos.y -= _pRectTransform_CanvasScaler.sizeDelta.y * _pRectTransform_CanvasScaler.pivot.y;
		}

		pTransform.position = vecWorldCamPos;
		return pTransform.position;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
