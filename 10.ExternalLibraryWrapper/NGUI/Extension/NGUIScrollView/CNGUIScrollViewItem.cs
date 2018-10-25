#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : Strix, KJH
 *	작성자 : KJH
 *	
 *	기능 : 데이터 세팅 및 버튼 클릭 이벤트를 사용하기위한 클래스,
 *		   사용하고싶은 스크롤뷰 오브젝트에 컴포넌트 추가.
   ============================================ */
#endregion Header
#if NGUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(UIDragScrollView))]
public class CNGUIScrollViewItem : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field
	/* public - Field declaration            */

	[Header( "디버그용" )]
	[SerializeField]
	private int _iRealID; public int p_iRealID { get { return _iRealID; } }

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private CNGUITweenPlayGroup _pUITweenGroupPlay; public CNGUITweenPlayGroup p_pUITweenGroupPlay { get { return _pUITweenGroupPlay; } }

	private IOnClickItemListener _pListener;

	#endregion Field
	#region Public
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetRealID(int iRealID)
	{
		_iRealID = iRealID;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	#endregion Public
	// ========================================================================== //
	#region Protected
	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	private void OnClick_Item(string strButtonName)
	{
		if (_pListener == null) return;

		_pListener.IOnClick_Item(_iRealID, strButtonName);
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pUITweenGroupPlay = GetComponent<CNGUITweenPlayGroup>();

		UIButton[] arrButton = GetComponentsInChildren<UIButton>();

		int iLen_Button = arrButton.Length;
		if (iLen_Button > 0)
		{
			_pListener = GetComponentInParent<IOnClickItemListener>();
			if (_pListener == null)
			{
				Debug.Log( "부모 오브젝트가 IOnClickItemListener 인터페이스를 상속 받아야합니다." );
				return;
			}
		}
		else return;

		for (int i = 0; i < iLen_Button; i++)
		{
			UIButton pUIButton = arrButton[i];
			EventDelegate.Set(pUIButton.onClick, new EventDelegate(this, "OnClick_Item", pUIButton.name));
		}
	}

	//protected override void OnUpdate()
	//{
	//	base.OnUpdate();

	//	if (Application.isEditor && Application.isPlaying == false)
	//		NGUITools.AddWidgetCollider(gameObject, true);
	//}

	#endregion Protected
	// ========================================================================== //
	#region Private
	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */
	#endregion Private
}
#endif