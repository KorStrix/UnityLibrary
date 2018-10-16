#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 참고 링크
// https://answers.unity.com/questions/682285/editor-script-for-setting-the-sorting-layer-of-an.html
[ExecuteInEditMode]
[System.Serializable]
public class CCompoSortingLayer : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	public string strSortingLayer;
	public int iSortOrder;

	/* protected - Field declaration         */

	/* private - Field declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
    {
        base.OnAwake();

        SetSortingLayer();
    }

    private void SetSortingLayer()
    {
        Renderer pRenderer = GetComponent<Renderer>();
        if (pRenderer != null)
        {
            pRenderer.sortingLayerName = strSortingLayer;
            pRenderer.sortingOrder = iSortOrder;
        }
    }

    #endregion Protected

    // ========================================================================== //

    #region Private

    /* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

#if UNITY_EDITOR
    private void Update()
    {
        SetSortingLayer();
    }
#endif

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
