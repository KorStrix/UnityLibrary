#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-07-31 오전 10:39:46
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

public class CPlatformControllerSimple : CRaycastCalculator
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }
    /* public - Field declaration            */

    public bool p_bIsDebugMode = false;
    public LayerMask passengerMask;

    /* protected & private - Field declaration         */

    Dictionary<Transform, CPlatformerCalculator> passengerDictionary = new Dictionary<Transform, CPlatformerCalculator>();
    List<PassengerMovement> passengerMovement;

    Vector3 _vecPrevPos;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    //void OnDrawGizmos()
    //{
    //    if (globalWaypoints != null)
    //    {
    //        Gizmos.color = Color.red;
    //        float size = .3f;

    //        for (int i = 0; i < globalWaypoints.Length; i++)
    //        {
    //            Gizmos.DrawLine(globalWaypoints[i] - Vector3.up * size, globalWaypoints[i] + Vector3.up * size);
    //            Gizmos.DrawLine(globalWaypoints[i] - Vector3.left * size, globalWaypoints[i] + Vector3.left * size);
    //        }
    //    }
    //}

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        _vecPrevPos = transform.position;
    }

    //public override void OnUpdate(ref bool bCheckUpdateCount)
    //{
    //    base.OnUpdate(ref bCheckUpdateCount);

    //    DoUpdateRaycastOrigins();

    //    Vector3 velocity = CalculatePlatformMovement();

    //    if (velocity.Equals(Vector3.zero))
    //        return;

    //    if (p_bIsDebugMode)
    //        Debug.Log(name + " velocity : " + velocity);

    //    CalculatePassengerMovement(velocity);
    //    MovePassengers(true);
    //}

    private void FixedUpdate()
    {
        DoUpdateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();

        if (velocity.Equals(Vector3.zero))
            return;

        if (p_bIsDebugMode)
            Debug.Log(name + " velocity : " + velocity);

        CalculatePassengerMovement(velocity);
        MovePassengers(true);
        MovePassengers(false);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    Vector3 CalculatePlatformMovement()
    {
        Vector3 vecPos = _vecPrevPos;
        _vecPrevPos = transform.position;

        return transform.position - vecPos;
    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach (PassengerMovement passenger in passengerMovement)
        {
            if (!passengerDictionary.ContainsKey(passenger.transform))
            {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<CPlatformerCalculator>());
            }

            if (p_bIsDebugMode)
                Debug.Log(" MovePassengers beforeMovePlatform : " + beforeMovePlatform + " passenger.velocity : " + passenger.velocity + " passenger.standingOnPlatform : " + passenger.standingOnPlatform + " passenger.moveBeforePlatform : " + passenger.moveBeforePlatform);

            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                passengerDictionary[passenger.transform].DoMove(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        // Vertically moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + _fSkinWidth_Vertical;

            for (int i = 0; i < _iVerticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? _pRaycastOrigins.vecBound_BottomLeft : _pRaycastOrigins.vecBound_TopLeft;
                rayOrigin += Vector2.right * (_fDstBetweenRays_Vertical * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                if (p_bIsDebugMode)
                    Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - _fSkinWidth_Vertical) * directionY;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }
        }

        // Horizontally moving platform
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + _fSkinWidth_Horizontal;

            for (int i = 0; i < _iHorizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? _pRaycastOrigins.vecBound_BottomLeft : _pRaycastOrigins.vecBound_BottomRight;
                rayOrigin += Vector2.up * (_fDstBetweenRays_Horizontal * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                if (p_bIsDebugMode)
                    Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - _fSkinWidth_Horizontal) * directionX;
                        float pushY = -_fSkinWidth_Horizontal;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }

        // Passenger on top of a horizontally or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = _fSkinWidth_Vertical * 2;

            for (int i = 0; i < _iVerticalRayCount; i++)
            {
                Vector2 rayOrigin = _pRaycastOrigins.vecBound_TopLeft + Vector2.right * (_fDstBetweenRays_Vertical * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                if (p_bIsDebugMode)
                    Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.red);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y + velocity.y - (hit.distance - _fSkinWidth_Vertical) * directionY;
                        //float pushY = velocity.y - (hit.distance - _fSkinWidth_Vertical) * directionY;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
    }

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test