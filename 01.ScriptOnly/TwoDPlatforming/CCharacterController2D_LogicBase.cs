#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-02-20 오전 11:34:31
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

public class CCharacterController2D_LogicBase : ScriptableObject
{
    [Rename_Inspector("디버깅 모드")]
    public bool p_bIsDebuging = false;

    protected CCharacterController2D _pCharacterController2D;
    protected Rigidbody2D _pRigidbody;
    protected Transform transform;

    public void DoInit(CCharacterController2D pCharacterController2D)
    {
        _pCharacterController2D = pCharacterController2D;
        _pRigidbody = _pCharacterController2D.p_pRigidbody;
        transform = _pCharacterController2D.transform;

        OnInit(pCharacterController2D);
    }

    public virtual void OnDrawGizmo(bool bIsDebuging) { }
    protected virtual void OnInit(CCharacterController2D pCharacterController2D) { }
}
