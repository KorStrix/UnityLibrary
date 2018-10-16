#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-16 오전 10:33:39
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPathFindingAgent : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [HideInInspector]
    public static CPathfinding _pathfindingManagerScript;

    /* protected & private - Field declaration         */

    private CPlatformerController _characterScript;
    private CPlatformerCalculator _controller;
    private CPlatformerAIController _aiControllerScript;

    public GameObject pathfindingTarget; /*Following / Chasing - overrides Vector3 Pathfinding*/

    public float followDistance = 0.5f; //if follow target is this distance away, we start following
    public bool debugBool = false; /*Very expensive if multiple characters have this enabled*/
    public bool drawPath = false; /*Very expensive if multiple characters have this enabled*/
    public Color drawColor = Color.red;
    public bool endDrawComplete = true;

    //Ensures starting position is grounded at the correct location.
    private bool useStored = false;
    private Vector3 storePoint;
    // private GameObject storeObject;

    //Pathfinding
    public bool repathOnFail = true;

    private LineRenderer pathLineRenderer;
    private int orderNum = -1;
    private List<CPathfinding.instructions> currentOrders = new List<CPathfinding.instructions>();
    private List<CPathfinding.instructions> waitingOrders = null;
    private Vector3 lastOrder;
    private bool pathIsDirty = false;
    private float oldDistance;
    private int newPathAttempts = 3;
    private int newPathAttemptCount = 0;
    private int failAttempts = 3;
    private float failAttemptCount = 0;

    //Timers
    public float followPathTimer = 0.5f;
    private float fFollowPathTimer;
    public float pathFailTimer = 0.25f;
    private float fPathFailTimer;
    private float oneWayTimer = 0.1f;
    private float fOneWayTimer;

    //AI
    [HideInInspector]
    public float lastPointRandomAccuracy = 0.2f;
    public static float pointAccuracy = 0.18f;

    [HideInInspector]
    public bool pathCompleted = true;
    [HideInInspector]
    public bool isPathFinding = false;

    private bool stopPathing = true;
    private bool hasLastOrder = false;
    private bool aiJumped = false;
    private bool onewayDropDown = false;
    private bool onewayGrounded = false;
    //private bool usedPortal = false;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    //Used for refreshing paths, example: Flee behaviour 
    public int GetNodesFromCompletion()
    {
        if (currentOrders == null) { return 0; }
        int r = currentOrders.Count - orderNum;
        return r;
    }

    //Request path towards Vector3
    public void RequestPath(Vector3 pathVector)
    {
        if (_controller.p_pCollisionInfo.below)
        {
            useStored = false;
            if (debugBool)
                Debug.Log("requeseting path vector");

            lastOrder = pathVector;
            _pathfindingManagerScript.RequestPathInstructions(gameObject, lastOrder, false, _characterScript.p_fMaxJumpHeight
                , true
                , true
                , false
                , _characterScript.p_ePlatformerState_Current == ECharacterControllerState.Falling
                , false
                );
        }
        else
        {
            useStored = true;
            storePoint = pathVector;
        }
    }

    //Request path towards GameObject
    public void RequestPath(GameObject Go)
    {
        if (Go == null) return;

        pathfindingTarget = Go;
        if (_controller.p_pCollisionInfo.below)
        {
            if (debugBool)
                Debug.Log("requeseting path target");

            _pathfindingManagerScript.RequestPathInstructions(gameObject, pathfindingTarget.transform.position, false, _characterScript.p_fMaxJumpHeight
                , true
                , true
                , false
                , _characterScript.p_ePlatformerState_Current == ECharacterControllerState.Falling
                , false
                );
        }
    }

    //Callback from Thread with path information
    public void ReceivePathInstructions(List<CPathfinding.instructions> instr, bool passed)
    {

        //Passed == false means incompleted / failure to reach node destination
        if (!passed) { PathNotFound(); return; }
        waitingOrders = instr; //Storage for the path until we're ready to use it
    }

    //AI Movement /*TODO: Cleanup!*/
    public void AiMovement(ref Vector3 velocity, ref Vector2 input, ref bool jumpRequest)
    {
        bool orderComplete = false;
        if (!stopPathing && currentOrders != null && orderNum < currentOrders.Count)
        {
            if (currentOrders[orderNum].order != CPathfinding.EPathNodeType.jump) { input.x = transform.position.x > currentOrders[orderNum].pos.x ? -1 : 1; }
            if (currentOrders[orderNum].order == CPathfinding.EPathNodeType.climb) { input.y = transform.position.y > currentOrders[orderNum].pos.y ? -1 : 1; }

            //prevent overshooting jumps and moving backwards & overcorrecting
            if (orderNum - 1 > 0 && (currentOrders[orderNum - 1].order == CPathfinding.EPathNodeType.jump || currentOrders[orderNum - 1].order == CPathfinding.EPathNodeType.fall) && transform.position.x + 0.18f > currentOrders[orderNum].pos.x &&
                transform.position.x - pointAccuracy < currentOrders[orderNum].pos.x)
            {
                velocity.x = 0f;
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, currentOrders[orderNum].pos.x, 0.2f), transform.position.y, transform.position.z);
            }

            //climbing
            //if (currentOrders[orderNum].order == "climb" && transform.position.x + pointAccuracy > currentOrders[orderNum].pos.x && transform.position.x - pointAccuracy < currentOrders[orderNum].pos.x)
            //{
            //    //if last node is ground node that was switched to climbing, path completed
            //    if (orderNum == currentOrders.Count - 1 && !_characterScript.ladder.isClimbing) { orderComplete = true; }
            //    if (_characterScript.ladder.isClimbing) { input.x = 0; }
            //    if (transform.position.y + pointAccuracy > currentOrders[orderNum].pos.y && transform.position.y - pointAccuracy < currentOrders[orderNum].pos.y)
            //    {

            //        if (orderNum == currentOrders.Count - 1) { orderComplete = true; }

            //        if (orderNum + 1 < currentOrders.Count && currentOrders[orderNum + 1].order == "climb")
            //        {
            //            orderComplete = true;
            //        }
            //    }
            //    if (orderNum + 1 < currentOrders.Count && currentOrders[orderNum + 1].order != "climb")
            //    {
            //        input.y = currentOrders[orderNum + 1].pos.y < currentOrders[orderNum].pos.y ? -1 : 1;
            //        if (!_characterScript.ladder.isClimbing)
            //        {
            //            orderComplete = true;
            //        }

            //    }
            //}

            //match X position of node (Ground, Fall)
            if (currentOrders[orderNum].order != CPathfinding.EPathNodeType.jump && currentOrders[orderNum].order != CPathfinding.EPathNodeType.climb
                && transform.position.x + pointAccuracy > currentOrders[orderNum].pos.x
                && transform.position.x - pointAccuracy < currentOrders[orderNum].pos.x)
            {
                input.x = 0f;
                if (transform.position.y + 0.866f > currentOrders[orderNum].pos.y
                && transform.position.y - 0.866f < currentOrders[orderNum].pos.y)
                {
                    //if next node is a jump, remove velocity.x, and lerp position to point.
                    if (orderNum + 1 < currentOrders.Count && currentOrders[orderNum + 1].order == CPathfinding.EPathNodeType.jump)
                    {
                        velocity.x *= 0.0f;
                        transform.position = new Vector3(Mathf.Lerp(transform.position.x, currentOrders[orderNum].pos.x, 0.2f), transform.position.y, transform.position.z);
                    }
                    //if last node was a jump, and next node is a fall, remove velocity.x, and lerp position to point
                    if (orderNum + 1 < currentOrders.Count && orderNum - 1 > 0 && currentOrders[orderNum + 1].order == CPathfinding.EPathNodeType.fall && currentOrders[orderNum + -1].order == CPathfinding.EPathNodeType.jump)
                    {
                        velocity.x *= 0.0f;
                        transform.position = new Vector3(Mathf.Lerp(transform.position.x, currentOrders[orderNum].pos.x, 0.5f), transform.position.y, transform.position.z);
                    }
                    //   ******TEMP FIX FOR fall nodes on oneways
                    if (currentOrders[orderNum].order == CPathfinding.EPathNodeType.fall)
                    {
                        input.y = -1;
                    }


                    if (currentOrders[orderNum].order != CPathfinding.EPathNodeType.portal)
                    {
                        orderComplete = true;
                    }

                }
            }
            //Jump
            if (currentOrders[orderNum].order == CPathfinding.EPathNodeType.jump && !aiJumped && _controller.p_pCollisionInfo.below)
            {
                //velocity.y = characterScript.jumpVelocity;
                jumpRequest = true;
                aiJumped = true;
                if (orderNum + 1 < currentOrders.Count && Mathf.Abs(currentOrders[orderNum + 1].pos.x - currentOrders[orderNum].pos.x) > 1f)
                {
                    orderComplete = true;
                    aiJumped = false;
                }
            }
            else if (aiJumped && transform.position.y + 1f > currentOrders[orderNum].pos.y && transform.position.y - 1f < currentOrders[orderNum].pos.y)
            {
                orderComplete = true;
                aiJumped = false;
            }

            //oneway nodes
            if (orderNum > 0 && (currentOrders[orderNum - 1].order == CPathfinding.EPathNodeType.fall || currentOrders[orderNum - 1].order == CPathfinding.EPathNodeType.jump))
            {
                if (_controller.p_pCollisionInfo.below)
                {

                    onewayGrounded = true;
                    if (onewayGrounded && onewayDropDown)
                    {
                        input.y = -1;
                        onewayGrounded = false;
                    }

                }
            }

            //next order!
            if (orderComplete)
            {

                orderNum++;

                onewayGrounded = false; //oneway detection
                onewayDropDown = false;
                fOneWayTimer = 0;

                if (orderNum < currentOrders.Count - 1)
                { //used for DirtyPath
                    oldDistance = Vector3.Distance(transform.position, currentOrders[orderNum].pos);
                }

                if (orderNum >= currentOrders.Count)
                {

                    velocity.x = 0;
                    //Carry out orders when the node is finally reached...
                    PathCompleted();
                }
            }
        }
    }

    public void CancelPathing()
    {
        if (debugBool) { Debug.Log("path canceled"); }
        if (endDrawComplete && pathLineRenderer) { pathLineRenderer.positionCount = (1); }
        //Remove orders && Prevent pathfinding
        hasLastOrder = false;
        currentOrders = null;
        isPathFinding = false;
        stopPathing = true;
        //Your Code: OR keep custom AI in the AI-CONTROLLER
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        if (_pathfindingManagerScript == null) { _pathfindingManagerScript = FindObjectOfType<CPathfinding>(); }
        _aiControllerScript = GetComponent<CPlatformerAIController>();
        _characterScript = GetComponent<CPlatformerController>();
        _controller = GetComponent<CPlatformerCalculator>();
        if (drawPath)
        {
            AddLineRenderer();
        }
    }

    //Applying new paths when character is ready
    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        if (useStored)
        {
            RequestPath(storePoint);
        }

        //Only recieve orders if we're grounded or on a ladder, so we don't accidentally fall off a ledge mid-jump.
        if (waitingOrders != null && (_controller.p_pCollisionInfo.below))
        {
            if (_aiControllerScript.NeedsPathfinding())
            {

                isPathFinding = true;
                currentOrders = waitingOrders;
                waitingOrders = null;
                pathCompleted = false;
                stopPathing = false;
                if (!pathfindingTarget)
                {
                    hasLastOrder = true;
                }

                newPathAttemptCount = 0;
                orderNum = 0;

                failAttemptCount = 0;
                if (currentOrders != null && orderNum < currentOrders.Count - 1)
                { //used for DirtyPath
                    oldDistance = Vector3.Distance(transform.position, currentOrders[orderNum].pos);
                }
                //If character is nowhere near starting node, we try to salvage the path by picking the nearest node and setting it as the start.
                if (Vector3.Distance(transform.position, currentOrders[0].pos) > 2f)
                {
                    float closest = float.MaxValue;
                    for (int i = 0; i < currentOrders.Count; i++)
                    {
                        float distance = Vector3.Distance(currentOrders[i].pos, transform.position);
                        if ((currentOrders[i].order == CPathfinding.EPathNodeType.walkable || currentOrders[i].order == CPathfinding.EPathNodeType.climb) && distance < closest)
                        {
                            closest = distance;
                            orderNum = i;
                        }
                    }
                }
                //If possible, we skip the first node, this prevents that character from walking backwards to first node.
                if (currentOrders.Count > orderNum + 1 && currentOrders[orderNum].order == currentOrders[orderNum + 1].order &&
                    (currentOrders[orderNum].order == CPathfinding.EPathNodeType.walkable || currentOrders[orderNum].order == CPathfinding.EPathNodeType.climb))
                {
                    orderNum += 1;
                }
                //Add Random deviation to last node position to stagger paths (Staggers character positions / Looks better.)
                if (currentOrders.Count - 1 > 0 && currentOrders[currentOrders.Count - 1].order == CPathfinding.EPathNodeType.walkable)
                {
                    currentOrders[currentOrders.Count - 1].pos.x += Random.Range(-1, 1) * lastPointRandomAccuracy;
                }

                PathStarted();
            }
        }
    }

    //Requesting new path timers
    private void FixedUpdate()
    {

        if (onewayGrounded)
        {
            fOneWayTimer += Time.deltaTime;
            if (fOneWayTimer >= oneWayTimer)
            {
                onewayDropDown = true;
                fOneWayTimer = 0;
            }
        }

        //Update Follow/Chase Path
        if (pathfindingTarget)
        {
            fFollowPathTimer += Time.deltaTime;
            if (fFollowPathTimer >= followPathTimer)
            {
                fFollowPathTimer = 0f;
                if ((currentOrders != null && currentOrders.Count > 0 && currentOrders.Count > 0 && Vector3.Distance(currentOrders[currentOrders.Count - 1].pos, pathfindingTarget.transform.position) > followDistance)
                    || ((currentOrders == null || currentOrders.Count == 0)))
                {
                    if (Vector3.Distance(transform.position, pathfindingTarget.transform.position) > followDistance + 0.18f)
                        pathIsDirty = true;
                }
            }
        }
        //Unable to make progress on current path, Update Path

        if (repathOnFail && !pathCompleted)
        {
            fPathFailTimer += Time.deltaTime;
            if (fPathFailTimer > pathFailTimer)
            {

                fPathFailTimer = 0;
                if (currentOrders != null && currentOrders.Count > orderNum)
                {
                    float newDistance = Vector3.Distance(transform.position, currentOrders[orderNum].pos);
                    if (oldDistance <= newDistance)
                    {
                        failAttemptCount++;
                        if (failAttemptCount >= failAttempts && _controller.p_pCollisionInfo.below)
                        {
                            failAttemptCount = 0;
                            pathIsDirty = true;
                        }
                    }
                    else { failAttemptCount = 0; }
                    oldDistance = newDistance;
                }
            }
        }
        //If path is dirty, request new path;
        if (pathIsDirty)
        {
            pathIsDirty = false;
            if (pathfindingTarget) { RequestPath(pathfindingTarget); } else if (hasLastOrder) { RequestPath(lastOrder); }
            if (debugBool)
            {
                Debug.Log("path is dirty");
            }
        }
    }

    //Debugging visuals
    private void OnDrawGizmos()
    {
        if (currentOrders != null && !pathCompleted && debugBool)
        {
            for (int i = 0; i < currentOrders.Count; i++)
            {

                if (i == orderNum)
                {
                    Gizmos.color = Color.cyan;
                }
                else
                {
                    if (i == 0) { Gizmos.color = Color.green; }
                    else if (i == currentOrders.Count - 1) { Gizmos.color = Color.red; }
                    else
                    {
                        Gizmos.color = Color.gray;
                    }
                }
                Gizmos.DrawSphere(currentOrders[i].pos, 0.11f);
                if (i + 1 < currentOrders.Count)
                {
                    if (i - 1 == orderNum || i == orderNum) { Gizmos.color = Color.red; } else { Gizmos.color = Color.gray; }

                    Gizmos.DrawLine(currentOrders[i].pos, currentOrders[i + 1].pos);
                }
            }
        }
    }


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

#region Private

    private void PathStarted()
    {
        if (debugBool) { Debug.Log("path started"); }

        if (drawPath)
        {
            if (!pathLineRenderer) { AddLineRenderer(); }
            pathLineRenderer.startColor = (drawColor);
            pathLineRenderer.endColor = (drawColor);
            pathLineRenderer.positionCount = (currentOrders.Count);
            for (int i = 0; i < currentOrders.Count; i++)
            {

                pathLineRenderer.SetPosition(i, new Vector3(currentOrders[i].pos.x, currentOrders[i].pos.y, 0));
            }

        }
        if (!drawPath && pathLineRenderer) { Destroy(gameObject.GetComponent<LineRenderer>()); }
        //Path has started
        //Your Code: OR keep custom AI in the AI-CONTROLLER

        _aiControllerScript.PathStarted();
    }

    private void PathCompleted()
    {
        if (debugBool) { Debug.Log("path completed"); }
        if (!drawPath && pathLineRenderer) { Destroy(gameObject.GetComponent<LineRenderer>()); }
        CancelPathing(); //Reset Variables && Clears the debugging gizmos from drawing
        //Path was completed
        //Your Code: OR keep custom AI in the AI-CONTROLLER
        _aiControllerScript.PathCompleted();
    }

    private void PathNotFound()
    {
        if (debugBool) { Debug.Log("path not found"); }
        newPathAttemptCount++;
        if (newPathAttemptCount >= newPathAttempts)
        {
            CancelPathing(); if (debugBool) { Debug.Log("newpath attempt limit reached. cancelling path."); }
        }
        //TODO: Attempt to get as close as possible to our goal even though no path was found?

        //No Path Found
        //Your Code: OR keep custom AI in the AI-CONTROLLER
    }

    private void AddLineRenderer()
    {

        if (!GetComponent<LineRenderer>())
        {
            pathLineRenderer = gameObject.AddComponent<LineRenderer>();
            pathLineRenderer.materials[0] = (Material)Resources.Load("Sprite/Default", typeof(Material));
            pathLineRenderer.materials[0].shader = Shader.Find("Sprites/Default");
            pathLineRenderer.positionCount = (1);
            pathLineRenderer.startWidth = (0.1f);
            pathLineRenderer.endWidth = (0.1f);
        }
    }

#endregion Private
}