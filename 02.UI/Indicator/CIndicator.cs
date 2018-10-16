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
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class CIndicator : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EIndicatorType
	{
		None,
		UGUI,
		NGUI,
		TextMeshPro,
	}

	/* public - Field declaration            */

	public class EventIndicator : UnityEvent<CIndicator> { }
	
	public EventIndicator p_Event_OnDisable = new EventIndicator();

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private Text _pUIText;	public Text p_pUIText {  get { return _pUIText; } }
	//private RectTransform _pRectTrans;

#if TMPro
	private TMPro.TextMeshPro _pUIText_TMPro;	public TMPro.TextMeshPro p_pUIText_TMPro { get { return _pUIText_TMPro; } }
#endif

#if NGUI
	private UILabel _pUILabel; public UILabel p_pUILabel { get { return _pUILabel; } }
#endif

	private EIndicatorType _eIndicatorType = EIndicatorType.None;	public EIndicatorType p_eType {  get { return _eIndicatorType; } }

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetText(string strText)
	{
		switch (_eIndicatorType)
		{
			case EIndicatorType.UGUI:
				_pUIText.text = strText;
				break;
#if NGUI
			case EIndicatorType.NGUI:
				_pUILabel.text = strText;
				break;
#endif
#if TMPro
			case EIndicatorType.TextMeshPro:
				_pUIText_TMPro.text = strText;
				break;
#endif
		}
	}

	public void DoSetPos(Vector3 vecPos)
	{
        transform.position = vecPos;
	}

	public void DoSetColor(Color pColor)
	{
		switch (_eIndicatorType)
		{
			case EIndicatorType.UGUI:
				_pUIText.color = pColor;
				break;
#if NGUI
			case EIndicatorType.NGUI:
				_pUILabel.color = pColor;
				break;
#endif
#if TMPro
			case EIndicatorType.TextMeshPro:
				_pUIText_TMPro.color = pColor;
				break;
#endif
		}
	}

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

		if(this.GetComponentInChildren(out _pUIText ))
		{
			_eIndicatorType = EIndicatorType.UGUI;
			//_pRectTrans = _pUIText.GetComponent<RectTransform>();
			return;
		}

#if TMPro
		if (this.GetComponentInChildren( out _pUIText_TMPro ))
		{
			_eIndicatorType = EIndicatorType.TextMeshPro;
			//_pRectTrans = _pUIText_TMPro.GetComponent<RectTransform>();
			return;
		}
#endif
#if NGUI
		if (this.GetComponentInChildren( out _pUILabel ))
		{
			_eIndicatorType = EIndicatorType.NGUI;
			return;
		}
#endif
    }


    protected override void OnEnableObject()
	{
		base.OnEnableObject();

		if (transform.localScale != Vector3.one)
            transform.localScale = Vector3.one;
	}

	protected override void OnDisableObject()
	{
		base.OnDisableObject();

		p_Event_OnDisable.Invoke(this);
	}

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
