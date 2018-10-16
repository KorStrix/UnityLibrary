using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* ============================================ 
   Editor      : Strix
   Date        : 2017-04-05 오후 1:24:58
   Description : 
   Edit Log    : 
   ============================================ */

public class CUICompoLocalize_Text : CUICompoLocalizeBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    /* private - Variable declaration           */


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

    public override void ILocalizeListner_ChangeLocalize(SystemLanguage eLanguage, string strLocalizeValue)
    {
        var pText_UGUI = GetComponent<UnityEngine.UI.Text>();
        if (pText_UGUI)
            pText_UGUI.text = strLocalizeValue;
#if NGUI
		var pText_NGUI = GetComponent<UILabel>();
		if (pText_NGUI != null)
			pText_NGUI.text = strLocalizeValue;
#endif
#if TMPro
		var pText_TMPro = GetComponent<TMPro.TextMeshPro>();
		if (pText_TMPro != null)
			pText_TMPro.text = strLocalizeValue;
#endif
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
