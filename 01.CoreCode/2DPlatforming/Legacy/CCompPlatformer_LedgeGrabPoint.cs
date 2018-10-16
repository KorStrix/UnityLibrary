#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-13 오후 6:09:50
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CCompPlatformer_LedgeGrabPoint : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public string strLedgeGrabTag;

    /* protected & private - Field declaration         */

    [GetComponentInParent]
    CPlatformerController _pController_Parents = null;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    // ========================================================================== //

    /* protected - Override & Unity API         */

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == strLedgeGrabTag)
        {
            _pController_Parents.Event_StartClimbing(collision);
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}