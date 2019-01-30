using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CCompoAddForce : CCompoEventTrigger
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	[Header( "AddForce 관련 옵션" )]
	public Vector3 _vecRandomForce_Min = new Vector3(-10f, -10f, 0f);
	public Vector3 _vecRandomForce_Max = new Vector3( 10f, 10f, 0f );

	public Vector3 _vecRandomForce_AbsoluteMin = new Vector3( 1f, 1f, 0f );

	[Header( "각속도 관련 옵션" )]
	public Vector3 _vecRandomAngulerForce_Min = new Vector3( 0f, 0f, -10f );
	public Vector3 _vecRandomAngulerForce_Max = new Vector3( 0f, 0f, 10f );

	public Vector3 _vecRandomAngulerForce_AbsoluteMin = new Vector3( 0f, 0f, 5f );

	[Header( "디버그용" )]
	[SerializeField]
	private bool _bIsReverseX = false;
	public Vector3 _vecRandoResult;
	public Vector3 _vecRandoResult_Angular;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Rigidbody _pRigidbody;
	private Rigidbody2D _pRigidbody2D;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetReverseX(bool bReverseX)
	{
		_bIsReverseX = bReverseX;
	}

	public void DoSetPhysics(bool bEnablePhysics)
	{
		if(_pRigidbody != null)
		{
			_pRigidbody.isKinematic = bEnablePhysics;
			_pRigidbody.detectCollisions = bEnablePhysics;
			_pRigidbody.velocity = Vector3.zero;
		}

		else if(_pRigidbody2D != null)
		{
			_pRigidbody2D.simulated = bEnablePhysics;
			_pRigidbody2D.velocity = Vector2.zero;
		}
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pRigidbody = GetComponentInChildren<Rigidbody>( true );
		_pRigidbody2D = GetComponentInChildren<Rigidbody2D>( true );
	}

	protected override void OnPlayEvent()
	{
		base.OnPlayEvent();

		DoSetPhysics( true );

		Vector3 vecRandomForce = PrimitiveHelper.RandomRange( _vecRandomForce_Min, _vecRandomForce_Max );
		if (vecRandomForce.x < 0f && vecRandomForce.x > -_vecRandomForce_AbsoluteMin.x)
			vecRandomForce.x = -_vecRandomForce_AbsoluteMin.x;
		else if(vecRandomForce.x > 0f && vecRandomForce.x < _vecRandomForce_AbsoluteMin.x)
			vecRandomForce.x = _vecRandomForce_AbsoluteMin.x;

		if (vecRandomForce.y < 0f && vecRandomForce.y > -_vecRandomForce_AbsoluteMin.y)
			vecRandomForce.y = -_vecRandomForce_AbsoluteMin.y;
		else if (vecRandomForce.y > 0f && vecRandomForce.y < _vecRandomForce_AbsoluteMin.y)
			vecRandomForce.y = _vecRandomForce_AbsoluteMin.y;

		if (vecRandomForce.z < 0f && vecRandomForce.z > -_vecRandomForce_AbsoluteMin.z)
			vecRandomForce.z = -_vecRandomForce_AbsoluteMin.z;
		else if (vecRandomForce.z > 0f && vecRandomForce.z < _vecRandomForce_AbsoluteMin.z)
			vecRandomForce.z = _vecRandomForce_AbsoluteMin.z;

		if (_bIsReverseX)
			vecRandomForce = new Vector3( -vecRandomForce.x, vecRandomForce.y, vecRandomForce.z);
		
		_vecRandoResult = vecRandomForce;
		if (_pRigidbody != null)
			_pRigidbody.AddForce( vecRandomForce );
		else if(_pRigidbody2D != null)
			_pRigidbody2D.AddForce( vecRandomForce, ForceMode2D.Impulse );

		Vector3 vecRandomForce_Anguler = PrimitiveHelper.RandomRange( _vecRandomAngulerForce_Min, _vecRandomAngulerForce_Max );
		if (vecRandomForce_Anguler.x < 0f && vecRandomForce_Anguler.x > -_vecRandomForce_AbsoluteMin.x)
			vecRandomForce_Anguler.x = -_vecRandomForce_AbsoluteMin.x;
		else if (vecRandomForce_Anguler.x > 0f && vecRandomForce_Anguler.x < _vecRandomForce_AbsoluteMin.x)
			vecRandomForce_Anguler.x = _vecRandomForce_AbsoluteMin.x;

		if (vecRandomForce_Anguler.y < 0f && vecRandomForce_Anguler.y > -_vecRandomForce_AbsoluteMin.y)
			vecRandomForce_Anguler.y = -_vecRandomForce_AbsoluteMin.y;
		else if (vecRandomForce_Anguler.y > 0f && vecRandomForce_Anguler.y < _vecRandomForce_AbsoluteMin.y)
			vecRandomForce_Anguler.y = _vecRandomForce_AbsoluteMin.y;

		if (vecRandomForce_Anguler.z < 0f && vecRandomForce_Anguler.z > -_vecRandomForce_AbsoluteMin.z)
			vecRandomForce_Anguler.z = -_vecRandomForce_AbsoluteMin.z;
		else if (vecRandomForce_Anguler.z > 0f && vecRandomForce_Anguler.z < _vecRandomForce_AbsoluteMin.z)
			vecRandomForce_Anguler.z = _vecRandomForce_AbsoluteMin.z;

		_vecRandoResult_Angular = vecRandomForce_Anguler;
		if (_pRigidbody != null)
			_pRigidbody.angularVelocity = vecRandomForce_Anguler;
		else if (_pRigidbody2D != null)
			_pRigidbody2D.angularVelocity = vecRandomForce_Anguler.z;
		else
			StartCoroutine( CoRotateTransform( vecRandomForce_Anguler ) );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private IEnumerator CoRotateTransform(Vector3 vecAngularVelocity)
	{
		while(true)
		{
			transform.Rotate( vecAngularVelocity );

			yield return YieldManager.GetWaitForSecond(0.02f);
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
