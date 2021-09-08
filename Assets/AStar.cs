using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[ExecuteInEditMode]
public class AStar : MonoBehaviour
{
    public enum TileStates
    {
        WALKABLE,
        FLYABLE,
        OBSTACLE
    }
    public enum EntityState
    {
        WALK,
        FLY
    }

    [Serializable]
    public class Tiles
    {
        public Sprite sprite;
        public TileStates state;
    }

    public bool DebugVisuals = false;

    [Header("Required")]
    public Tilemap Ground;
    public List<Tiles> TileInfo = new List<Tiles>();
    public float ObstacleOffset;

    #region Navmesh items
    public int PerUnitFreq = 2;
    private static float PerUnitDist;
    public static readonly List<Node> NodeMap = new List<Node>();

    #region Heaps
    public static List<Node> OpenNodes = new List<Node>();
    public static List<Node> ClosedNodes = new List<Node>();
    #endregion
    #endregion

    #region Navmesh debug visuals
    private static float DisplayDist;
    private static float DisplayGap = 0.05f;
    #endregion


    

    public class Node
    {
        public TileStates state;

        //Distance from start node
        public float G;

        //distance from end node
        public float H;

        //H cost + G cost
        public float F { get { return H + G; } }

        public Vector2 Position;
        public Node Parent;
        public List<Node> Children;

        public Node(Vector2 pos)
        {
            Position = pos;
        }

        public void UpdateNode(Vector2 TargetPos)
        {
            H = (TargetPos.x - Position.x) + (TargetPos.y - Position.y);//Manhattan distance ignores obstacles, since this is heuristic value, its important but not required to be accurate
        }

        public void ChildrenUpdate()
        {
            //THIS NEEDS TO BE REVIEWED
            //THIS NEEDS REWRITING, FINDING NODES BY POSITION IN THIS WAY IS UNRELIABLE
            //--------------------------------------------------------------------------

            List<Node> ch = new List<Node>();
            for (int c_x = -1; c_x <= 1; c_x++)
            {
                for (int c_y = -1; c_y <= 1; c_y++)
                {

                    Vector2 tmp = new Vector2(Position.x + (c_x * PerUnitDist), (Position.y + (c_y * PerUnitDist)));

                    if (tmp == Position)
                        continue;
                    //skip over the current node, cause its not a child of itself
                    Node tmpn = NodeMap.Find((n) => { return n.Position == tmp;  });
                    if (tmpn != null)
                        ch.Add(tmpn);
                }
            }
            Children = ch;
        }
    }


    private void Awake()
    {
        Setup();
    }


    private void Setup()
    {
        NodeMap.Clear();
        PerUnitDist = 1 / (float)PerUnitFreq;//this should be obvious, 1 unit = 1 tile in our case

        
        // 1 - the display gap * per unit + 1 / per unit = the width and height of the cell
        DisplayDist = (1 - (DisplayGap * (PerUnitFreq + 1)))/PerUnitFreq;



        //Have to do this bc turns out for tile maps all assets are instantiated of an existing one, so the position values in the matrix are useless for now cause too dumb to think of better solution
        for (int x = Ground.cellBounds.xMin; x < Ground.cellBounds.xMax; x++)
        {
            for (int y = Ground.cellBounds.yMin; y < Ground.cellBounds.yMax; y++)
            {
                Vector3 TilePos = Ground.CellToWorld(new Vector3Int(x, y, (int)Ground.transform.position.z));
                Vector3Int TilePos_raw = new Vector3Int(x, y, (int)Ground.transform.position.z);
                if (Ground.HasTile(TilePos_raw) && TileInfo.Exists((t) => { return t.sprite == ((Tile)Ground.GetTile(TilePos_raw)).sprite; }))
                {
                    for (int w = 1; w <= PerUnitFreq; w++)
                    {
                        for (int h = 1; h <= PerUnitFreq; h++)
                        {
                            #region Grid displaying, was moved here, this is backup copy now
                            //Grid Display
                            //float DispPos_X = (DisplayGap * w) + (DisplayDist * w) - (DisplayDist / 2);
                            //float DispPos_Y = (DisplayGap * h) + (DisplayDist * h) - (DisplayDist / 2);
                            //if (!Physics2D.OverlapCircle(new Vector2(TilePos.x + DispPos_X, TilePos.y - DispPos_Y), ObstacleOffset))
                            //    Gizmos.DrawCube(new Vector2(TilePos.x + DispPos_X, TilePos.y - DispPos_Y), new Vector3(DisplayDist, DisplayDist, 0.0001f));
                            #endregion

                            Vector2 nodepos = new Vector2((PerUnitDist * w) - (PerUnitDist / 2), -((PerUnitDist * h) - (PerUnitDist / 2)));
                            //get the upper right corner, then remove half the size. basically, make a square, and set the position to the center of that square, thats our node pos
                            nodepos = new Vector2(TilePos.x + nodepos.x, TilePos.y - nodepos.y);

                            if (!Physics2D.OverlapCircle(nodepos, ObstacleOffset, LayerMask.GetMask("COLLISION")))
                            {
                                NodeMap.Add(new Node(nodepos) { state = TileInfo.Find((t) => { return t.sprite == ((Tile)Ground.GetTile(TilePos_raw)).sprite; }).state });
                            }
                        }
                    }
                }
            }
        }
    }

    public bool RunSetup = false;

    public bool RenderPath = false;
    public Node PathRender = null;
    private Node StartNode = null;

    private void OnDrawGizmos()
    {
        if (RunSetup)
        {
            Setup();
        }

        if (DebugVisuals)
        {
            foreach(Node n in NodeMap)
            {
                int x_, y_;
                x_ = n.Position.x < 0 ? (int)(Math.Ceiling(n.Position.x)) : (int)(Math.Floor(n.Position.x));
                y_ = n.Position.y < 0 ? (int)(Math.Ceiling(n.Position.y)) : (int)(Math.Floor(n.Position.y));

                if (RenderPath)
                {
                    Node CurrentNode = PathRender;
                    while (CurrentNode != StartNode)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawCube(CurrentNode.Position, new Vector3(DisplayDist, DisplayDist, 0.0001f));
                        CurrentNode = CurrentNode.Parent;
                    }
                    if(CurrentNode == StartNode)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawCube(n.Position, new Vector3(DisplayDist, DisplayDist, 0.0001f));
                    }
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.DrawCube(n.Position, new Vector3(DisplayDist, DisplayDist, 0.0001f));
                    Gizmos.color = Color.white;
                }
            }

            //float DispPos_X = (DisplayGap * w) + (DisplayDist * w) - (DisplayDist / 2);
            //float DispPos_Y = (DisplayGap * h) + (DisplayDist * h) - (DisplayDist / 2);
            //if (!Physics2D.OverlapCircle(new Vector2(TilePos.x + DispPos_X, TilePos.y - DispPos_Y), ObstacleOffset))
            //    Gizmos.DrawCube(new Vector2(TilePos.x + DispPos_X, TilePos.y - DispPos_Y), new Vector3(DisplayDist, DisplayDist, 0.0001f));

        }
    }

    public Node ClosestNode(Vector2 pos)
    {
        pos = new Vector2((Mathf.Round(pos.x / PerUnitDist))*PerUnitDist, (Mathf.Round(pos.y / PerUnitDist)) * PerUnitDist);
        float dist = Mathf.Infinity;
        Node Lowest = new Node(new Vector2());
        for(int i = 0; i < NodeMap.Count; i++)
        {
            if (Vector2.Distance(NodeMap[i].Position, pos) < dist)
            {
                dist = Vector2.Distance(NodeMap[i].Position, pos);
                Lowest = NodeMap[i];
            } 
        }
        return Lowest;//NodeMap.Find((n) => { return n.Position == pos; });
    }
     
    public Node RequestPath(Node Start, Node Target)
    {
        OpenNodes.Clear();
        ClosedNodes.Clear();
        OpenNodes.Add(Start);
        StartNode = Start;
        bool PathFound = false;
        while (OpenNodes.Count > 0)
        {
            Node CurrentNode = OpenNodes[0];
            for(int i = 0; i< OpenNodes.Count; i++)
            {
                if(OpenNodes[i].F <= CurrentNode.F)
                    if(OpenNodes[i].H < CurrentNode.H)
                        CurrentNode = OpenNodes[i];
            }

            //OpenNodes.OrderBy((f) => f.F).FirstOrDefault((n) => { return true; });
            //Debug.Log(CurrentNode.Position.ToString());

            OpenNodes.Remove(CurrentNode);
            ClosedNodes.Add(CurrentNode);

            if (CurrentNode == Target)
            {
                PathFound = true;
                return CurrentNode;
            }
            else
            {
                CurrentNode.ChildrenUpdate();
                foreach (Node node in CurrentNode.Children)
                {
                    if (node.state == TileStates.OBSTACLE || ClosedNodes.Contains(node))
                        continue;

                    //distance from current to neighbour is 1 for now
                    if (CurrentNode.G + 1 < node.G || !OpenNodes.Contains(node))
                    {
                        node.G = CurrentNode.G + 1;
                        node.UpdateNode(Target.Position);
                        node.Parent = CurrentNode;

                        if (!OpenNodes.Contains(node))
                            OpenNodes.Add(node);
                    }
                }
            }
        }
        return new Node(new Vector2(Mathf.Infinity, Mathf.Infinity));
    }


}

