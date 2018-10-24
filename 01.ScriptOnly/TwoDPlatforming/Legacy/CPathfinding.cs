using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CPathfinding : MonoBehaviour
{
    public enum EPathNodeType
    {
        None,
        fall,
        walkable,
        jump,
        climb,
        portal
    }

    public class pathNode
    {
        public Vector3 pos;
        public EPathNodeType type;
        public float realHeight = 0f;
        public float height = 0f;

        public float f = 0f; //estimated distance from finish
        public float g = 0f; //cost to get to node
        public float c = 0f; //cost of node

        public float h = 0f; //nodeValue

        public pathNode parent = null;
        public GameObject gameObject;

        public pathNode spawnedFrom = null; //the node that created this.
        public List<pathNode> createdJumpNodes = new List<pathNode>();
        public List<pathNode> createdFallNodes = new List<pathNode>();

        public List<pathNode> neighbours = new List<pathNode>();

        public pathNode(EPathNodeType typeOfNode, Vector3 position)
        {
            pos = position;
            type = typeOfNode;
        }
    }

    public class instructions
    {
        public Vector3 pos = Vector3.zero;
        public EPathNodeType order = EPathNodeType.None;

        public instructions(Vector3 position, EPathNodeType pOrder)
        {
            pos = position;
            order = pOrder;
        }
    }
    public class threadLock
    {
        public GameObject character;
        public bool passed = false;
        public bool usingLadder;
        public Vector3 charPos, end;
        public float jump;
        public List<instructions> instr = null;

        //abilities
        public bool canMove;
        public bool canJump;
        public bool canClimb;
        public bool canFall;
        public bool canPortal;

        public threadLock(GameObject pC, Vector3 pE, bool uL, float jumpHeight, bool cMove, bool cJump, bool cClimb, bool cFall, bool cPortal)
        {
            character = pC;
            usingLadder = uL;
            charPos = pC.transform.position;
            end = pE;
            jump = jumpHeight;

            canMove = cMove;
            canJump = cJump;
            canClimb = cClimb;
            canFall = cFall;
            canPortal = cPortal;
        }
    }

    [System.Serializable]
    public class nodeWeight
    {

        public float groundNode = 1f;
        public float jumpNode = 9.2f;
        public float fallNode = 1f;
        public float climbNode = 3f;
        public float portalNode = 0f;

        public float GetNodeWeightByString(EPathNodeType nodeType)
        {
            switch (nodeType)
            {
                case EPathNodeType.walkable: return groundNode;
                case EPathNodeType.jump: return jumpNode;
                case EPathNodeType.fall: return fallNode;
                case EPathNodeType.climb: return climbNode;
                case EPathNodeType.portal: return portalNode;
            }
            return 0f;
        }

        public Color GetNodeColorByString(EPathNodeType nodeType)
        {
            switch (nodeType)
            {
                case EPathNodeType.walkable: return Color.yellow;
                case EPathNodeType.jump: return Color.blue;
                case EPathNodeType.fall: return Color.black;
                case EPathNodeType.climb: return Color.cyan;
            }
            return Color.white;
        }
    }

    public LayerMask groundLayer;
    public LayerMask ladderLayer;
    public LayerMask portalLayer;
    public LayerMask onewayLayer;

    public GameObject _currentMap;

    public float blockSize = 1f; //each block is square. This should probably match your square 2dCollider on a tile.
    public float jumpHeight = 3.8f; //the maximum jump height of a character
    public float maxJumpBlocksX = 3f; //the furthest a character can jump without momentum
    public float jumpHeightIncrement = 1f;
    public float minimumJump = 1.8f;

    private float groundNodeHeight = 0.01f; //percentage of blockSize (Determines height off ground level for a groundNode)
    private float groundMaxWidth = 0.35f; //percentage of blockSize (Determines max spacing allowed between two groundNodes)
    private float fall_X_Spacing = 0.25f; //percentage of blockSize (Determines space away from groundNode's side to place the fallNode)
    private float fall_Y_GrndDist = 0.02f;
    private Thread t;


    private List<pathNode> nodes = new List<pathNode>();
    private List<pathNode> groundNodes = new List<pathNode>();
    private List<pathNode> ladderNodes = new List<pathNode>();
    private List<pathNode> portalNodes = new List<pathNode>();

    public List<threadLock> orders = new List<threadLock>();
    public List<threadLock> readyOrders = new List<threadLock>();

    public nodeWeight nodeWeights;

    public bool debugTools = false; /*Pauses game on runtime and displays pathnode connections*/

    public List<pathNode> GetGroundAndLadders()
    {
        List<pathNode> gl = new List<pathNode>();
        gl.AddRange(groundNodes);
        gl.AddRange(ladderNodes);
        return gl;
    }

    void Awake()
    {
        //groundLayer = LayerManager.access.groundLayer;
        //ladderLayer = LayerManager.access.ladderLayer;
        //portalLayer = LayerManager.access.portalLayer;
        //onewayLayer = LayerManager.access.onewayLayer;
    }

    void Start()
    {
        //Debug tools do not work in awake!
        CreateNodeMap();
    }

    void Update()
    {
        DeliverPathfindingInstructions();
        MakeThreadDoWork();
    }

    void CreateNodeMap()
    {
        nodes = new List<pathNode>();
        groundNodes = new List<pathNode>();
        ladderNodes = new List<pathNode>();
        portalNodes = new List<pathNode>();

        List<GameObject> groundObjects = new List<GameObject>();
        List<GameObject> onewayObjects = new List<GameObject>();
        List<GameObject> ladderObjects = new List<GameObject>();
        List<GameObject> portalObjects = new List<GameObject>();

        //Find all children of tile parent
        foreach (Transform child in _currentMap.transform)
        {
            if (1 << child.gameObject.layer == groundLayer.value)
            {
                groundObjects.Add(child.gameObject);
            }
            else if (1 << child.gameObject.layer == ladderLayer.value)
            {
                ladderObjects.Add(child.gameObject);
            }
            else if (1 << child.gameObject.layer == portalLayer.value)
            {
                portalObjects.Add(child.gameObject);
            }
            else if (1 << child.gameObject.layer == onewayLayer.value)
            {
                onewayObjects.Add(child.gameObject);
            }
        }

        FindGroundNodes(groundObjects);
        FindOnewayNodes(onewayObjects);
        FindLadderNodes(ladderObjects);
        FindFallNodes(groundNodes); //@param list of nodes to search (tiles)
        FindJumpNodes(groundNodes);
        FindPortalNodes(portalObjects);

        GroundNeighbors(groundNodes, groundNodes);

        LadderNeighbors(ladderNodes, ladderNodes, false); //manaage ladder nodes like ground nodes *************TODO
        LadderNeighbors(groundNodes, ladderNodes, true);
        LadderNeighbors(ladderNodes, groundNodes, true);

        //manaage ladder nodes like ground nodes *************TODO
        PortalNeighbors(portalNodes, portalNodes, true); //portalNodes must be in position 1
        PortalNeighbors(portalNodes, groundNodes, false);

        JumpNeighbors(attachedJumpNodes(groundNodes), groundNodes); //CHANGE this function to find all jump nodes attached to ground nodes **********TODO
        FallNeighbors(attachedFallNodes(groundNodes), groundNodes);  //CHANGE this function to find all fall nodes attached to ground nodes **********TODO

        if (debugTools)
        {
            Debug.Break();
        }
    }

    public void RequestPathInstructions(GameObject character, Vector3 location, bool usingLadder, float jumpH,/*char abilities*/ bool movement, bool jump, bool ladder, bool fall, bool portal)
    {
        bool replaced = false;
        threadLock newLocker = new threadLock(character, location, usingLadder, jumpH, movement, jump, ladder, fall, portal);

        for (int i = 1; i < orders.Count; i++)
        {
            if (orders[i].character == character)
            {
                orders[i] = newLocker;
                replaced = true;
                break;
            }
        }

        if (!replaced)
        {
            orders.Add(newLocker);
        }
    }

    public void FindPath(object threadLocker)
    {
        threadLock a = (threadLock)threadLocker;
        Vector3 character = a.charPos;
        Vector3 location = a.end;
        float characterJump = a.jump;

        List<instructions> instr = new List<instructions>();

        List<pathNode> openNodes = new List<pathNode>();
        List<pathNode> closedNodes = new List<pathNode>();
        List<pathNode> pathNodes = new List<pathNode>();

        ResetLists(); //sets parent to null

        pathNode startNode = new pathNode(EPathNodeType.None, Vector3.zero);
        if (a.usingLadder) { startNode = getNearestLadderNode(character); } else { startNode = getNearestGroundNode(character); }

        pathNode endNode = getNearestNode(location);

        /*if a point couldnt be found or if character can't move cancel path*/
        if (endNode == null || startNode == null || !a.canMove)
        {
            a.passed = false;
            a.instr = instr;
            readyOrders.Add(a);
            return;
        }

        startNode.g = 0;
        startNode.f = Vector3.Distance(startNode.pos, endNode.pos);
        openNodes.Add(startNode);

        pathNode currentNode = new pathNode(EPathNodeType.None, Vector3.zero);
        while (openNodes.Count > 0)
        {
            float lowestScore = float.MaxValue;
            for (int i = 0; i < openNodes.Count; i++)
            {
                if (openNodes[i].f < lowestScore)
                {
                    currentNode = openNodes[i]; lowestScore = currentNode.f;
                }
            }
            if (currentNode == endNode) { closedNodes.Add(currentNode); break; }
            else
            {
                closedNodes.Add(currentNode);
                openNodes.Remove(currentNode);
                if (currentNode.type != EPathNodeType.jump || (currentNode.type == EPathNodeType.jump
                    && Mathf.Abs(currentNode.realHeight - characterJump) < jumpHeightIncrement * 0.92) && characterJump <= currentNode.realHeight + jumpHeightIncrement * 0.08)
                {
                    for (int i = 0; i < currentNode.neighbours.Count; i++)
                    {
                        if (!a.canJump && currentNode.neighbours[i].type == EPathNodeType.jump) { continue; }
                        if (!a.canClimb && currentNode.neighbours[i].type == EPathNodeType.climb) { continue; }
                        if (!a.canFall && currentNode.neighbours[i].type == EPathNodeType.fall) { continue; }
                        if (!a.canPortal && currentNode.neighbours[i].type == EPathNodeType.portal) { continue; }

                        if (currentNode.neighbours[i].parent == null)
                        {

                            currentNode.neighbours[i].g = currentNode.neighbours[i].c + currentNode.g;
                            currentNode.neighbours[i].h = Vector3.Distance(currentNode.neighbours[i].pos, endNode.pos);
                            if (currentNode.neighbours[i].type == EPathNodeType.jump) { currentNode.neighbours[i].h += currentNode.neighbours[i].realHeight; }
                            currentNode.neighbours[i].f = currentNode.neighbours[i].g + currentNode.neighbours[i].h;
                            currentNode.neighbours[i].parent = currentNode;
                            openNodes.Add(currentNode.neighbours[i]);
                        }
                        else
                        {
                            if (currentNode.g + currentNode.neighbours[i].c < currentNode.neighbours[i].g)
                            {
                                currentNode.neighbours[i].g = currentNode.neighbours[i].c + currentNode.g;
                                currentNode.neighbours[i].f = currentNode.neighbours[i].g + currentNode.neighbours[i].h;
                                currentNode.neighbours[i].parent = currentNode;
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < 700; i++)
        {
            if (currentNode.parent == null)
                break;

            if (i > 600)
                Debug.Log("somethingwrong");

            pathNodes.Add(currentNode);
            currentNode = currentNode.parent;
            if (currentNode == startNode)
            {
                pathNodes.Add(startNode);
                break;
            }
        }

        a.passed = pathNodes[0] == endNode;
        pathNodes.Reverse();
        for (int i = 0; i < pathNodes.Count; i++)
        {
            instr.Add(new instructions(pathNodes[i].pos, pathNodes[i].type));
        }

        a.instr = instr;
        readyOrders.Add(a);
    }

    public void DeliverPathfindingInstructions()
    {
        for (int i = 0; i < readyOrders.Count; i++)
        {
            if (readyOrders[i].character)
            {
                if (readyOrders[i].character.transform.GetComponent<CPathFindingAgent>() != null)
                {
                    readyOrders[i].character.transform.GetComponent<CPathFindingAgent>().ReceivePathInstructions(readyOrders[i].instr, readyOrders[i].passed);
                }
            }
        }
        readyOrders = new List<threadLock>();
    }

    private void ResetLists()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].parent = null;
        }
    }

    private void FindPortalNodes(List<GameObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Vector3 portal = objects[i].transform.position; //ground.y += blockSize * 0.5f + blockSize * groundNodeHeight;
            pathNode newPortalNode = new pathNode(EPathNodeType.portal, portal);
            newPortalNode.gameObject = objects[i];

            portalNodes.Add(newPortalNode);

            newPortalNode.c = nodeWeights.GetNodeWeightByString(newPortalNode.type);
            nodes.Add(newPortalNode);
        }
    }

    private void PortalNeighbors(List<pathNode> fromNodes, List<pathNode> toNodes, bool onlyPortals)
    {
        // float distanceForGround = blockSize * 1.5f + 0.1f;
        // float maxXDistance = blockSize * 1f;
        for (int i = 0; i < fromNodes.Count; i++)
        {
            pathNode a = fromNodes[i];
            if (a.gameObject)
            {
                //GameObject aCheck = a.gameObject.transform.GetComponent<Portal>().connectedTo;
                //if (aCheck != null)
                //{
                //    for (int t = 0; t < toNodes.Count; t++)
                //    {
                //        pathNode b = toNodes[t];
                //        if (onlyPortals)
                //        {
                //            if (aCheck == b.gameObject)
                //            {
                //                a.neighbours.Add(b);
                //                if (debugTools)
                //                {
                //                    Debug.DrawLine(a.pos, b.pos, Color.cyan);
                //                }
                //            }
                //        }
                //        else
                //        {
                //            if (Mathf.Abs(a.pos.x - b.pos.x) < maxXDistance &&
                //                Vector3.Distance(a.pos, b.pos) < distanceForGround)
                //            {
                //                a.neighbours.Add(b);
                //                b.neighbours.Add(a);
                //                if (debugTools)
                //                {
                //                    Debug.DrawLine(a.pos, b.pos, Color.cyan);
                //                }
                //            }
                //        }
                //    }
                //}
            }
        }
    }

    private void FindLadderNodes(List<GameObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Vector3 ladder = objects[i].transform.position;
            pathNode newLadderNode = new pathNode(EPathNodeType.climb, ladder);
            ladderNodes.Add(newLadderNode);

            newLadderNode.c = nodeWeights.GetNodeWeightByString(newLadderNode.type);
            nodes.Add(newLadderNode);
        }
    }
    private void LadderNeighbors(List<pathNode> fromNodes, List<pathNode> toNodes, bool includesGround)
    {
        float distanceBetween = blockSize + groundMaxWidth;
        float distanceForGround = blockSize * 0.5f + 0.2f;
        float maxXDistance = blockSize * 0.501f;
        for (int i = 0; i < fromNodes.Count; i++)
        {
            pathNode a = fromNodes[i];
            for (int t = 0; t < toNodes.Count; t++)
            {
                pathNode b = toNodes[t];
                if (Mathf.Abs(a.pos.x - b.pos.x) < maxXDistance &&
                   ((!includesGround && Vector3.Distance(a.pos, b.pos) < distanceBetween) ||
                    (includesGround && Vector3.Distance(a.pos, b.pos) < distanceForGround)))
                {
                    a.neighbours.Add(b);
                    if (debugTools)
                    {
                        Debug.DrawLine(a.pos, b.pos, Color.red);
                    }
                }
            }
        }
    }

    private void FindGroundNodes(List<GameObject> objects)
    {
        nodes = new List<pathNode>();
        for (int i = 0; i < objects.Count; i++)
        {
            Vector3 ground = objects[i].transform.position; ground.y += blockSize * 0.5f + blockSize * groundNodeHeight;
            pathNode newGroundNode = new pathNode(EPathNodeType.walkable, ground);
            groundNodes.Add(newGroundNode);

            newGroundNode.c = nodeWeights.GetNodeWeightByString(newGroundNode.type);
            nodes.Add(newGroundNode);

            newGroundNode.gameObject = objects[i];
        }
    }
    private void FindOnewayNodes(List<GameObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Vector3 ground = objects[i].transform.position; ground.y += blockSize * 0.5f + blockSize * groundNodeHeight;
            pathNode newGroundNode = new pathNode(EPathNodeType.walkable, ground);
            groundNodes.Add(newGroundNode);

            newGroundNode.c = nodeWeights.GetNodeWeightByString(newGroundNode.type);
            nodes.Add(newGroundNode);

            newGroundNode.gameObject = objects[i];
        }
    }

    private void GroundNeighbors(List<pathNode> fromNodes, List<pathNode> toNodes)
    {
        //Distance max distance allowed between two nodes
        float distanceBetween = blockSize + groundMaxWidth;
        for (int i = 0; i < fromNodes.Count; i++)
        {
            pathNode a = fromNodes[i];
            for (int t = 0; t < toNodes.Count; t++)
            {
                pathNode b = toNodes[t];
                //testing distance between nodes
                if (Mathf.Abs(a.pos.y - b.pos.y) < blockSize * 0.7 && Vector3.Distance(a.pos, b.pos) < distanceBetween)
                {
                    //testing collision between nodes
                    if (!Physics2D.Linecast(a.pos, b.pos, groundLayer))
                    {
                        a.neighbours.Add(b);
                        if (debugTools)
                        {
                            Debug.DrawLine(a.pos, b.pos, Color.red);
                        }
                    }
                }
            }
        }
    }

    private void FindJumpNodes(List<pathNode> searchList)
    {
        if (jumpHeight > 0)
        {
            for (int i = 0; i < searchList.Count; i++)
            {
                float curHeight = jumpHeight;
                while (curHeight >= minimumJump)
                {
                    Vector3 air = searchList[i].pos; air.y += curHeight;

                    if (!Physics2D.Linecast(searchList[i].pos, air, groundLayer))
                    {
                        pathNode newJumpNode = new pathNode(EPathNodeType.jump, air);

                        newJumpNode.spawnedFrom = searchList[i]; //this node has been spawned from a groundNode
                        //jumpNodes.Add(newJumpNode);
                        newJumpNode.c = nodeWeights.GetNodeWeightByString(newJumpNode.type);
                        newJumpNode.height = curHeight;
                        newJumpNode.realHeight = curHeight;
                        nodes.Add(newJumpNode);

                        newJumpNode.spawnedFrom.createdJumpNodes.Add(newJumpNode);
                    }
                    else
                    {
                        float h = curHeight;
                        float minHeight = blockSize * 1f; //2f
                        while (h > minHeight)
                        {
                            Vector3 newHeight = new Vector3(air.x, air.y - (curHeight - h), air.z);
                            if (!Physics2D.Linecast(searchList[i].pos, newHeight, groundLayer))
                            {
                                pathNode newJumpNode = new pathNode(EPathNodeType.jump, newHeight);

                                newJumpNode.spawnedFrom = searchList[i]; //this node has been spawned from a groundNode
                                //jumpNodes.Add(newJumpNode);
                                newJumpNode.c = nodeWeights.GetNodeWeightByString(newJumpNode.type);
                                newJumpNode.realHeight = curHeight;
                                newJumpNode.height = h;
                                nodes.Add(newJumpNode);

                                newJumpNode.spawnedFrom.createdJumpNodes.Add(newJumpNode);
                                break;
                            }
                            else
                            {
                                //0.5f
                                h -= blockSize * 0.1f;
                            }
                        }
                    }
                    curHeight -= jumpHeightIncrement;
                }
            }
        }
    }

    private void JumpNeighbors(List<pathNode> fromNodes, List<pathNode> toNodes)
    {
        for (int i = 0; i < fromNodes.Count; i++)
        {
            pathNode a = fromNodes[i];
            for (int t = 0; t < toNodes.Count; t++)
            {
                pathNode b = toNodes[t];
                a.spawnedFrom.neighbours.Add(a);
                if (debugTools)
                {
                    Debug.DrawLine(a.pos, a.spawnedFrom.pos, Color.red);
                }

                float xDistance = Mathf.Abs(a.pos.x - b.pos.x);
                if (xDistance < blockSize * maxJumpBlocksX + blockSize + groundMaxWidth) //
                    //the x distance modifier used to be 0.72!
                    if (b != a.spawnedFrom && a.pos.y > b.pos.y + blockSize * 0.5f &&
                        a.pos.y - b.pos.y > Mathf.Abs(a.pos.x - b.pos.x) * 0.9f - blockSize * 1f &&
                          Mathf.Abs(a.pos.x - b.pos.x) < blockSize * 4f + groundMaxWidth)
                    {
                        if (!Physics2D.Linecast(a.pos, b.pos, groundLayer))
                        {
                            bool hitTest = true;
                            if ((Mathf.Abs(a.pos.x - b.pos.x) < blockSize + groundMaxWidth && a.spawnedFrom.pos.y == b.pos.y) ||
                                (a.pos.y - a.spawnedFrom.pos.y + 0.01f < a.height && Mathf.Abs(a.pos.x - b.pos.x) > blockSize + groundMaxWidth))
                            {
                                hitTest = false;
                            }

                            //hit head code... jump height must be above 2.5 to move Xdistance2.5 else you can only move 1 block when hitting head.
                            if (a.realHeight > a.height)
                            {
                                float tempFloat = a.height > 2.5f ? 3.5f : 1.5f;
                                if (tempFloat == 1.5f && a.height > 1.9f) { tempFloat = 2.2f; }
                                if (a.spawnedFrom.pos.y < b.pos.y && Mathf.Abs(a.spawnedFrom.pos.x - b.pos.x) > blockSize * 1.5f) { tempFloat = 0f; }
                                if (Mathf.Abs(a.spawnedFrom.pos.x - b.pos.x) > blockSize * tempFloat)
                                {
                                    hitTest = false;
                                }
                            }

                            if (hitTest)
                            {
                                float middle = -(a.pos.x - b.pos.x) / 2f;
                                float quarter = middle / 2f;

                                Vector3 origin = a.spawnedFrom.pos;
                                Vector3 midPoint = new Vector3(a.pos.x + middle, a.pos.y, a.pos.z);
                                Vector3 quarterPoint = new Vector3(a.pos.x + quarter, a.pos.y, a.pos.z);

                                Vector3 quarterPastMidPoint = new Vector3(a.pos.x + middle + quarter, a.pos.y - blockSize, a.pos.z);
                                Vector3 lowerMid = new Vector3(a.pos.x + middle, a.pos.y - blockSize, a.pos.z);
                                Vector3 straightUp = new Vector3(b.pos.x, a.pos.y - blockSize, a.pos.z);

                                if (xDistance > blockSize + groundMaxWidth)
                                    if (Physics2D.Linecast(origin, quarterPoint, groundLayer) ||

                                        (xDistance > blockSize + groundMaxWidth &&
                                         Physics2D.Linecast(b.pos, quarterPastMidPoint, groundLayer) &&
                                         a.spawnedFrom.pos.y >= b.pos.y - groundNodeHeight) ||

                                        (Physics2D.Linecast(origin, midPoint, groundLayer)) ||

                                          (xDistance > blockSize + groundMaxWidth &&
                                         a.spawnedFrom.pos.y >= b.pos.y - groundNodeHeight &&
                                         Physics2D.Linecast(lowerMid, b.pos, groundLayer)) ||

                                            (xDistance > blockSize * 1f + groundMaxWidth &&
                                         a.spawnedFrom.pos.y >= b.pos.y &&
                                          Physics2D.Linecast(b.pos, straightUp, groundLayer))
                                       )
                                    {
                                        hitTest = false;
                                    }
                            }

                            if (hitTest)
                            {
                                a.neighbours.Add(b);
                                if (debugTools)
                                {
                                    Debug.DrawLine(a.pos, b.pos, Color.blue);
                                }
                            }
                        }
                    }
            }
        }
    }

    private void FindFallNodes(List<pathNode> searchList)
    {
        float spacing = blockSize * 0.5f + blockSize * fall_X_Spacing;

        for (int i = 0; i < searchList.Count; i++)
        {
            Vector3 leftNode = searchList[i].pos; leftNode.x -= spacing;
            Vector3 rightNode = searchList[i].pos; rightNode.x += spacing;

            //raycheck left
            if (!Physics2D.Linecast(searchList[i].pos, leftNode, groundLayer))
            {
                Vector3 colliderCheck = leftNode;
                colliderCheck.y -= fall_Y_GrndDist;

                //raycheck down
                if (!Physics2D.Linecast(leftNode, colliderCheck, groundLayer))
                {
                    pathNode newFallNode = new pathNode(EPathNodeType.fall, leftNode);

                    newFallNode.spawnedFrom = searchList[i]; //this node has been spawned from a groundNode
                    //fallNodes.Add(newFallNode);

                    newFallNode.c = nodeWeights.GetNodeWeightByString(newFallNode.type);
                    nodes.Add(newFallNode);

                    newFallNode.spawnedFrom.createdFallNodes.Add(newFallNode);
                }
            }

            //raycheck right
            if (!Physics2D.Linecast(searchList[i].pos, rightNode, groundLayer))
            {
                Vector3 colliderCheck = rightNode;
                colliderCheck.y -= fall_Y_GrndDist;

                //raycheck down
                if (!Physics2D.Linecast(rightNode, colliderCheck, groundLayer))
                {
                    pathNode newFallNode = new pathNode(EPathNodeType.fall, rightNode);

                    newFallNode.spawnedFrom = searchList[i]; //this node has been spawned from a groundNode
                    newFallNode.c = nodeWeights.GetNodeWeightByString(newFallNode.type);
                    nodes.Add(newFallNode);

                    newFallNode.spawnedFrom.createdFallNodes.Add(newFallNode);
                }
            }
        }
    }
    private void FallNeighbors(List<pathNode> fromNodes, List<pathNode> toNodes)
    {
        for (int i = 0; i < fromNodes.Count; i++)
        {
            pathNode a = fromNodes[i];
            for (int t = 0; t < toNodes.Count; t++)
            {
                pathNode b = toNodes[t];
                float xDistance = Mathf.Abs(a.pos.x - b.pos.x);
                a.spawnedFrom.neighbours.Add(a);
                if (debugTools)
                {
                    Debug.DrawLine(a.spawnedFrom.pos, a.pos, Color.blue);
                }

                if ((xDistance < blockSize * 1f + groundMaxWidth && a.pos.y > b.pos.y) || (a.pos.y - b.pos.y > Mathf.Abs(a.pos.x - b.pos.x) * 2.2f + blockSize * 1f && //2.2 + blocksize * 1f
                    xDistance < blockSize * 4f))
                {
                    if (!Physics2D.Linecast(a.pos, b.pos, groundLayer))
                    {
                        bool hitTest = true;

                        float middle = -(a.pos.x - b.pos.x) * 0.5f;
                        float quarter = middle / 2f;

                        float reduceY = Mathf.Abs(a.pos.y - b.pos.y) > blockSize * 4f ? blockSize * 1.3f : 0f;

                        Vector3 middlePointDrop = new Vector3(a.pos.x + middle, a.pos.y - reduceY, a.pos.z);
                        Vector3 quarterPointTop = new Vector3(a.pos.x + quarter, a.pos.y, a.pos.z);
                        Vector3 quarterPointBot = new Vector3(b.pos.x - quarter, b.pos.y, b.pos.z);
                        Vector3 corner = new Vector3(b.pos.x, (a.pos.y - blockSize * xDistance - blockSize * 0.5f) - groundNodeHeight, a.pos.z);

                        if (Physics2D.Linecast(quarterPointTop, b.pos, groundLayer) ||
                            Physics2D.Linecast(middlePointDrop, b.pos, groundLayer) ||
                            a.pos.y > b.pos.y + blockSize + groundNodeHeight && Physics2D.Linecast(corner, b.pos, groundLayer) ||
                            Physics2D.Linecast(quarterPointBot, a.pos, groundLayer))
                        {
                            hitTest = false;
                        }
                        if (hitTest)
                        {
                            a.neighbours.Add(b);
                            if (debugTools)
                            {
                                Debug.DrawLine(a.pos, b.pos, Color.black);
                            }
                        }
                    }
                }
            }
        }
    }

    //get nearest node ladder, ground. Useful for finding start and end points of the path
    private pathNode getNearestNode(Vector3 obj)
    {
        float dist = float.MaxValue;
        pathNode node = null;
        for (int i = 0; i < groundNodes.Count; i++)
        {
            if (groundNodes[i].neighbours.Count > 0 && obj.y > groundNodes[i].pos.y && Mathf.Abs(obj.x - groundNodes[i].pos.x) < blockSize
                /*only find ground nodes that are within 4f*/&& obj.y - groundNodes[i].pos.y < 4f)
            {
                float temp = Vector3.Distance(obj, (Vector3)groundNodes[i].pos);
                if (dist > temp)
                {
                    dist = temp; node = groundNodes[i];
                }
            }
        }

        for (int i = 0; i < ladderNodes.Count; i++)
        {
            if (ladderNodes[i].neighbours.Count > 0 && obj.y > ladderNodes[i].pos.y && Mathf.Abs(obj.x - ladderNodes[i].pos.x) < blockSize)
            {
                float temp = Vector3.Distance(obj, (Vector3)ladderNodes[i].pos);
                if (dist > temp)
                {
                    dist = temp; node = ladderNodes[i];
                }
            }
        }

        return node;
    }

    private pathNode getNearestGroundNode(Vector3 obj)
    {
        float dist = float.MaxValue;
        pathNode node = null;
        for (int i = 0; i < groundNodes.Count; i++)
        {
            if (groundNodes[i].neighbours.Count > 0)
            {
                float temp = Vector3.Distance(obj, (Vector3)groundNodes[i].pos);
                if (dist > temp)
                {
                    if (obj.y >= groundNodes[i].pos.y && Mathf.Abs(obj.x - groundNodes[i].pos.x) < blockSize)
                    {
                        dist = temp;
                        node = groundNodes[i];
                    }
                }
            }
        }
        return node;
    }

    private pathNode getNearestLadderNode(Vector3 obj)
    {
        float dist = float.MaxValue;
        pathNode node = null;
        for (int i = 0; i < ladderNodes.Count; i++)
        {
            if (ladderNodes[i].neighbours.Count > 0)
            {
                float temp = Vector3.Distance(obj, (Vector3)ladderNodes[i].pos);
                if (dist > temp && Mathf.Abs(obj.x - ladderNodes[i].pos.x) < blockSize)
                {
                    dist = temp;
                    node = ladderNodes[i];
                }
            }
        }
        return node;
    }

    //Used when reconstructing pathnode connections
    List<pathNode> attachedJumpNodes(List<pathNode> pGround)
    {
        List<pathNode> returnNodes = new List<pathNode>();
        for (int i = 0; i < pGround.Count; i++)
        {

            returnNodes.AddRange(pGround[i].createdJumpNodes);
        }
        return returnNodes;
    }

    List<pathNode> attachedFallNodes(List<pathNode> pGround)
    {
        List<pathNode> returnNodes = new List<pathNode>();
        for (int i = 0; i < pGround.Count; i++)
        {

            returnNodes.AddRange(pGround[i].createdFallNodes);
        }
        return returnNodes;
    }

    public void MakeThreadDoWork()
    {
        if ((orders.Count > 0 && t == null) || (orders.Count > 0 && !t.IsAlive))
        {
            t = new Thread(new ParameterizedThreadStart(FindPath));
            t.IsBackground = true;
            t.Start(orders[0]);
            orders.RemoveAt(0);
        }
    }

    private void OnDrawGizmos()
    {
        if (debugTools == false) return;

        for (int i = 0; i < nodes.Count; i++)
        {
            Gizmos.color = nodeWeights.GetNodeColorByString(nodes[i].type);
            Gizmos.DrawSphere(nodes[i].pos, 0.12f);
        }
    }
}