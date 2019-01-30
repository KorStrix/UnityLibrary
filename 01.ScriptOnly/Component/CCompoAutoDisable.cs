using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CCompoAutoDisable : CCompoEventTrigger
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public float fAutoDisableTime = 5f;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	System.Action<GameObject> _OnDisable;
    Coroutine _pCoroutine;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetCallBack_OnDisable(System.Action<GameObject> OnDisable )
	{
		_OnDisable = OnDisable;
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

        _pCoroutine = StartCoroutine(CoDelayDisable());
	}

    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        if (_pCoroutine != null)
            StopCoroutine(_pCoroutine);
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

    private IEnumerator CoDelayDisable()
	{
		yield return YieldManager.GetWaitForSecond(fAutoDisableTime);

		gameObject.SetActive( false );
		if(_OnDisable != null)
			_OnDisable( gameObject);
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
