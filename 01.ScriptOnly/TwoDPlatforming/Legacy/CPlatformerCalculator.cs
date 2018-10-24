#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-12 오후 12:29:48
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPlatformerCalculator : CRaycastCalculator
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum ECollisionIgnoreType
    {
        None,
        Though_Up,
        Though_Down,
    }

    [System.Serializable]
    public class CollisionInfo
    {
        public bool above;
        public bool below { get; private set; }
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope { get; private set; }

        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public Vector2 moveAmountOld;
        public int iFaceDir_OneIsLeft { get; private set; }
        public bool fallingThroughPlatform;

        [HideInInspector]
        public List<Transform> _listHitTransform = new List<Transform>();

        [HideInInspector]
        public List<RaycastHit2D> _listHit_Vertical = new List<RaycastHit2D>();

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            DoSet_MaxSlope(false);
            slopeNormal = Vector2.zero;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;

            _listHitTransform.Clear();
        }

        System.Action<bool> _OnChangeBelow;
        System.Action<int> _OnChangeFaceDir;
        System.Action<bool> _OnChangeSlopeSliding;

        public void DoSet_Below(bool bBelow)
        {
            if (below != bBelow)
            {
                below = bBelow;
                _OnChangeBelow(bBelow);
            }
        }

        public void DoSet_Below(System.Action<bool> OnChangeBelow)
        {
            _OnChangeBelow = OnChangeBelow;
        }

        public void DoSet_OnChangeSlopeSliding(System.Action<bool> OnChangeSlopeSliding)
        {
            this._OnChangeSlopeSliding = OnChangeSlopeSliding;
        }

        public void DoSet_OnChangeFaceDir(System.Action<int> OnChangeFaceDir)
        {
            this._OnChangeFaceDir = OnChangeFaceDir;
        }

        public void DoSet_MaxSlope(bool bSliding)
        {
            if (slidingDownMaxSlope != bSliding)
            {
                if (_OnChangeSlopeSliding != null)
                    _OnChangeSlopeSliding(bSliding);

                slidingDownMaxSlope = bSliding;
            }
        }

        public void DoSetFaceDir_OneIsLeft(int iFaceDir)
        {
            if (iFaceDir_OneIsLeft != iFaceDir)
            {
                if (_OnChangeFaceDir != null)
                    _OnChangeFaceDir(iFaceDir);

                iFaceDir_OneIsLeft = iFaceDir;
            }
        }
    }

    /* public - Field declaration            */

    public delegate void OnHit_VerticalCollider(Transform pTransformHit, out ECollisionIgnoreType eIgnnore_ThisHitInfo);

    public bool bDebugMode;

    public LayerMask collisionMask;

    //[Rename_Inspector("레이 원점 X 오프셋")]
    //public float _fRayOriginOffset_X = 0f;
    //[Rename_Inspector("레이 원점 Y 오프셋")]
    //public float _fRayOriginOffset_Y = 0f;
    [Rename_Inspector("처음 바라보는 방향")]
    public int p_iFaceDir_OnAwake = 1;
    public float maxSlopeAngle = 80;

    public CollisionInfo p_pCollisionInfo;
    [HideInInspector]
    public Vector2 _vecPlayerInput;

    public Vector3 p_vecMoveAmountOrigin { get; private set; }
    public Vector3 p_vecMoveAmountResult { get; private set; }

    public int _iHorizontalHitCount { get; private set; }
    public int _iVerticalHitCount { get; private set; }

    /* protected & private - Field declaration         */

    //OnHit_VerticalCollider _OnHit_VerticalCollider;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoSet_CalcaulateVerticalCollider(OnHit_VerticalCollider OnHit_VerticalCollider)
    {
        //_OnHit_VerticalCollider = OnHit_VerticalCollider;
    }

    public void DoSet_OnChangeFaceDir(System.Action<int> OnChangeFaceDir)
    {
        p_pCollisionInfo.DoSet_OnChangeFaceDir(OnChangeFaceDir);
    }
    
    public void DoSet_OnChangeSlopeSliding(System.Action<bool> OnChangeSlopeSliding)
    {
        p_pCollisionInfo.DoSet_OnChangeSlopeSliding(OnChangeSlopeSliding);
    }

    public void DoSet_OnChangeBelow(System.Action<bool> OnChangeBelow)
    {
        p_pCollisionInfo.DoSet_Below(OnChangeBelow);
    }

    public void DoMove(Vector2 moveAmount, bool bPlatform)
    {
        DoMove(moveAmount, Vector3.zero, bPlatform);
    }

    public void DoMove(Vector2 moveAmount, Vector2 vecInput, bool standingOnPlatform = false)
    {
        DoUpdateRaycastOrigins();

        p_pCollisionInfo.Reset();
        p_pCollisionInfo.moveAmountOld = moveAmount;
        p_vecMoveAmountOrigin = moveAmount;
        _vecPlayerInput = vecInput;

        if (moveAmount.y < 0)
            DescendSlope(ref moveAmount);

        if (moveAmount.x != 0)
            p_pCollisionInfo.DoSetFaceDir_OneIsLeft(System.Math.Sign(moveAmount.x));

        HorizontalCollision(ref moveAmount);
        //if (moveAmount.y != 0)
        {
            VerticalCollision(ref moveAmount);
        }

        p_vecMoveAmountResult = moveAmount;
        transform.Translate(moveAmount);

        if (standingOnPlatform)
        {
            p_pCollisionInfo.DoSet_Below(true);
        }
    }

    public void DoIgnoreCollider(float fRestoreSeconds)
    {
        p_pCollisionInfo.fallingThroughPlatform = true;
        Invoke("ResetFallingThroughPlatform", fRestoreSeconds);
    }
    
    public bool DoCheckBelow(out RaycastHit2D pHitBelow)
    {
        pHitBelow = default(RaycastHit2D);
        float directionY = -1f;
        float rayLength = Mathf.Abs(p_vecMoveAmountOrigin.y) + _fSkinWidth_Vertical;

        for (int i = 0; i < _iVerticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? _pRaycastOrigins.vecBound_BottomLeft : _pRaycastOrigins.vecBound_TopLeft;
            rayOrigin += Vector2.right * (_fDstBetweenRays_Vertical * i);
            if (i == _iVerticalRayCount - 1)
                rayOrigin.x = ((directionY == -1) ? _pRaycastOrigins.vecBound_BottomRight.x : _pRaycastOrigins.vecBound_TopRight.x);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            if (bDebugMode)
            {
                if (hit)
                    Debug.DrawRay(rayOrigin, Vector2.up * directionY * hit.distance, Color.magenta);
                else
                    Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.cyan);
            }

            if (hit)
            {
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1/* || hit.distance == 0*/)
                    {
                        continue;
                    }
                    if (p_pCollisionInfo.fallingThroughPlatform)
                    {
                        continue;
                    }
                    if (_vecPlayerInput.y == -1)
                    {
                        p_pCollisionInfo.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", .5f);
                        continue;
                    }
                }

                pHitBelow = hit;
                return true;
            }
        }

        return false;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        p_pCollisionInfo.DoSetFaceDir_OneIsLeft(p_iFaceDir_OnAwake);
    }

#if UNITY_EDITOR
    Vector3 vecDebugOffset = new Vector2(1f, 1f);

    private void OnDrawGizmos()
    {
        if (bDebugMode == false) return;

        Vector3 vecPos = transform.position + vecDebugOffset;
        UnityEditor.Handles.Label(vecPos, "collisions.climbingSlope : " + p_pCollisionInfo.climbingSlope);

        vecPos.y -= 1;
        UnityEditor.Handles.Label(vecPos, "_pCollisionInfo.slidingDownMaxSlope : " + p_pCollisionInfo.slidingDownMaxSlope);

        vecPos.y -= 1;
        UnityEditor.Handles.Label(vecPos, "_iHorizontalHitCount : " + _iHorizontalHitCount);

        vecPos.y -= 1;
        UnityEditor.Handles.Label(vecPos, "_iVerticalHitCount : " + _iVerticalHitCount);

        vecPos.y -= 1;
        UnityEditor.Handles.Label(vecPos, "_vecPlayerInput : " + _vecPlayerInput.ToString("F4"));

        vecPos.y -= 1;
        UnityEditor.Handles.Label(vecPos, "_vecMoveAmountOrigin : " + p_vecMoveAmountOrigin.ToString("F4"));

        vecPos.y -= 1;
        UnityEditor.Handles.Label(vecPos, "_vecMoveAmountResult : " + p_vecMoveAmountResult.ToString("F4"));

        vecPos.y -= 1;
        UnityEditor.Handles.Label(vecPos, "p_pCollisionInfo.right : " + p_pCollisionInfo.right + " p_pCollisionInfo.left : " + p_pCollisionInfo.left);

        vecPos.y -= 1;
        UnityEditor.Handles.Label(vecPos, "p_pCollisionInfo.below : " + p_pCollisionInfo.below + " p_pCollisionInfo.above : " + p_pCollisionInfo.above);

    }
#endif

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    void HorizontalCollision(ref Vector2 moveAmount)
    {
        float directionX = p_pCollisionInfo.iFaceDir_OneIsLeft;
        float rayLength = Mathf.Abs(moveAmount.x) + _fSkinWidth_Horizontal;

        if (Mathf.Abs(moveAmount.x) < _fSkinWidth_Horizontal)
        {
            rayLength = 2 * _fSkinWidth_Horizontal;
        }

        _iHorizontalHitCount = 0;
        for (int i = 0; i < _iHorizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? _pRaycastOrigins.vecBound_BottomLeft : _pRaycastOrigins.vecBound_BottomRight;
            rayOrigin += Vector2.up * (_fDstBetweenRays_Horizontal * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (bDebugMode)
            {
                if (hit)
                    Debug.DrawRay(rayOrigin, Vector2.right * directionX * hit.distance, Color.red);
                else
                    Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.green);
            }

            if (hit)
            {
                if (hit.distance == 0)
                    continue;

                p_pCollisionInfo._listHitTransform.Add(hit.transform);
                _iHorizontalHitCount++;

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (p_pCollisionInfo.descendingSlope)
                    {
                        p_pCollisionInfo.descendingSlope = false;
                        moveAmount = p_pCollisionInfo.moveAmountOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != p_pCollisionInfo.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - _fSkinWidth_Horizontal;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!p_pCollisionInfo.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    moveAmount.x = (hit.distance - _fSkinWidth_Horizontal) * directionX;
                    rayLength = hit.distance;

                    if (p_pCollisionInfo.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(p_pCollisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    p_pCollisionInfo.left = directionX == -1;
                    p_pCollisionInfo.right = directionX == 1;
                }
            }
        }
    }


    void VerticalCollision(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + _fSkinWidth_Vertical;

        _iVerticalHitCount = 0;
        for (int i = 0; i < _iVerticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? _pRaycastOrigins.vecBound_BottomLeft : _pRaycastOrigins.vecBound_TopLeft;
            rayOrigin += Vector2.right * (_fDstBetweenRays_Vertical * i + moveAmount.x);
            if (i == _iVerticalRayCount - 1)
                rayOrigin.x = ((directionY == -1) ? _pRaycastOrigins.vecBound_BottomRight.x : _pRaycastOrigins.vecBound_TopRight.x) + moveAmount.x;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            if(bDebugMode)
            {
                if(hit)
                    Debug.DrawRay(rayOrigin, Vector2.up * directionY * hit.distance, Color.red);
                else
                    Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.green);
            }

            if (hit)
            {
                //if (hit.collider.tag == "Through")
                //{
                //    if (directionY == 1/* || hit.distance == 0*/)
                //    {
                //        continue;
                //    }
                //    if (p_pCollisionInfo.fallingThroughPlatform)
                //    {
                //        continue;
                //    }
                //    if (_vecPlayerInput.y == -1)
                //    {
                //        p_pCollisionInfo.fallingThroughPlatform = true;
                //        Invoke("ResetFallingThroughPlatform", .5f);
                //        continue;
                //    }
                //}

                p_pCollisionInfo._listHit_Vertical.Add(hit);
                p_pCollisionInfo._listHitTransform.Add(hit.transform);
                _iVerticalHitCount++;
                moveAmount.y = (hit.distance - _fSkinWidth_Vertical) * directionY;
                rayLength = hit.distance;

                if (p_pCollisionInfo.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(p_pCollisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                p_pCollisionInfo.DoSet_Below(directionY == -1);
                p_pCollisionInfo.above = directionY == 1;
            }
        }

        if (p_pCollisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + _fSkinWidth_Vertical;
            Vector2 rayOrigin = ((directionX == -1) ? _pRaycastOrigins.vecBound_BottomLeft : _pRaycastOrigins.vecBound_BottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != p_pCollisionInfo.slopeAngle)
                {
                    moveAmount.x = (hit.distance - _fSkinWidth_Vertical) * directionX;
                    p_pCollisionInfo.slopeAngle = slopeAngle;
                    p_pCollisionInfo.slopeNormal = hit.normal;
                }
            }
        }
    }

    void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            p_pCollisionInfo.DoSet_Below(true);
            p_pCollisionInfo.climbingSlope = true;
            p_pCollisionInfo.slopeAngle = slopeAngle;
            p_pCollisionInfo.slopeNormal = slopeNormal;
        }
    }

    void DescendSlope(ref Vector2 moveAmount)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(_pRaycastOrigins.vecBound_BottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + _fSkinWidth_Vertical, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(_pRaycastOrigins.vecBound_BottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + _fSkinWidth_Vertical, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        }

        if (!p_pCollisionInfo.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? _pRaycastOrigins.vecBound_BottomRight : _pRaycastOrigins.vecBound_BottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - _fSkinWidth_Vertical <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendmoveAmountY;

                            p_pCollisionInfo.slopeAngle = slopeAngle;
                            p_pCollisionInfo.descendingSlope = true;
                            p_pCollisionInfo.DoSet_Below(true);
                            p_pCollisionInfo.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                p_pCollisionInfo.slopeAngle = slopeAngle;
                p_pCollisionInfo.DoSet_MaxSlope(true);
                p_pCollisionInfo.slopeNormal = hit.normal;
            }
        }

    }

    //void SlideDownMaxSlope(CRaycastHitWrapper pHit, ref Vector2 vecMoveAmount)
    //{
    //    if (!pHit) return;

    //    float fSlopeAngle = Vector2.Angle(pHit.normal, Vector2.up);
    //    bool bIsSlopeSliding = fSlopeAngle > maxSlopeAngle;
    //    if (bIsSlopeSliding)
    //    {
    //        vecMoveAmount.x = Mathf.Abs(vecMoveAmount.y) - (pHit.distance - _pRaycastOrigins.fRayLength_Vertical) / Mathf.Tan(fSlopeAngle * Mathf.Deg2Rad);
    //        if (Mathf.Sign(vecMoveAmount.x) != Mathf.Sign(pHit.normal.x))
    //            vecMoveAmount.x *= -1f;

    //        if(vecMoveAmount.y > 0f)
    //            vecMoveAmount.y = Mathf.Tan(p_pCollisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(vecMoveAmount.x);

    //        p_pCollisionInfo.slopeAngle = fSlopeAngle;
    //        p_pCollisionInfo.slopeNormal = pHit.normal;
    //    }

    //    p_pCollisionInfo.DoSet_MaxSlope(bIsSlopeSliding);
    //}

    void ResetFallingThroughPlatform()
    {
        p_pCollisionInfo.fallingThroughPlatform = false;
    }

    #endregion Private
}