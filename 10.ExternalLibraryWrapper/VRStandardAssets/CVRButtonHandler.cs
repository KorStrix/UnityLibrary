#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-04-27 오후 1:15:03
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if VRStandardAssets
using VRStandardAssets.Utils;

[RequireComponent(typeof(VRInteractiveItem))]
public class CVRButtonHandler : CObjectBase
{
    UnityEngine.UI.Button _pButton_UGUI;

    protected override void OnAwake()
    {
        base.OnAwake();

        VRInteractiveItem pInteractiveItem = GetComponent<VRInteractiveItem>();

        _pButton_UGUI = GetComponent<UnityEngine.UI.Button>();
        if(_pButton_UGUI)
            pInteractiveItem.OnClick += OnClickHandle_UGUI;
    }

    private void OnClickHandle_UGUI()
    {
        _pButton_UGUI.enabled = true;
        _pButton_UGUI.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current));
        _pButton_UGUI.enabled = false;
    }
}
#endif