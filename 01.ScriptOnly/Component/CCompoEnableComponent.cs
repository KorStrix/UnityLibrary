using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CCompoEnableComponent : CCompoEventTrigger
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	[System.Serializable]
	public class SEnableComponentEvent
	{
		public Behaviour pUnityComponent;
		public bool bEnable;
		public float fDelayTimeSec;
	}

	/* public - Variable declaration            */

	public List<SEnableComponentEvent> p_listComponent = new List<SEnableComponentEvent>();
	
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

		for(int i = 0; i < p_listComponent.Count; i++)
		{
			if(p_listComponent[i].fDelayTimeSec != 0f)
				StartCoroutine( CoDelayAction( p_listComponent[i] ) );
			else
				p_listComponent[i].pUnityComponent.enabled = p_listComponent[i].bEnable;
		}
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private IEnumerator CoDelayAction( SEnableComponentEvent pEvent )
	{
		yield return YieldManager.GetWaitForSecond( pEvent.fDelayTimeSec );

		pEvent.pUnityComponent.enabled = pEvent.bEnable;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
