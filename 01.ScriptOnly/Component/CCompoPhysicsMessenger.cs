#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-06-14 오전 11:54:31
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

public class CCompoPhysicsMessenger : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */


    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        if (transform.parent == null)
        {
            Debug.LogWarning(name + " 이 컴포넌트는 부모 오브젝트가 있어야 정상 동작합니다.", gameObject);
            Destroy(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.parent.SendMessageUpwards("OnTriggerEnter2D", collision, SendMessageOptions.DontRequireReceiver);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.parent.SendMessageUpwards("OnCollisionEnter2D", collision, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        transform.parent.SendMessageUpwards("OnTriggerStay2D", collision, SendMessageOptions.DontRequireReceiver);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        transform.parent.SendMessageUpwards("OnCollisionStay2D", collision, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        transform.parent.SendMessageUpwards("OnTriggerExit2D", collision, SendMessageOptions.DontRequireReceiver);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        transform.parent.SendMessageUpwards("OnCollisionExit2D", collision, SendMessageOptions.DontRequireReceiver);
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