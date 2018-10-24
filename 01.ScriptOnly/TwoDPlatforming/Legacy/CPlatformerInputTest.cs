#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-16 오후 3:21:30
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPlatformerInputTest : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    /* protected & private - Field declaration         */

    CPlatformerController player;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnStart()
    {
        base.OnStart();

        player = GetComponent<CPlatformerController>();
    }

    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.DoInputVelocity(directionalInput, Input.GetKey(KeyCode.LeftShift));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.DoJumpInputDown();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            player.DoJumpInputUp();
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}