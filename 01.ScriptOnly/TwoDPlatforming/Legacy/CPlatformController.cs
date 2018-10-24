using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPlatformController : CRaycastCalculator
{
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

    public bool p_bIsDebugMode = false;

    [Rename_Inspector("플랫폼 대상 레이어")]
    public LayerMask p_pLayerPC;

    public Vector3[] globalWaypoints;

    public float speed = 10;
    [Rename_Inspector("루프 유무")]
    public bool cyclic;
    public float waitTime;
    [Range(0, 2)]
    public float easeAmount;

    [Rename_Inspector("OnEnable 시 플레이 할 것인지")]
    public bool p_bIsPlay_OnEnable = false;

    protected int _iIndex_CurrentWayPoint;
    float percentBetweenWaypoints;
    float nextMoveTime;

    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, CPlatformerCalculator> passengerDictionary = new Dictionary<Transform, CPlatformerCalculator>();

    [Rename_Inspector("플레이 유무", false)]
    public bool p_bIsPlaying = false;

    //protected override void OnStart()
    //{
    //    base.OnStart();

    //    globalWaypoints = new Vector3[localWaypoints.Length];
    //    for (int i = 0; i < localWaypoints.Length; i++)
    //    {
    //        globalWaypoints[i] = localWaypoints[i] + transform.position;
    //    }
    //}

    public void DoPlay()
    {
        p_bIsPlaying = true;
        OnPlay();
    }

    public void DoStop()
    {
        p_bIsPlaying = false;
        OnStop();
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        if (p_bIsPlay_OnEnable)
            DoPlay();
    }

    void Update()
    {
        if (p_bIsPlaying == false)
            return;

        DoUpdateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();
        if (p_bIsDebugMode)
            Debug.Log(name + " velocity : " + velocity);

        CalculatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }


    // ========================================================================

    protected virtual void OnPlay() { }
    protected virtual void OnStop() { }

    // ========================================================================

    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    Vector3 CalculatePlatformMovement()
    {
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        _iIndex_CurrentWayPoint %= globalWaypoints.Length;
        int toWaypointIndex = (_iIndex_CurrentWayPoint + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[_iIndex_CurrentWayPoint], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[_iIndex_CurrentWayPoint], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            _iIndex_CurrentWayPoint++;

            if (!cyclic)
            {
                if (_iIndex_CurrentWayPoint >= globalWaypoints.Length - 1)
                {
                    _iIndex_CurrentWayPoint = 0;
                    //System.Array.Reverse(globalWaypoints);

                    DoStop();
                }
            }
            nextMoveTime = Time.time + waitTime;
        }

        return newPos - transform.position;
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
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, p_pLayerPC);

                if (p_bIsDebugMode)
                {
                    if (hit)
                        Debug.DrawRay(rayOrigin, Vector2.up * hit.distance, Color.green);
                    else
                        Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
                }

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
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, p_pLayerPC);

                if (p_bIsDebugMode)
                {
                    if (hit)
                        Debug.DrawRay(rayOrigin, Vector2.right * hit.distance, Color.green);
                    else
                        Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
                }

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
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, p_pLayerPC);

                if (p_bIsDebugMode)
                {
                    if (hit)
                        Debug.DrawRay(rayOrigin, Vector2.up * hit.distance, Color.green);
                    else
                        Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.red);
                }

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

    void OnDrawGizmos()
    {
        if (p_bIsDebugMode == false)
            return;

        if (globalWaypoints != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < globalWaypoints.Length - 1; i++)
            {
                Gizmos.DrawWireSphere(globalWaypoints[i], 1f);
                Gizmos.DrawLine(globalWaypoints[i], globalWaypoints[i + 1]);
            }
        }
    }

}
