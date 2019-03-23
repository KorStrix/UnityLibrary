#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-02-08 오전 11:31:53
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


public enum EPhysicsEvent
{
    Enter,
    Stay,
    Exit
}

public delegate void OnPhysicsEvent2D(List<Collider2D> listCollider, EPhysicsEvent ePhysicsEvent);
public delegate void OnPhysicsEvent3D(List<Collider> listCollider, EPhysicsEvent ePhysicsEvent);

public interface IPhysicsWrapper
{
    event OnPhysicsEvent2D p_Event_IPhysicsWrapper_OnPhysicsEvent_2D;
    event OnPhysicsEvent3D p_Event_IPhysicsWrapper_OnPhysicsEvent_3D;

    List<Collider2D> GetColliderList_2D_Enter();
    List<Collider2D> GetColliderList_2D_Stay();
    List<Collider2D> GetColliderList_2D_Exit();

    List<Collider> GetColliderList_3D_Enter();
    List<Collider> GetColliderList_3D_Stay();
    List<Collider> GetColliderList_3D_Exit();
}
