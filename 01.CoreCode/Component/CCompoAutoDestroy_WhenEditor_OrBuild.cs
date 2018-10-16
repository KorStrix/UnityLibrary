using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CCompoAutoDestroy_WhenEditor_OrBuild : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

    public enum ECurrentType
    {
        Editor,
        Build
    }

    /* public - Variable declaration            */

    public ECurrentType _eCurrentType = ECurrentType.Editor;

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

    protected override void OnAwake()
	{
		base.OnAwake();

        if (Application.isEditor && _eCurrentType == ECurrentType.Editor)
            DestroyImmediate(gameObject);
    }

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
