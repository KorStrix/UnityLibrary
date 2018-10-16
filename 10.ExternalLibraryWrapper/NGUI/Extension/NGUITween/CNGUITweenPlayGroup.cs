#if NGUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-09-21 오후 9:09:17
   Description : 
   Edit Log    : 
   ============================================ */

public class CNGUITweenPlayGroup : CCompoEventTrigger
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */
	
	private enum ETweenPlay
	{
		Forward,
		Reverse
	}

	/* public - Variable declaration            */
	
	[Header("플레이 방향")] [SerializeField] private ETweenPlay p_eTweenPlay = ETweenPlay.Forward;
	[Header("플레이 대상 그룹ID")] [SerializeField] private int p_iTweenGroupID = 0;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private List<UITweener> _listTween = new List<UITweener>();

	private int iCachedTweenCount;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoPlayTweenGroup_Forward(int iGroupID, float fDelay = 0f)
	{
		ProcPlayTweenGroup(iGroupID, fDelay, ETweenPlay.Forward);
	}

	public void DoPlayTweenGroup_Reverse(int iGroupID, float fDelay = 0f)
	{
		ProcPlayTweenGroup(iGroupID, fDelay, ETweenPlay.Reverse);
	}

	public void DoStopTweenGroup(int iGroupID)
	{
		ProcStopTweenGroup(iGroupID);
	}

	public void DoStopTweenAll()
	{
		ProcStopTweenAll();
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	protected override void OnPlayEventMain()
	{
		base.OnPlayEventMain();

		ProcPlayTweenGroup( p_iTweenGroupID, 0f, p_eTweenPlay );
	}
	
	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		GetComponentsInChildren<UITweener>(true, _listTween);

		iCachedTweenCount = _listTween.Count;
		for (int i = 0; i < iCachedTweenCount; i++)
			_listTween[i].enabled = false;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void ProcPlayTweenGroup(int iGroupID, float fDelay, ETweenPlay eTweenPlay)
	{
		for (int i = 0; i < iCachedTweenCount; i++)
		{
			UITweener pUITween = _listTween[i];
			if (pUITween.tweenGroup != iGroupID) continue;

			if (fDelay > 0f)
				pUITween.delay = fDelay;

			pUITween.Play(eTweenPlay == ETweenPlay.Forward);
			pUITween.ResetToBeginning();
		}
	}

	private void ProcStopTweenGroup(int iGroupID)
	{
		for (int i = 0; i < iCachedTweenCount; i++)
		{
			UITweener pUITween = _listTween[i];
			if (pUITween.tweenGroup != iGroupID) continue;

			pUITween.enabled = false;
			pUITween.ResetToBeginning();
		}
	}

	private void ProcStopTweenAll()
	{
		for (int i = 0; i < iCachedTweenCount; i++)
		{
			UITweener pUITween = _listTween[i];

			pUITween.enabled = false;
			pUITween.ResetToBeginning();
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
#endif