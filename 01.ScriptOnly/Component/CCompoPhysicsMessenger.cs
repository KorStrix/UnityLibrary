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

    public enum EPhysicsHow
    {
        Enter, Exit
    }


    /* public - Field declaration            */

    public CObserverSubject<Collider2D, EPhysicsHow> p_Event_OnTrigger2D { get; private set; } = new CObserverSubject<Collider2D, EPhysicsHow>();
    public CObserverSubject<Collision2D, EPhysicsHow> p_Event_OnCollision2D { get; private set; } = new CObserverSubject<Collision2D, EPhysicsHow>();

    public CObserverSubject<Collider2D> p_Event_OnTrigger2D_Stay { get; private set; } = new CObserverSubject<Collider2D>();
    public CObserverSubject<Collision2D> p_Event_OnCollision2D_Stay { get; private set; } = new CObserverSubject<Collision2D>();

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
        p_Event_OnTrigger2D.DoNotify(collision, EPhysicsHow.Enter);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        p_Event_OnTrigger2D_Stay.DoNotify(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        p_Event_OnTrigger2D.DoNotify(collision, EPhysicsHow.Exit);
    }




    private void OnCollisionEnter2D(Collision2D collision)
    {
        p_Event_OnCollision2D.DoNotify(collision, EPhysicsHow.Enter);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        p_Event_OnCollision2D_Stay.DoNotify(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        p_Event_OnCollision2D.DoNotify(collision, EPhysicsHow.Exit);
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