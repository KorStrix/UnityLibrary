#if NGUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : parkjonghwa                             
   Date        : 2017-02-28 오후 6:40:04
   Description : 
   Edit Log    : 
   ============================================ */

[RequireComponent (typeof(UIProgressBar))]
public class CUIInGame_HPbar : CUIInGameBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    protected UIProgressBar _pProgressBar;
    protected float _fHPMax;

    /* private - Variable declaration         */

    // ========================================================================== //

    /* public - [Do] Function
         외부 객체가 호출                         */

    public void DoInitHPBar(Transform pTarget, int iHPMax)
    {
        _fHPMax = iHPMax;
        _pTargetTransform = pTarget;
		_pProgressBar.value = 1;
        gameObject.SetActive(true);

        StartCoroutine(CoSyncToTarget_Position());
    }

    public void DoDamaged(float fRemainHP)
    {
		_pProgressBar.value = fRemainHP / _fHPMax;
    }

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pProgressBar = GetComponent<UIProgressBar>();
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */
 
}
#endif