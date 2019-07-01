#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-02-20 오전 11:18:25
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "StrixSO/CharacterController2D/" + nameof(CCharacterController2D_JumpLogicDefault))]
public class CCharacterController2D_JumpLogicDefault : CCharacterController2D_LogicBase
{
    [DisplayName("점프 힘")]
    [SerializeField]
    private float _fJumpForce = 6000f;              public float p_fJumpForce => _fJumpForce;
    [DisplayName("공중에서 조종 가능한지")]
    [SerializeField]
    private bool _bAirControl = true;               public bool p_bAirControl => _bAirControl;
    [DisplayName("떨어지는 상태까지 도달하는 시간")]
    [SerializeField]
    private float _fTimeToFalling = 1f;             public float p_fTimeToFalling => _fTimeToFalling;

    [Space(10)]
    [Header("멀티 점프 관련")]
    [DisplayName("가능한 멀티 점프 수(2 == 더블점프)")]
    [SerializeField]
    private int _iMultipleJumpCount = 2;
    [DisplayName("멀티 점프당 점프 추가 힘")]
    [SerializeField]
    private float _fJumpForce_OnMultiple = -3000f;
    [DisplayName("멀티 점프시 이전 속도를 무시할지")]
    [SerializeField]
    private bool _bIgnore_OnMultiJump = false;

    [DisplayName("현재 점프 카운트", false)]
    [SerializeField]
    private int _iCurrentMultipleJumpCount = 0;

    // ========================================================================== //

    public void DoIncreaseJumpCount()
    {
        if(p_bIsDebuging)
            Debug.Log("DoIncreaseJumpCount - _iCurrentMultipleJumpCount : " + _iCurrentMultipleJumpCount, this);

        _iCurrentMultipleJumpCount++;
    }

    public virtual bool Check_IsPossible_JumpNormal(bool bCheckIsWallSliding, bool bIsGround, bool bIsJumpingCurrent, bool bInput_Jump)
    {
        if (_iCurrentMultipleJumpCount >= _iMultipleJumpCount)
            return false;

        if (_iMultipleJumpCount == 1)
            return (bCheckIsWallSliding || bIsGround) && bIsJumpingCurrent == false && bInput_Jump;
        else
            return bInput_Jump;
    }

    public virtual bool Check_IsPossible_JumpThroughtDown(Collider2D pColliderTerrain, bool bIsGround, bool bIsCrounch, bool bInput_Jump)
    {
        if (pColliderTerrain == null || pColliderTerrain.tag != "Platform")
            return false;

        return bIsGround && bIsCrounch && bInput_Jump;
    }

    public virtual void OnJump_Normal(ref Vector2 vecJumpAddForce, ref bool bInputJump)
    {
        if (p_bIsDebuging)
            Debug.Log("OnJump_Normal : " + vecJumpAddForce, this);

        vecJumpAddForce.y = p_fJumpForce + (_fJumpForce_OnMultiple * _iCurrentMultipleJumpCount);
    }

    public virtual void OnJump_ThroughtDown(HashSet<Collider2D> setCharacterBodyCollider, Collider2D pColliderTerrain, ref bool bIsFalling)
    {
        if (pColliderTerrain == null)
            return;

        bIsFalling = true;
        _pCharacterController2D.StartCoroutine(CoThroughtDownJump(setCharacterBodyCollider, pColliderTerrain));
    }

    IEnumerator CoThroughtDownJump(HashSet<Collider2D> setCharacterBodyCollider, Collider2D pColliderTerrain)
    {
        foreach (var pCollider in setCharacterBodyCollider)
            Physics2D.IgnoreCollision(pCollider, pColliderTerrain, true);

        yield return new WaitForSeconds(0.5f);

        foreach (var pCollider in setCharacterBodyCollider)
            Physics2D.IgnoreCollision(pCollider, pColliderTerrain, false);
    }

    public virtual void Logic_InputJump(ref Vector2 vecJumpAddForce, ref bool bInputJump)
    {
        if (p_bIsDebuging)
            Debug.Log("Logic_InputJump - vecJumpAddForce : " + vecJumpAddForce, this);

        //float fJumpLimit = UpdateJumpVelocity(vecJumpAddForce.y);
        //if (_pRigidbody.velocity.y < fJumpLimit)
        if(_bIgnore_OnMultiJump && _iMultipleJumpCount >= 1)
            rigidbody.velocity = vecJumpAddForce * rigidbody.mass * Time.fixedDeltaTime;
        else
            rigidbody.AddForce(vecJumpAddForce);

        vecJumpAddForce = Vector2.zero;
        bInputJump = false;
    }

    // ========================================================================== //

    protected override void OnInit(CCharacterController2D pCharacterController2D)
    {
        base.OnInit(pCharacterController2D);

        pCharacterController2D.p_Event_OnGround.Subscribe += OnResetJumpCount;
        pCharacterController2D.p_Event_OnLadder.Subscribe += OnResetJumpCount;
        pCharacterController2D.p_Event_OnWallSliding.Subscribe += OnSet_JumpCount_IsOne;
        pCharacterController2D.p_Event_OnFalling.Subscribe += OnFalling;

        _iCurrentMultipleJumpCount = 0;
    }

    private void OnFalling(bool bFalling)
    {
        if(bFalling && _pCharacterController2D.p_bIsJumping == false)
            _iCurrentMultipleJumpCount = 1;
    }

    private void OnResetJumpCount(bool bReset)
    {
        if(bReset)
            _iCurrentMultipleJumpCount = 0;
    }

    private void OnSet_JumpCount_IsOne(bool bReset)
    {
        if (bReset)
            _iCurrentMultipleJumpCount = 1;
    }
}
