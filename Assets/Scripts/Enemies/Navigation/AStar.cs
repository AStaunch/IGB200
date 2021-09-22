using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public static class AStar
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

    #region Required shit
    #region basics
    public static Tilemap Ground;
    public static List<Tiles> TileInfo = new List<Tiles>();
    public static float ObstacleOffset;
    #endregion
    #region Navmesh items
    public static int PerUnitFreq = 2;
    private static float PerUnitDist_;
    public static float PerUnitDist { get { return PerUnitDist_; } }
    public static readonly List<Node> NodeMap = new List<Node>();
    #endregion
    #region Heaps
    //these need to be reviewed, array heaps have the worst runtime of available A* heaps. Try sorted or fibonacci
    //private static List<Node> OpenNodes = new List<Node>();
    //private static List<Node> ClosedNodes = new List<Node>();
    private static FibonnaciHeap Open_FHeap = new FibonnaciHeap();
    private static FibonnaciHeap Closed_FHeap = new FibonnaciHeap();
    #endregion
    #endregion

    #region Navmesh debug visuals
    private static float DisplayDist_;
    public static float DisplayDist { get { return DisplayDist_; } }
    public readonly static float DisplayGap = 0.05f;
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
        public Node ActiveChild;

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
            //--------------------------------------------------------------------------
            //THIS NEEDS TO BE REVIEWED
            //THIS NEEDS REWRITING, FINDING NODES BY POSITION IN THIS WAY IS UNRELIABLE but it do kinda work doe
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
                    Node tmpn = NodeMap.Find((n) => { return n.Position == tmp; });
                    if (tmpn != null)
                        ch.Add(tmpn);
                }
            }
            Children = ch;
        }
    }
    
    public static void Setup()
    {
        NodeMap.Clear();
        PerUnitDist_ = 1 / (float)PerUnitFreq;//this should be obvious, 1 unit = 1 tile in our case


        // 1 - the display gap * per unit + 1 / per unit = the width and height of the cell
        DisplayDist_ = (1 - (DisplayGap * (PerUnitFreq + 1))) / PerUnitFreq;



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

                            if (!Physics2D.OverlapCircle(nodepos, ObstacleOffset, LayerMask.GetMask("WALL")))
                            {
                                NodeMap.Add(new Node(nodepos) { state = TileInfo.Find((t) => { return t.sprite == ((Tile)Ground.GetTile(TilePos_raw)).sprite; }).state });
                            }
                        }
                    }
                }
            }
        }
    }


    #region moved to startup obj
    //private void OnDrawGizmos()
    //{
    //    if (RunSetup)
    //    {
    //        Setup();
    //    }

    //    if (DebugVisuals)
    //    {
    //        foreach (Node n in NodeMap)
    //        {
    //            Gizmos.DrawCube(n.Position, new Vector3(DisplayDist, DisplayDist, 0.0001f));
    //            Gizmos.color = Color.white;
    //        }

    //        //float DispPos_X = (DisplayGap * w) + (DisplayDist * w) - (DisplayDist / 2);
    //        //float DispPos_Y = (DisplayGap * h) + (DisplayDist * h) - (DisplayDist / 2);
    //        //if (!Physics2D.OverlapCircle(new Vector2(TilePos.x + DispPos_X, TilePos.y - DispPos_Y), ObstacleOffset))
    //        //    Gizmos.DrawCube(new Vector2(TilePos.x + DispPos_X, TilePos.y - DispPos_Y), new Vector3(DisplayDist, DisplayDist, 0.0001f));

    //    }
    //}
    #endregion


    public static Node ClosestNode(Vector2 pos)
    {
        //watch.Start();
        List<Node> nos = NodeMap.FindAll((n) => { return Mathf.Floor(n.Position.x) == Mathf.Floor(pos.x) && (float)Math.Ceiling(n.Position.y) == (float)Math.Ceiling(pos.y); });
        Node no =  new Node(new Vector2(Mathf.Infinity, Mathf.Infinity));
        foreach(Node tmp in nos)
        {
            no = Vector2.Distance(tmp.Position, pos) < Vector2.Distance(no.Position, pos) ? tmp : no;
        }
        //Debug.Log($"Closest Node: {watch.ElapsedMilliseconds}");
        //watch.Stop();
        return no;
    }

    public static Node RequestPath(Node Start, Node Target, EntityState Enemytype)
    {
        //watch.Start();
        #region Original
        //OpenNodes.Clear();
        //ClosedNodes.Clear();
        //OpenNodes.Add(Start);
        //while (OpenNodes.Count > 0)
        //{
        //    Node CurrentNode = OpenNodes[0];
        //    for (int i = 0; i < OpenNodes.Count; i++)
        //    {
        //        if (OpenNodes[i].F <= CurrentNode.F)
        //            if (OpenNodes[i].H < CurrentNode.H)
        //                CurrentNode = OpenNodes[i];
        //    }

        //    OpenNodes.Remove(CurrentNode);
        //    ClosedNodes.Add(CurrentNode);

        //    if (CurrentNode == Target)
        //    {
        //        return CurrentNode;
        //    }
        //    else
        //    {
        //        CurrentNode.ChildrenUpdate();
        //        foreach (Node node in CurrentNode.Children)
        //        {
        //            if (Enemytype == EntityState.WALK)
        //            {
        //                if (node.state == TileStates.OBSTACLE || node.state == TileStates.FLYABLE || ClosedNodes.Contains(node))
        //                    continue;
        //            }
        //            else if (node.state == TileStates.OBSTACLE || ClosedNodes.Contains(node))
        //                continue;

        //            if (CurrentNode.G + Vector2.Distance(CurrentNode.Position, node.Position) < node.G || !OpenNodes.Contains(node))
        //            {
        //                node.G = CurrentNode.G + Vector2.Distance(CurrentNode.Position, node.Position);
        //                node.UpdateNode(Target.Position);
        //                node.Parent = CurrentNode;
        //                CurrentNode.ActiveChild = node;

        //                if (!OpenNodes.Contains(node))
        //                    OpenNodes.Add(node);
        //            }
        //        }
        //    }
        //}
        //return new Node(new Vector2(Mathf.Infinity, Mathf.Infinity));
        #endregion

        Open_FHeap.WipeHeap();
        Closed_FHeap.WipeHeap();
        Open_FHeap.insert(new FibHeapNode(Start));


        while (Open_FHeap.Count > 0)
        {
            //pop min does a 2 for one. gets the lowest, and pulls it
            //ince the min is always the root, this is O(1)
            //and reduces the code by 8 lines
            FibHeapNode CurrentNode_fib = Open_FHeap.PopMin();
            Node CurrentNode = CurrentNode_fib.Value;

            Closed_FHeap.insert(CurrentNode_fib);


            if (CurrentNode == Target)
            {
                return CurrentNode;
            }
            else
            {
                CurrentNode.ChildrenUpdate();
                foreach (Node node in CurrentNode.Children)
                {
                    if (Enemytype == EntityState.WALK)
                    {
                        if (node.state == TileStates.OBSTACLE || node.state == TileStates.FLYABLE || Closed_FHeap.Contains(node))
                            continue;
                    }
                    else if (node.state == TileStates.OBSTACLE || Closed_FHeap.Contains(node))
                        continue;

                    if (CurrentNode.G + Vector2.Distance(CurrentNode.Position, node.Position) < node.G || !Open_FHeap.Contains(node))
                    {
                        node.G = CurrentNode.G + Vector2.Distance(CurrentNode.Position, node.Position);
                        node.UpdateNode(Target.Position);
                        node.Parent = CurrentNode;
                        //CurrentNode.ActiveChild = node;

                        if (!Open_FHeap.Contains(node))
                            Open_FHeap.insert(new FibHeapNode(node));
                    }
                }
            }
        }
        return new Node(new Vector2(Mathf.Infinity, Mathf.Infinity));


    }

    public static Node[] ReversePath(Node Start, Node Path)
    {
        if (Path.Position == new Vector2(Mathf.Infinity, Mathf.Infinity))
            return new Node[0];

        List<Node> paths_nodes = new List<Node>();
        Node CurrentNode = Path;
        while (CurrentNode != Start)
        {
            paths_nodes.Add(CurrentNode);
            CurrentNode = CurrentNode.Parent;
        }
        if (CurrentNode == Start)
        {
            paths_nodes.Add(CurrentNode);
            paths_nodes.Reverse();
        }
        return paths_nodes.ToArray();
    }
}

