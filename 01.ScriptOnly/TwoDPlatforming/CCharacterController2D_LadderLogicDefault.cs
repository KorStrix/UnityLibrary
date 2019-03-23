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

[CreateAssetMenu(menuName = "StrixSO/" + nameof(CCharacterController2D_LadderLogicDefault))]
public class CCharacterController2D_LadderLogicDefault : CCharacterController2D_LogicBase
{
    public CObserverSubject<bool> p_Event_OnLadder { get; private set; } = new CObserverSubject<bool>();

    public Collider2D p_pCollider_Ladder { get; private set; }
    public bool p_bIsClimbLadder { get; private set; }

    [Rename_Inspector("사다리 오르는 속도")]
    [SerializeField]
    private float _fLadderSpeed = 1f;

    float _fGraivityOrigin;
    bool _bLockClimb;

    // ========================================================================== //

    public bool Check_HasLadder()
    {
        return p_pCollider_Ladder != null;
    }

    public bool DoMove_OnLadder(float fMoveAmount)
    {
        if (fMoveAmount == 0f)
            return false;

        if (p_bIsClimbLadder == false && _bLockClimb == false)
            OnClimbLadder();

        Vector3 vecPos = transform.position;
        if (fMoveAmount > 0f)
            vecPos.y += _fLadderSpeed * Time.deltaTime;
        else
            vecPos.y -= _fLadderSpeed * Time.deltaTime;

        transform.position = vecPos;
        return true;
    }

    public virtual void DoCalculateLadder(Collider2D pColliderCeiling, Collider2D pColliderGround, bool bIsGround, int iArrayLength, Collider2D[] arrColliderGround, ref float fMoveAmount_0_1, out bool bIsMoveLadder)
    {
        bIsMoveLadder = false;
        bool bHasLadder_Prev = Check_HasLadder();

        int iLadderColliderCount = 0;
        // 직전에 사다리가 있거나, 사다리가 없었어도 인풋이 들어오면
        if (bHasLadder_Prev || (bHasLadder_Prev == false && fMoveAmount_0_1 != 0f))
        {
            // 바닥에만 사다리가 있을 때
            if (Check_IsPossibleLadder(out iLadderColliderCount, iArrayLength, arrColliderGround) && fMoveAmount_0_1 < 0f && (bIsGround == false || (bIsGround && iLadderColliderCount >= 2)) )

                DoSetLadderCollider(arrColliderGround[0]);

            // 천장에 사다리가 있는 경우
            if (Check_IsPossibleLadder(pColliderCeiling) && fMoveAmount_0_1 > 0f)
                DoSetLadderCollider(pColliderCeiling);
        }

        if ((fMoveAmount_0_1 > 0f && pColliderCeiling == null) || (bHasLadder_Prev && Check_HasLadder() == false) || (iLadderColliderCount <= 1 && Check_MoveDown_OnGround(bIsGround, fMoveAmount_0_1)))
        {
            fMoveAmount_0_1 = 0f;
            Event_OnExitLadder();
            return;
        }

        if (_bLockClimb == false && Check_HasLadder())
        {
            DoMove_OnLadder(fMoveAmount_0_1);
            bIsMoveLadder = true;
        }
    }

    public virtual bool Check_IsPossibleLadder(Collider2D pCollider)
    {
        return pCollider != null && pCollider.tag == "Ladder";
    }

    public virtual void DoCalculateJump(out bool bIsJumping, float fHorizontal_Input)
    {
        bIsJumping = fHorizontal_Input != 0f;
        _bLockClimb = bIsJumping == false;

        if(p_bIsClimbLadder)
        {
            Collider2D pColliderLadder = p_pCollider_Ladder;
            Event_OnExitLadder();

            if (pColliderLadder != null)
                _pCharacterController2D.StartCoroutine(CoIgnoreLadderCollider(pColliderLadder));
        }
    }

    public bool Check_IsPossibleLadder(out int iLadderColliderCount, int iArrayLength, params Collider2D[] arrCollider)
    {
        iLadderColliderCount = 0;
        for (int i = 0; i < iArrayLength; i++)
        {
            if (Check_IsPossibleLadder(arrCollider[i]))
                iLadderColliderCount++;
        }

        return iLadderColliderCount != 0;
    }

    public void Event_OnExitLadder()
    {
        if (p_pCollider_Ladder == null)
            return;

        OnExitLadder();
    }

    public void DoSetLadderCollider(Collider2D pCollider)
    {
        p_pCollider_Ladder = pCollider;
    }

    public void RemoveLadderCollider(Collider2D pCollider)
    {
        if(pCollider != null && p_pCollider_Ladder == pCollider)
            Event_OnExitLadder();
    }

    // ========================================================================== //

    protected override void OnInit(CCharacterController2D pCharacterController2D)
    {
        base.OnInit(pCharacterController2D);

        _bLockClimb = false;
        p_pCollider_Ladder = null;
        p_bIsClimbLadder = false;

        _pCharacterController2D.p_Event_OnGround.Subscribe += OnGround;
    }

    // ========================================================================== //

    IEnumerator CoIgnoreLadderCollider(Collider2D pColliderLadder)
    {
        foreach (var pBodyCollider in _pCharacterController2D.p_setCharacterBody)
            Physics2D.IgnoreCollision(pColliderLadder, pBodyCollider, true);

        yield return new WaitForSeconds(0.2f);

        foreach (var pBodyCollider in _pCharacterController2D.p_setCharacterBody)
            Physics2D.IgnoreCollision(pColliderLadder, pBodyCollider, false);
    }

    private void OnClimbLadder()
    {
        _fGraivityOrigin = _pRigidbody.gravityScale;
        _pRigidbody.gravityScale = 0f;
        _pRigidbody.isKinematic = true;
        _pRigidbody.velocity = Vector2.zero;

        Vector3 vecLadderPosition = p_pCollider_Ladder.transform.position;
        vecLadderPosition.y = transform.position.y;
        transform.position = vecLadderPosition;

        SetClimbLadder(true);

        if (p_bIsDebuging)
            Debug.Log("OnClimbLadder - _fGraivityOrigin : " + _fGraivityOrigin, this);
    }

    private void OnExitLadder()
    {
        p_pCollider_Ladder = null;
        _pRigidbody.gravityScale = _fGraivityOrigin;
        _pRigidbody.isKinematic = false;
        SetClimbLadder(false);

        if (p_bIsDebuging)
            Debug.Log("Event_OnExitLadder - _fGraivityOrigin : " + _fGraivityOrigin, this);
    }

    private void SetClimbLadder(bool bValue)
    {
        p_bIsClimbLadder = bValue;
        p_Event_OnLadder.DoNotify(bValue);
    }

    private void OnGround(bool bIsGround)
    {
        if (_bLockClimb && bIsGround)
            _bLockClimb = false;
    }

    private bool Check_MoveDown_OnGround(bool bIsGround, float fMoveAmount_0_1)
    {
        return (bIsGround && fMoveAmount_0_1 < 0f);
    }
}
