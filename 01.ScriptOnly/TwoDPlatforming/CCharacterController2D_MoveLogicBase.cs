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

public abstract class CCharacterController2D_MoveLogicBase : CCharacterController2D_LogicBase
{
    [DisplayName("걷기 속도")]
    public float p_fSpeed_OnWalking = 40f;
    [DisplayName("달리기 속도")]
    public float p_fSpeed_OnRunning = 80f;
    [DisplayName("최대 걷기 속도까지 걸리는 시간")]
    public float p_fTimeToMaxSpeedApex = 0.5f;
    [DisplayName("걷기로 도달하는 최대 Move Delta (1이 달리기)")] [UnityEngine.Range(0, 1)]
    public float p_fMaxMoveDelta_OnWalking = 0.5f;
    [DisplayName("바라보는 방향 기준을 인풋이 아닌 속도를 기준으로 할 것인지")]
    public bool p_bLookAtVelocity = false;
    [DisplayName("앞에 막힌 벽을 향해 움직일 때 움직이는 애니메이션을 할것인지")]
    public bool p_bIsPlayAnimation_OnForwardIsBlock = false;

    [Space(10)]
    [Header("바닥에 붙는 옵션")]
    [DisplayName("바닥에 붙는 옵션을 사용할 것인지")]
    public bool p_bUseAttachGround = true;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf(nameof(p_bUseAttachGround))]
#endif
    [DisplayName("바닥 감지 레이 Y 위치 오프셋")]
    public float p_fGroundCheck_RayOrigin = -3.025f;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf(nameof(p_bUseAttachGround))]
#endif
    [DisplayName("바닥 감지 레이 길이")]
    public float p_fGroundCheck_RayDistance = 2f;

    [Space(10)]
    [Header("제동 옵션")]
    [DisplayName("제동 시 천천히 멈추기 유무")]
    public bool p_bIsSmoothMoveStop = false;
    [DisplayName("제동 시간")]
    public float p_fTimeToStop = 0.1f;

    protected CapsuleCollider2D _pColliderGround;
    protected LayerMask _sLayerTerrain;

    // =========================================================================================

    virtual public void DoMove(float fMoveVelocity, float fMoveSpeed, System.Action<Vector2> OnMove)
    {
        OnMove(new Vector2(fMoveVelocity * fMoveSpeed, rigidbody.velocity.y));
    }

    virtual public void DoCalculate_MoveVelocity(float fMoveAmount, bool bInput_Crouch, bool bInput_Run, float fCrouchSpeed, ref float fMoveVelocity)
    {
        if (_pCharacterController2D.p_bIsGround || _pCharacterController2D.p_pLogicJump.p_bAirControl)
        {
            fMoveVelocity = (bInput_Crouch ? fMoveAmount * fCrouchSpeed : fMoveAmount);

            if (p_bLookAtVelocity)
            {
                if (fMoveVelocity > 0)
                    _pCharacterController2D.DoSet_ChangeFaceDir(true);
                else if (fMoveVelocity < 0)
                    _pCharacterController2D.DoSet_ChangeFaceDir(false);
            }
            else if (_pCharacterController2D.p_bIsSlopeSliding == false)
            {
                if ((fMoveVelocity > 0 && _pCharacterController2D.p_bFaceDirection_IsRight == false) ||
                     fMoveVelocity < 0 && _pCharacterController2D.p_bFaceDirection_IsRight)
                {
                    _pCharacterController2D.DoSet_ChangeFaceDir(!_pCharacterController2D.p_bFaceDirection_IsRight);
                }
            }
        }
    }


    virtual public void Calculate_MoveDelta(ref float fMoveDelta_0_1)
    {
        if (_pCharacterController2D.p_bIsLock_Move)
        {
            fMoveDelta_0_1 = 0f;
            return;
        }

        if (_pCharacterController2D.p_bIsMoving && fMoveDelta_0_1 < p_fMaxMoveDelta_OnWalking)
        {
            fMoveDelta_0_1 += (1f / p_fTimeToMaxSpeedApex) * Time.deltaTime;
            if (fMoveDelta_0_1 > p_fMaxMoveDelta_OnWalking)
                fMoveDelta_0_1 = p_fMaxMoveDelta_OnWalking;
        }

        if (_pCharacterController2D.p_bIsMoving == false)
        {
            if (p_bIsSmoothMoveStop)
            {
                if (fMoveDelta_0_1 > 0f)
                    fMoveDelta_0_1 -= (1f / p_fTimeToStop) * Time.deltaTime;
            }
            else
                fMoveDelta_0_1 = 0f;
        }
    }

    virtual public float GetMoveSpeed(float fMoveDelta_0_1)
    {
        return Mathf.Lerp(p_fSpeed_OnWalking, p_fSpeed_OnRunning, fMoveDelta_0_1);
    }

    abstract public bool AttachGround(out Collider2D pColliderGround);

    // ==================================================================================================

    protected override void OnInit(CCharacterController2D pCharacterController2D)
    {
        base.OnInit(pCharacterController2D);

        _pColliderGround = pCharacterController2D.p_pCollider_GroundChecker;
        _sLayerTerrain = pCharacterController2D.p_sWhatIsTerrain;
    }
}