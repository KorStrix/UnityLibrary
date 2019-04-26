#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-02-20 오후 4:08:35
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

[CreateAssetMenu(menuName = "StrixSO/CharacterController2D/" + nameof(CCharacterController2D_LadderLogic_NotClimbLadder))]
public class CCharacterController2D_LadderLogic_NotClimbLadder : CCharacterController2D_LadderLogicBase
{
    protected override void OnCalculateLadder(Collider2D pColliderCeiling, Collider2D pColliderGround, bool bIsGround, int iArrayLength, Collider2D[] arrColliderGround, ref float fMoveAmount_0_1, out bool bIsMoveLadder)
    {
        bIsMoveLadder = false;
    }

    public override void DoCalculateJump(out bool bIsJumping, float fHorizontal_Input)
    {
        bIsJumping = false;
    }
}
