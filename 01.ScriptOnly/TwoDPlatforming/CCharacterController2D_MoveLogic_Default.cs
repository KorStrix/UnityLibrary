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

[CreateAssetMenu(menuName = "StrixSO/CharacterController2D/" + nameof(CCharacterController2D_MoveLogic_Default))]
public class CCharacterController2D_MoveLogic_Default : CCharacterController2D_MoveLogicBase
{
    public override bool AttachGround(out Collider2D pColliderGround)
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
}