#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-07 오전 9:59:24
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CUICompoLocalize_Sprite : CUICompoLocalizeBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    /* protected & private - Field declaration         */

    static private CResourceGetter<Sprite> g_pResourceGetter = null;


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */


    public override void ILocalizeListner_ChangeLocalize(SystemLanguage eLanguage, string strLocalizeValue)
    {
        if (g_pResourceGetter == null)
            g_pResourceGetter = new CResourceGetter<Sprite>("Sprite");

        UnityEngine.UI.Image pImage_UGUI = GetComponent<UnityEngine.UI.Image>();
        if (pImage_UGUI)
            pImage_UGUI.sprite = g_pResourceGetter.GetResource(strLocalizeValue);

    #if NGUI
        _pSprite_NGUI = GetComponent<UISprite>();
        if(_pSprite_NGUI)
            _pSprite_NGUI.sprite = g_pResourceGetter.GetResource_Origin(strText);
#endif
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private

    // ========================================================================== //
    
}
