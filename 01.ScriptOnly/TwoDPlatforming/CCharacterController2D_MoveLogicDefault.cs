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

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

[CreateAssetMenu(menuName = "StrixSO/" + nameof(CCharacterController2D_MoveLogicDefault))]
public class CCharacterController2D_MoveLogicDefault : CCharacterController2D_LogicBase
{
    [Rename_Inspector("걷기 속도")]
    public float p_fSpeed_OnWalking = 40f;
    [Rename_Inspector("달리기 속도")]
    public float p_fSpeed_OnRunning = 80f;
    [Rename_Inspector("최대 걷기 속도까지 걸리는 시간")]
    public float p_fTimeToMaxSpeedApex = 0.5f;
    [Rename_Inspector("걷기로 도달하는 최대 Move Delta (1이 달리기)")] [UnityEngine.Range(0, 1)]
    public float p_fMaxMoveDelta_OnWalking = 0.5f;
    [Rename_Inspector("바라보는 방향 기준을 인풋이 아닌 속도를 기준으로 할 것인지")]
    public bool p_bLookAtVelocity = false;
    [Rename_Inspector("앞에 막힌 벽을 향해 움직일 때 움직이는 애니메이션을 할것인지")]
    public bool p_bIsPlayAnimation_OnForwardIsBlock = false;

    [Space(10)]
    [Header("바닥에 붙는 옵션")]
    [Rename_Inspector("바닥에 붙는 옵션을 사용할 것인지")]
    public bool p_bUseAttachGround = true;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf("p_bUseAttachGround")]
#endif
    [Rename_Inspector("바닥 감지 레이 Y 위치 오프셋")]
    public float p_fGroundCheck_RayOrigin = -3.025f;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf("p_bUseAttachGround")]
#endif
    [Rename_Inspector("바닥 감지 레이 길이")]
    public float p_fGroundCheck_RayDistance = 2f;

    [Space(10)]
    [Header("제동 옵션")]
    [Rename_Inspector("제동 시 천천히 멈추기 유무")]
    public bool p_bIsSmoothMoveStop = false;
    [Rename_Inspector("제동 시간")]
    public float p_fTimeToStop = 0.1f;

    protected CapsuleCollider2D _pColliderGround;
    protected LayerMask _sLayerTerrain;

    // =========================================================================================

    public void DoMove(float fMoveAmount, bool bInput_Crouch, bool bInput_Run, float fCrouchSpeed, ref float fMoveVelocity)
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
        if (_pCharacterController2D.p_bMoveIsLock)
            return;

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

    public bool AttachGround(out Collider2D pColliderGround)
    {
        pColliderGround = null;

        if (p_bUseAttachGround == false)
            return false;

        if (_pCharacterController2D.p_bIsClimbingLadder)
            return false;

        RaycastHit2D pHit = GetGroundHit();
        if (pHit)
        {
            Vector3 vecPos = transform.position;
            vecPos.y -= (GetRayOriginPosition() - pHit.point).y;
            transform.position = vecPos;
        }

        return pHit;
    }

    // ==================================================================================================

    protected override void OnInit(CCharacterController2D pCharacterController2D)
    {
        base.OnInit(pCharacterController2D);

        _pColliderGround = pCharacterController2D.p_pCollider_GroundChecker;
        _sLayerTerrain = pCharacterController2D.p_sWhatIsTerrain;
    }

    public override void OnDrawGizmo(bool bIsDebuging)
    {
        base.OnDrawGizmo(bIsDebuging);

        if (bIsDebuging == false || p_bUseAttachGround == false)
            return;

        var pHit = GetGroundHit();
        if (pHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(GetRayOriginPosition(), pHit.point);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(GetRayOriginPosition(), GetRayOriginPosition() + (Vector2.down * p_fGroundCheck_RayDistance));
        }
    }

    // ==================================================================================================

    private RaycastHit2D GetGroundHit()
    {
        return Physics2D.Raycast(GetRayOriginPosition(), Vector3.down, p_fGroundCheck_RayDistance, _sLayerTerrain);
    }

    private Vector2 GetRayOriginPosition()
    {
        return _pColliderGround.transform.position + new Vector3(0f, p_fGroundCheck_RayOrigin, 0f);
    }

    private Vector2 MultiplyVector(Vector2 vecA, Vector2 vecB)
    {
        return new Vector2(vecA.x * vecB.x, vecA.y * vecB.y);
    }
}