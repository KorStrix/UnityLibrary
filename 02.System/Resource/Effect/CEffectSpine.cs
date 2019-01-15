#if Spine

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Spine;

/* ============================================ 
   Editor      : Strix
   Description : 
   Edit Log    : 
   ============================================ */

public class CEffectSpine : CEffect
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private SkeletonAnimation _pSpineAnimation;
	private CCompoAutoDisable _pAutoDisable;
	//private string _strAnimationName;

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

	protected override void OnDefineEffect()
	{
		base.OnDefineEffect();

		if (_eEffectType == EEffectType.None)
		{
			_pSpineAnimation = GetComponentInChildren<SkeletonAnimation>();
			if (_pSpineAnimation != null)
			{
    //            Spine.Animation[] arrAnimation = _pSpineAnimation.Skeleton.data.Animations.Items;
    //            Spine.Animation pAnimationFirst = arrAnimation[0];
    //            _strAnimationName = pAnimationFirst.name;
				//_pSpineAnimation.loop = false;
				//_pSpineAnimation.AnimationName = _strAnimationName;

				_eEffectType = EEffectType.Spine;
				//_pAutoDisable = gameObject.AddComponent<CCompoAutoDisable>();
				//_pAutoDisable.fAutoDisableTime = pAnimationFirst.duration;
				//_pAutoDisable.p_eInputType_Main = CCompoEventTrigger.EInputType.OnEnable;
				//_pAutoDisable.DoPlayEvent_Main();
			}
		}
	}

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		//if (_pSpineAnimation != null)
		//{
		//	_pSpineAnimation.AnimationName = "";
		//	_pSpineAnimation.AnimationName = _strAnimationName;
		//}
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
#endif