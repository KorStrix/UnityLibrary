#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-04-23 오후 4:12:21
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if VRStandardAssets
using VRStandardAssets.Utils;

[RequireComponent(typeof(VRInteractiveItem))]
public class CVRInteractiveItem_DebugPrint : CObjectBase
{
    protected override void OnAwake()
    {
        base.OnAwake();

        VRInteractiveItem pInteractiveItem = GetComponent<VRInteractiveItem>();
        pInteractiveItem.OnClick += PInteractiveItem_OnClick;
        pInteractiveItem.OnDoubleClick += PInteractiveItem_OnDoubleClick;
        pInteractiveItem.OnDown += PInteractiveItem_OnDown;
        pInteractiveItem.OnOut += PInteractiveItem_OnOut;
        pInteractiveItem.OnOver += PInteractiveItem_OnOver;
        pInteractiveItem.OnUp += PInteractiveItem_OnUp;
    }

    private void PInteractiveItem_OnUp()
    {
        PrintLog("PInteractiveItem_OnUp");
    }

    private void PInteractiveItem_OnOver()
    {
        PrintLog("PInteractiveItem_OnOver");
    }

    private void PInteractiveItem_OnOut()
    {
        PrintLog("PInteractiveItem_OnOut");
    }

    private void PInteractiveItem_OnDown()
    {
        PrintLog("PInteractiveItem_OnDown");
    }

    private void PInteractiveItem_OnDoubleClick()
    {
        PrintLog("PInteractiveItem_OnDoubleClick");
    }

    private void PInteractiveItem_OnClick()
    {
        PrintLog("PInteractiveItem_OnClick");
    }

    private void PrintLog(string strPrintLog)
    {
        Debug.Log(name + strPrintLog, this);
    }
}
#endif
