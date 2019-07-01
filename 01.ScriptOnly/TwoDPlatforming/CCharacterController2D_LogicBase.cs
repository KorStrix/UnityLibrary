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

public class CCharacterController2D_LogicBase : ScriptableObject
{
    [DisplayName("디버깅 모드")]
    public bool p_bIsDebuging = false;

    protected CCharacterController2D _pCharacterController2D;
    protected Rigidbody2D rigidbody;
    protected Transform transform;

    protected CCharacterController2D_MoveLogicBase _pMoveLogic { get; private set; }
    protected CCharacterController2D_LadderLogicBase _pLadderLogic { get; private set; }

    public void DoInit(CCharacterController2D pCharacterController2D, CCharacterController2D_MoveLogicBase pMoveLogic, CCharacterController2D_LadderLogicBase pLadderLogic)
    {
        _pCharacterController2D = pCharacterController2D;
        rigidbody = _pCharacterController2D.p_pRigidbody;
        transform = _pCharacterController2D.transform;

        _pMoveLogic = pMoveLogic;
        _pLadderLogic = pLadderLogic;

        OnInit(pCharacterController2D);
    }

    public virtual void OnDrawGizmo(bool bIsDebuging) { }
    protected virtual void OnInit(CCharacterController2D pCharacterController2D) { }
}
