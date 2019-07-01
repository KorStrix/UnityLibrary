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

public abstract class CCharacterController2D_LadderLogicBase : CCharacterController2D_LogicBase
{
    public ObservableCollection<bool> p_Event_OnLadder { get; protected set; } = new ObservableCollection<bool>();

    public Collider2D p_pCollider_Ladder { get; protected set; }
    public bool p_bIsClimbLadder { get; protected set; }
    public float p_fVerticalMoveAmount_0_1 { get; protected set; }

    [DisplayName("사다리 오르는 속도")]
    [SerializeField]
    protected float _fLadderSpeed = 1f;

    // ========================================================================== //

    public void DoCalculateLadder(Collider2D pColliderCeiling, Collider2D pColliderGround, bool bIsGround, int iArrayLength, Collider2D[] arrColliderGround, ref float fMoveAmount_0_1, out bool bIsMoveLadder)
    {
        OnCalculateLadder(pColliderCeiling, pColliderGround, bIsGround, iArrayLength, arrColliderGround, ref fMoveAmount_0_1, out bIsMoveLadder);
        p_fVerticalMoveAmount_0_1 = fMoveAmount_0_1;
    }

    public abstract void DoCalculateJump(out bool bIsJumping, float fHorizontal_Input);

    public virtual void Event_OnExitLadder() { }
    public virtual bool Check_IsPossibleLadder(Collider2D pCollider)
    {
        return pCollider != null && pCollider.tag == "Ladder";
    }

    protected abstract void OnCalculateLadder(Collider2D pColliderCeiling, Collider2D pColliderGround, bool bIsGround, int iArrayLength, Collider2D[] arrColliderGround, ref float fMoveAmount_0_1, out bool bIsMoveLadder);
}
