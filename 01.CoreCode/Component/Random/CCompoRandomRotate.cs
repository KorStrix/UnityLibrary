using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CCompoRandomRotate : CCompoEventTrigger
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public Vector3 _vecRandomRotateSpeed_Min = new Vector3( -10f, -10f, 0f );
	public Vector3 _vecRandomRotateSpeed_Max = new Vector3( 10f, 10f, 0f );

	[SerializeField]
	private bool _bExcuteUpdate = false;

	[Header( "모니터링용" )]
	[SerializeField]
	private Vector3 _vecRotate = Vector3.zero;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

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

		_vecRotate = PrimitiveHelper.RandomRange( _vecRandomRotateSpeed_Min, _vecRandomRotateSpeed_Max );
        transform.Rotate(_vecRotate);

		if (_bExcuteUpdate)
			StartCoroutine("CoUpdateRotate");
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private IEnumerator CoUpdateRotate()
	{
		while(true)
		{
            transform.Rotate(_vecRotate * Time.deltaTime);

			yield return null;
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
