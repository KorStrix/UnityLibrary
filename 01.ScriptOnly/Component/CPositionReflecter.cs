#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-02-07 오후 5:03:15
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public class CPositionReflecter : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [Rename_Inspector("반사힘")]
    public float p_fPushPower = 10f;

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D pRigidbody = collision.otherRigidbody;
        if(pRigidbody)
        {
            pRigidbody.AddForce((collision.transform.position - transform.position).normalized * p_fPushPower, ForceMode2D.Impulse);
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test