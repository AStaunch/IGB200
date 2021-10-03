using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public static class AStar_experimental
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
    public static FHeap_experimental.FibonnaciHeap Open_FHeap = new FHeap_experimental.FibonnaciHeap();
    public static FHeap_experimental.FibonnaciHeap Closed_FHeap = new FHeap_experimental.FibonnaciHeap();
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
        PerUnitDist_ = 1 / (float)PerUnitFreq;

        DisplayDist_ = (1 - (DisplayGap * (PerUnitFreq + 1))) / PerUnitFreq;

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
                            Vector2 nodepos = new Vector2((PerUnitDist * w) - (PerUnitDist / 2), -((PerUnitDist * h) - (PerUnitDist / 2)));
                            
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


    public static Node ClosestNode(Vector2 pos)
    {
        List<Node> nos = NodeMap.FindAll((n) => { return Mathf.Floor(n.Position.x) == Mathf.Floor(pos.x) && (float)Math.Ceiling(n.Position.y) == (float)Math.Ceiling(pos.y); });
        Node no =  new Node(new Vector2(Mathf.Infinity, Mathf.Infinity));
        foreach(Node tmp in nos)
        {
            no = Vector2.Distance(tmp.Position, pos) < Vector2.Distance(no.Position, pos) ? tmp : no;
        }
        return no;
    }

    public static Node Start_;
    public static Node Target_;
    public static EntityState Enemytype_;
    public static Node Result_;

    struct PathJob : IJob
    {
        public void Execute()
        {
            Open_FHeap.WipeHeap();
            Closed_FHeap.WipeHeap();
            Open_FHeap.insert(new FHeap_experimental.FibHeapNode(Start_));

            bool Found = false;

            while (Open_FHeap.Count > 0 && !Found)
            {
                FHeap_experimental.FibHeapNode CurrentNode_fib = Open_FHeap.PopMin();
                Node CurrentNode = CurrentNode_fib.Value;

                Closed_FHeap.insert(CurrentNode_fib);


                if (CurrentNode == Target_)
                {
                    Result_ = CurrentNode;
                    Found = true;
                }
                else
                {
                    CurrentNode.ChildrenUpdate();
                    foreach (Node node in CurrentNode.Children)
                    {
                        if (Enemytype_ == EntityState.WALK)
                        {
                            if (node.state == TileStates.OBSTACLE || node.state == TileStates.FLYABLE || Closed_FHeap.Contains(node))
                                continue;
                        }
                        else if (node.state == TileStates.OBSTACLE || Closed_FHeap.Contains(node))
                            continue;

                        if (CurrentNode.G + Vector2.Distance(CurrentNode.Position, node.Position) < node.G || !Open_FHeap.Contains(node))
                        {
                            node.G = CurrentNode.G + Vector2.Distance(CurrentNode.Position, node.Position);
                            node.UpdateNode(Target_.Position);
                            node.Parent = CurrentNode;
                            //CurrentNode.ActiveChild = node;

                            if (!Open_FHeap.Contains(node))
                                Open_FHeap.insert(new FHeap_experimental.FibHeapNode(node));
                        }
                    }
                }
            }
            if(Result_ == null)
            {
                Debug.Log("emptied while");
                Result_ = new Node(new Vector2(Mathf.Infinity, Mathf.Infinity));
            }
        }
    }

    //public static Node RequestPath(Node Start, Node Target, EntityState Enemytype)
    //{
    //    Start_ = Start;
    //    Target_ = Target;
    //    Enemytype_ = Enemytype;

    //    var Path = new PathJob(); //{ Start = Start, Target = Target, Enemytype = Enemytype };
    //    JobHandle job = Path.Schedule();
    //    job.Complete();
    //    return Result_;
    //}

    public static IEnumerator RequestPath(Node Start, Node Target, EntityState Enemytype)
    {
        Start_ = Start;
        Target_ = Target;
        Enemytype_ = Enemytype;

        var Path = new PathJob(); //{ Start = Start, Target = Target, Enemytype = Enemytype };
        JobHandle job = Path.Schedule();
        job.Complete();
        
        yield return new WaitUntil(new Func<bool>(()=> job.IsCompleted));
        yield return true;
    }


    //public static Node RequestPath(Node Start, Node Target, EntityState Enemytype)
    //{
        

    //    Open_FHeap.WipeHeap();
    //    Closed_FHeap.WipeHeap();
    //    Open_FHeap.insert(new FibHeapNode(Start));


    //    while (Open_FHeap.Count > 0)
    //    {
    //        FibHeapNode CurrentNode_fib = Open_FHeap.PopMin();
    //        Node CurrentNode = CurrentNode_fib.Value;

    //        Closed_FHeap.insert(CurrentNode_fib);


    //        if (CurrentNode == Target)
    //        {
    //            return CurrentNode;
    //        }
    //        else
    //        {
    //            CurrentNode.ChildrenUpdate();
    //            foreach (Node node in CurrentNode.Children)
    //            {
    //                if (Enemytype == EntityState.WALK)
    //                {
    //                    if (node.state == TileStates.OBSTACLE || node.state == TileStates.FLYABLE || Closed_FHeap.Contains(node))
    //                        continue;
    //                }
    //                else if (node.state == TileStates.OBSTACLE || Closed_FHeap.Contains(node))
    //                    continue;

    //                if (CurrentNode.G + Vector2.Distance(CurrentNode.Position, node.Position) < node.G || !Open_FHeap.Contains(node))
    //                {
    //                    node.G = CurrentNode.G + Vector2.Distance(CurrentNode.Position, node.Position);
    //                    node.UpdateNode(Target.Position);
    //                    node.Parent = CurrentNode;
    //                    //CurrentNode.ActiveChild = node;

    //                    if (!Open_FHeap.Contains(node))
    //                        Open_FHeap.insert(new FibHeapNode(node));
    //                }
    //            }
    //        }
    //    }
    //    return new Node(new Vector2(Mathf.Infinity, Mathf.Infinity));


    //}

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

