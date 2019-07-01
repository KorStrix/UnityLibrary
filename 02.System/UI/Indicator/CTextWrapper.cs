#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
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

public class CTextWrapper : CObjectBase, IPoolingUIObject
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EIndicatorType
	{
		None,
		UGUI,
		TextMeshPro,
	}

	/* public - Field declaration            */

    public ObservableCollection<CTextWrapper> p_Event_OnDisable { get; private set; } = new ObservableCollection<CTextWrapper>();
    public string text
    {
        get { return GetText(); }
        set { DoSetText(value); }
    }

    [GetComponentInChildren(bIsPrint_OnNotFound = false)]
    public Text p_pUIText { get; private set; }

#if TMPro
    [GetComponentInChildren(bIsPrint_OnNotFound = false)]
    public TMPro.TextMeshPro p_pUIText_TMPro { get; private set; }
#endif


    public EIndicatorType p_eIndicatorType { get; private set; } = EIndicatorType.None;

    /* protected - Field declaration         */

    /* private - Field declaration           */

    //private RectTransform _pRectTrans;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoSetText(string strText)
	{
		switch (p_eIndicatorType)
		{
			case EIndicatorType.UGUI:
                p_pUIText.text = strText;
				break;
#if TMPro
			case EIndicatorType.TextMeshPro:
				p_pUIText_TMPro.text = strText;
				break;
#endif
		}
	}

    public string GetText()
    {
        switch (p_eIndicatorType)
        {
            case EIndicatorType.UGUI: return p_pUIText.text;
#if TMPro
            case EIndicatorType.TextMeshPro: return p_pUIText_TMPro.text;
#endif
        }

        return "";
    }

	public void DoSetPos(Vector3 vecPos)
	{
        transform.position = vecPos;
	}

	public void DoSetColor(Color pColor)
	{
		switch (p_eIndicatorType)
		{
			case EIndicatorType.UGUI:
                p_pUIText.color = pColor;
				break;
#if TMPro
			case EIndicatorType.TextMeshPro:
				p_pUIText_TMPro.color = pColor;
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

		if(p_pUIText)
		{
			p_eIndicatorType = EIndicatorType.UGUI;
			return;
		}

#if TMPro
		if (p_pUIText_TMPro)
		{
			p_eIndicatorType = EIndicatorType.TextMeshPro;
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

	protected override void OnDisableObject(bool bIsQuitApplciation)
	{
		base.OnDisableObject(bIsQuitApplciation);

		p_Event_OnDisable.DoNotify(this);
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
