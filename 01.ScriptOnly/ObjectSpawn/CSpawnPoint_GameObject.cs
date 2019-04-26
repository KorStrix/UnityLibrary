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

/// <summary>
/// 
/// </summary>
public class CSpawnPoint_GameObject : CSpawnPointBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [Rename_Inspector("스폰할 게임오브젝트")]
    public GameObject p_pObjectSpawnOriginal;

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override GameObject GetOriginalObject()
    {
        return p_pObjectSpawnOriginal;
    }

    protected override void OnInit(out string strResourcesPath)
    {
        strResourcesPath = "";
    }

    protected override bool OnCheck_IsDrawHelpBox()
    {
        return false;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}