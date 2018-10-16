#if NGUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-05-29 오후 1:41:47
   Description : 
   Edit Log    : 
   ============================================ */

public class CNGUITweenProgress : UITweener
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	public float from = 0f;
	public float to = 0f;

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private UIProgressBar _pUIProgress;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	private void Awake()
	{
		_pUIProgress = GetComponent<UIProgressBar>();
	} 

    protected override void OnUpdate(float factor, bool isFinished)
    {
		if (_pUIProgress == null)
			_pUIProgress = GetComponent<UIProgressBar>();

		_pUIProgress.value = Mathf.Lerp(from, to, factor);
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
#endif