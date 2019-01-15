using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CCompoMovePos : CCompoEventTrigger
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public Space _pSpaceSimulate = Space.Self;
	public Vector3 _vecRandomForce_Min = new Vector3( -10f, -10f, 0f );
	public Vector3 _vecRandomForce_Max = new Vector3( 10f, 10f, 0f );

	[Header("모니터링용")]
	[SerializeField]
	private Vector3 _vecVelocity = Vector3.zero;
	
	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetVelocity(Vector3 vecVelocity)
	{
		_vecVelocity = vecVelocity;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnPlayEvent()
	{
		base.OnPlayEvent();

		_vecVelocity = PrimitiveHelper.RandomRange( _vecRandomForce_Min, _vecRandomForce_Max );
	}

    public override void OnUpdate()
    {
		base.OnUpdate();

        transform.Translate( _vecVelocity * Time.deltaTime , _pSpaceSimulate );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
