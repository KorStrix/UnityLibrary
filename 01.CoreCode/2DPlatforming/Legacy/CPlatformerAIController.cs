#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-16 오전 11:19:05
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPlatformerAIController : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EAIState { none, groundpatrol, pathfinding, chase, flee, pathfindChase } /*Add custom AI states here!*/

    /* public - Field declaration            */

    public static CPathfinding _pathScript;
    public static GameObject player;

    /* protected & private - Field declaration         */


    public EAIState state = EAIState.pathfinding;

    // private CPlatformerController _characterScript;
    private CPlatformerCalculator _controller;
    private CPathFindingAgent _pathAgent;

    [HideInInspector]
    public TextMesh _behaviourText;

    private float direction = 1;
    private float fleeTimer = 0.5f;
    private float fFleeTimer;

    private bool destroy = false;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public bool NeedsPathfinding()
    {
        if (state == EAIState.pathfinding || state == EAIState.flee || state == EAIState.chase || state == EAIState.pathfindChase) { return true; }
        _pathAgent.CancelPathing();
        return false;
    }

    public void GetInput(ref Vector3 velocity, ref Vector2 input, ref bool jumpRequest)
    {
        switch (state)
        {
            case EAIState.none: break;
            case EAIState.groundpatrol: GroundPatrol(ref input); break;
            case EAIState.flee: Flee(); break;
            case EAIState.pathfindChase: PathfindChase(); break;
            case EAIState.chase: Chase(); break; //add this line in to the GetInput method
            default: break;
        }

        if (state == EAIState.pathfinding || state == EAIState.flee || state == EAIState.chase || state == EAIState.pathfindChase)
        {
            _pathAgent.AiMovement(ref velocity, ref input, ref jumpRequest);
        }
    }

    /*gets called from pathagent when character finishes navigating path*/
    public void PathCompleted()
    {
        switch (state)
        {
            case EAIState.pathfinding: _behaviourText.text = ""; break;
            case EAIState.pathfindChase: destroy = true; break; /*when character reaches house, destroy on next update*/
            case EAIState.chase: _behaviourText.text = "Chase"; break;
            case EAIState.flee: _behaviourText.text = "Flee"; break;
        }
    }

    /*gets called from pathagent when character beings navigating path*/
    public void PathStarted()
    {
        switch (state)
        {
            case EAIState.pathfinding: _behaviourText.text = "Pathfinding"; break;
            case EAIState.chase: _behaviourText.text = "Chase"; break;
            case EAIState.pathfindChase: _behaviourText.text = "Pathfinding"; break;
        }
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        _controller = GetComponent<CPlatformerCalculator>();
        //_characterScript = GetComponent<CPlatformerController>();
        _pathAgent = GetComponent<CPathFindingAgent>();

        if (_pathScript == null)
        {
            _pathScript = GameObject.FindObjectOfType<CPathfinding>();
        }

        _behaviourText = transform.Find("BehaviourText").GetComponent<TextMesh>();
        switch (state)
        {
            case EAIState.flee: _behaviourText.text = "Flee"; break;
            case EAIState.groundpatrol: _behaviourText.text = "Ground Patrol"; break;

            default: _behaviourText.text = ""; break;
        }
    }

    /*Destroy object on lateupdate to avoid warning errors of objects not existing*/
    void LateUpdate()
    {
        if (destroy) { Destroy(gameObject); }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

#region Private

    private void SetPathingTargetFlee(float directionX)
    {
        Vector3 positionT = transform.position;

        for (int i = 0; i < 2; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(3 * directionX, 2.9f), 4.5f, _controller.collisionMask);
            positionT = transform.position;
            if (hit)
            {
                hit = Physics2D.Raycast(transform.position, new Vector2(1 * directionX, 0), 1f, _controller.collisionMask);
                if (hit)
                {
                    directionX *= -1f;
                }
                else
                {
                    positionT.x += 2f * directionX; break;
                }
            }
            else
            {
                positionT.x += 3f * directionX;
                positionT.y += 2.9f;
                break;
            }
        }
        _pathAgent.RequestPath(positionT);
    }

    private bool PlayerInRange(float range, bool raycastOn)
    {
        if (player && Vector3.Distance(player.transform.position, transform.position) < range)
        {
            if (raycastOn && !Physics2D.Linecast(transform.position, player.transform.position, _controller.collisionMask))
            {
                return true;
            }
            else if (!raycastOn)
            {
                return true;
            }
        }
        return false;
    }

    private void PathfindChase()
    {
        //Switch to chase if player in range
        if (PlayerInRange(6f, true))
        {
            _pathAgent.pathfindingTarget = player;
            state = EAIState.chase;
            _behaviourText.text = "Chase";
        }
    }

    private void Chase()
    { //Add this method into AiController

        _pathAgent.pathfindingTarget = player;
        state = EAIState.chase;
        _behaviourText.text = "Chase";

    }

    private void Flee()
    {
        fFleeTimer += Time.deltaTime;
        if (fFleeTimer >= fleeTimer)
        {
            fFleeTimer = 0;
            if (_pathAgent.GetNodesFromCompletion() > 2) { return; }
            if (!PlayerInRange(7f, true) && _controller.p_pCollisionInfo.below)
            {
                state = EAIState.groundpatrol;
                _behaviourText.text = "Ground Patrol";
                _pathAgent.CancelPathing(); return;
            }

            float radius = 8f; //Max range for random node
            float innerRadius = 3f; //Min range for random node

            List<CPathfinding.pathNode> nodes = _pathScript.GetGroundAndLadders();
            List<CPathfinding.pathNode> nodesInArea = new List<CPathfinding.pathNode>();

            float slopeX = (player.transform.position.y - transform.position.y) * 0.1f; //inverse 1 (slope), x and y are purposefuly backwards.
            float slopeY = -(player.transform.position.x - transform.position.x);
            Vector2 a = new Vector2(transform.position.x + slopeX * radius, transform.position.y + slopeY);
            Vector2 b = new Vector2(transform.position.x - slopeX * radius, transform.position.y - slopeY);
            Debug.DrawLine(a, b, Color.red, 5f);

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].neighbours.Count > 2 &&
                    ((b.x - a.x) * (nodes[i].pos.y - a.y) - (b.y - a.y) * (nodes[i].pos.x - a.x)) > 0 &&
                    Mathf.Pow(transform.position.x - nodes[i].pos.x, 2) + Mathf.Pow(transform.position.y - nodes[i].pos.y, 2) <= Mathf.Pow(radius, 2) &&
                    Mathf.Pow(transform.position.x - nodes[i].pos.x, 2) + Mathf.Pow(transform.position.y - nodes[i].pos.y, 2) >= Mathf.Pow(innerRadius, 2))
                {
                    nodesInArea.Add(nodes[i]);
                }
            }

            if (nodesInArea.Count > 0)
            {
                Vector3 test = nodesInArea[Random.Range(0, nodesInArea.Count - 1)].pos;
                test.y += 0.5f;
                _pathAgent.RequestPath(test);
                _behaviourText.text = "Flee";
            }
            else
            {
                //could potentially run back into character, -- we can't run any further away in direction we want, so we pick a random point.

                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].neighbours.Count > 2 &&
                        Mathf.Pow(transform.position.x - nodes[i].pos.x, 2) + Mathf.Pow(transform.position.y - nodes[i].pos.y, 2) <= Mathf.Pow(radius, 2) &&
                        Mathf.Pow(transform.position.x - nodes[i].pos.x, 2) + Mathf.Pow(transform.position.y - nodes[i].pos.y, 2) >= Mathf.Pow(innerRadius, 2))
                    {
                        nodesInArea.Add(nodes[i]);
                    }
                }
                if (nodesInArea.Count > 0)
                {
                    Vector3 test = nodesInArea[Random.Range(0, nodesInArea.Count - 1)].pos;
                    test.y += 0.5f;
                    _pathAgent.RequestPath(test);
                    _behaviourText.text = "Flee";
                }
                else
                {

                    _behaviourText.text = "Can't Flee & Scared.";

                }
            }
        }
    }

    private void GroundPatrol(ref Vector2 input)
    {
        //Switch to flee if player in range
        if (PlayerInRange(6f, true))
        {
            state = EAIState.flee;

        }
        if (direction == 1 && (_controller.p_pCollisionInfo.right || _controller.p_pCollisionInfo.below))
        {
            direction = -1;
        }
        else if (direction == -1 && (_controller.p_pCollisionInfo.left || _controller.p_pCollisionInfo.below))
        {
            direction = 1;
        }

        input.x = direction;
    }

#endregion Private
}