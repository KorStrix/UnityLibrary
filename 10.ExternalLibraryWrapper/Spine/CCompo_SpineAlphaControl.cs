#if Spine

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Spine.Unity;
using Spine;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

[UnityEngine.ExecuteInEditMode]
public class CCompo_SpineAlphaControl : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	[SerializeField]
	private float fAlpha = 0.5f;
	[SerializeField]
	private bool _bExcutePlaying = false;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private SkeletonAnimation _pAnimation;
	private Skeleton _pSkeleton;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetAlphaRenderer( float fAlpha )
	{
		if (_pSkeleton == null)
		{
			if (_pAnimation == null)
				_pAnimation = GetComponentInChildren<SkeletonAnimation>();

			_pSkeleton = _pAnimation.skeleton;
		}

		if (_pSkeleton != null)
        {
            Color pColor = _pSkeleton.GetColor();
            pColor.a = fAlpha;
            _pSkeleton.SetColor(pColor);

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

		_pAnimation = GetComponentInChildren<SkeletonAnimation>();
		if (_pAnimation.SkeletonDataAsset == null)
			Debug.Log( name + "스켈레톤 데이터 에셋이 없다", this );

		//_pSkeletonData = _pAnimation.SkeletonDataAsset.GetSkeletonData( false );
		_pSkeleton = _pAnimation.skeleton;
	}

    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        if (_bExcutePlaying)
			DoSetAlphaRenderer( fAlpha );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */
	   
	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
#endif