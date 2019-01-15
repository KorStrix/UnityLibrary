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
public class CCompo_SpineColorControl : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    public Color p_pColorEdit;
    public bool p_bEditUpdate = false;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private SkeletonAnimation _pAnimation;
	private Skeleton _pSkeleton;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetColor( Color pColor )
    {
        Init();

        if (_pSkeleton != null)
            _pSkeleton.SetColor(pColor);
    }

    public Color GetColor()
    {
        Init();

        if (_pSkeleton != null)
            return _pSkeleton.GetColor();
        else
            return Color.white;
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

#if UNITY_EDITOR
    private void Update()
    {
        if (p_bEditUpdate)
            DoSetColor(p_pColorEdit);
    }
#endif

    // ========================================================================== //

    /* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

    private void Init()
    {
        if (_pSkeleton == null)
        {
            if (_pAnimation == null)
                _pAnimation = GetComponentInChildren<SkeletonAnimation>();

            _pSkeleton = _pAnimation.skeleton;
        }
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
#endif