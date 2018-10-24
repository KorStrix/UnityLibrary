#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-07 오전 10:10:38
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class CUICompoLocalizeBase : CObjectBase, CManagerUILocalize.ILocalizeListener
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public bool p_bIsAutoChange_OnEnable = true;
    public string _strLocalizeKey;

    /* protected & private - Field declaration         */

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    public string ILocalizeListner_GetLocalizeKey()
    {
        return _strLocalizeKey;
    }

    void Reset()
    {
        if (Application.isEditor && Application.isPlaying == false)
            _strLocalizeKey = name;
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        CManagerUILocalize.instance.DoRegist_LocalizeListener(this, gameObject);

        if (p_bIsAutoChange_OnEnable)
            ILocalizeListner_ChangeLocalize(CManagerUILocalize.instance.p_eCurrentLocalize, CManagerUILocalize.instance.DoGetCurrentLocalizeValue_Random(_strLocalizeKey));
    }

    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        CManagerUILocalize.instance.DoRemove_LocalizeListener(this);
    }


    /* protected - [abstract & virtual]         */

    abstract public void ILocalizeListner_ChangeLocalize(SystemLanguage eLanguage, string strLocalizeValue);

    // ========================================================================== //

    #region Private

    #endregion Private
    
}
