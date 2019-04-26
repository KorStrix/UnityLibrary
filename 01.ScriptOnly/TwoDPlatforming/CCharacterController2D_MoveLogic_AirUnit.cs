#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-02-26 오전 11:09:17
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

[CreateAssetMenu(menuName = "StrixSO/CharacterController2D/" + nameof(CCharacterController2D_MoveLogic_AirUnit))]
public class CCharacterController2D_MoveLogic_AirUnit : CCharacterController2D_MoveLogicBase
{
    public override bool AttachGround(out Collider2D pColliderGround)
    {
        pColliderGround = null;
        return false;
    }

    public override void DoMove(float fMoveVelocity, float fMoveSpeed, Action<Vector2> OnMove)
    {
        OnMove(new Vector2(fMoveVelocity * fMoveSpeed, _pLadderLogic.p_fVerticalMoveAmount_0_1 * fMoveSpeed));
    }

    // ==================================================================================================

}