#if NGUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-08-30 오전 11:06:22
   Description : 
   Edit Log    : 
   ============================================ */

public enum ELabelIndicator
{
	Label_Indicator
}

public class CResource_NGUILabelIndicator : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private TweenPosition _pTweenPosition;
	private TweenAlpha _pTweenAlpha;

	private UILabel _pUILabel;
	private System.Action _OnDisable;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoStartTween_Indicator( string strText, int iDepth, Color pColor, Vector2 vecTweenPosFrom, CNGUILabelIndicator.SInfoIndicator sInfoIndicator )
	{
		_pUILabel.text = strText;
		_pUILabel.fontSize = sInfoIndicator.iFontSize;
		_pUILabel.color = pColor;
		_pUILabel.depth = iDepth;
		_OnDisable = sInfoIndicator.OnDisable;

		ProcPlayTweenPosition( vecTweenPosFrom, sInfoIndicator.v2Direction, sInfoIndicator.fDistance, sInfoIndicator.fDuration);
		ProcPlayTweenAlpha(sInfoIndicator.fFadeDelay, sInfoIndicator.fFadeDuration);
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	private void OnFinish_TweenAlpha()
	{
		if(_OnDisable != null)
		{
			_OnDisable();
			_OnDisable = null;
		}

		CManagerPooling<ELabelIndicator, CResource_NGUILabelIndicator>.instance.DoPush(this);
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pUILabel = GetComponent<UILabel>();

		_pTweenPosition = GetComponent<TweenPosition>();
		_pTweenAlpha = GetComponent<TweenAlpha>();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void ProcPlayTweenPosition(Vector2 vecFrom, Vector2 v2Direction, float fDistance, float fDistDuration)
	{
		_pTweenPosition.ResetToBeginning();
		_pTweenPosition.from = vecFrom;
		_pTweenPosition.to = vecFrom + (v2Direction * fDistance);
		_pTweenPosition.duration = fDistDuration;
		_pTweenPosition.PlayForward();
	}

	private void ProcPlayTweenAlpha(float fFadeDelay, float fFadeDuration)
	{
		_pTweenAlpha.ResetToBeginning();
		_pTweenAlpha.from = 1f;
		_pTweenAlpha.to = 0f;
		_pTweenAlpha.duration = fFadeDuration;
		_pTweenAlpha.delay = fFadeDelay;

		_pTweenAlpha.SetOnFinished(OnFinish_TweenAlpha);
		
		_pTweenAlpha.PlayForward();
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
#endif