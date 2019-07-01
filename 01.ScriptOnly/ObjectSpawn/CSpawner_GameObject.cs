#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-02 오후 9:10:22
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// 
/// </summary>
public class CSpawner_GameObject : CSpawnerBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

#if ODIN_INSPECTOR
    [LabelText("스폰할 게임 오브젝트"), ListDrawerSettings(Expanded = false)]
#endif
    public List<SpawnSettingInfo> p_listSpawnSetting_GameObject;

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override List<SpawnSettingInfo> GetSpawnObject_OriginalList()
    {
        return p_listSpawnSetting_GameObject;
    }

    protected override void OnInit(ref string strResourcesPath_Default_Is_ResourcesPath)
    {
        strResourcesPath_Default_Is_ResourcesPath = "";
    }

    protected override bool OnCheck_IsDraw_SelectSpawnObject()
    {
        return false;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}