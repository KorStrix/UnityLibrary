#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-09-21 오후 1:04:20
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class CManagerSetTargetFrameRate : CSingletonMonoBase<CManagerSetTargetFrameRate>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public int p_iTargetFrameRate = 300;

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        Application.targetFrameRate = p_iTargetFrameRate;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}