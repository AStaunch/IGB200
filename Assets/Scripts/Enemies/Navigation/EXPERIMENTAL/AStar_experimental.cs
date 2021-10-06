using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Jobs;
using Unity.Collections;
using System.Runtime.InteropServices;
using JobTask_syst;

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
            H = (TargetPos.x - Position.x) + (TargetPos.y - Position.y);
        }

        public void ChildrenUpdate()
        {
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

    public static Node Result_;


    class PathJob : JobTask
    {
        public PathWrap wraps;
        public GCHandle resAllocation;
        public PathResult pathResults { get { return (PathResult)resAllocation.Target; } set { resAllocation.Target = value; } }

        public void Execute()
        {
            Node Start = wraps.Start;
            Node Target = wraps.Target;

            Open_FHeap.WipeHeap();
            Closed_FHeap.WipeHeap();
            Open_FHeap.insert(new FHeap_experimental.FibHeapNode(Start));

            Node Result = new Node(new Vector2(Mathf.Infinity, Mathf.Infinity)); ;

            while (Open_FHeap.count > 0)
            {
                FHeap_experimental.FibHeapNode CurrentNode_fib = Open_FHeap.popmin();
                Node CurrentNode = CurrentNode_fib.Value;

                Closed_FHeap.insert(CurrentNode_fib);

                if (CurrentNode == Target)
                {
                    Result = CurrentNode;
                    break;
                }
                else
                {
                    CurrentNode.ChildrenUpdate();
                    foreach (Node node in CurrentNode.Children)
                    {
                        if (wraps.State == EntityState.WALK)
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

                            if (!Open_FHeap.Contains(node))
                                Open_FHeap.insert(new FHeap_experimental.FibHeapNode(node));
                        }
                    }
                }
            }
            pathResults = new PathResult() { Path = Result };
        }

        public NativeArray<PathResult> returnv()
        {
            NativeArray<PathResult> r = new NativeArray<PathResult>(1, Allocator.TempJob);
            r[0] = pathResults;
            return r;
        }
    }

    struct PathWrap
    {
        public Node Start;
        public Node Target;
        public EntityState State;
    }

    public struct PathResult
    {
        public Node Path;
    }


    public static IEnumerator RequestPath(Node Start, Node Target, EntityState Enemytype)
    {
        //Predefine an allocated section of memory for Garbage collection to ignore
        GCHandle res = GCHandle.Alloc(new PathResult());

        //Create a JobTask to wrap the existing job system for multi-threading
        JobTask task = new PathJob() { wraps = new PathWrap() { Start = Start, State = Enemytype, Target = Target }, resAllocation = res };
        //allocate memory for the JobTask
        GCHandle gch = GCHandle.Alloc(task);
        
        //schedule the JobWrap for a JobHandle so we schedule and complete off unity's main thread
        JobHandle jbh = new JobWrap()
        {
            Handle = gch
        }.Schedule();
        jbh.Complete();

        yield return new WaitUntil(() => jbh.IsCompleted);

        //this IEnumerator is running inside Corontine_wrap, so this allows us to return the result, which is still allocated in gc
        yield return ((PathResult)res.Target).Path;

        //obviously we need to release the memory. rather not bluescreen due to memory usage exceeding the rams cap
        gch.Free();
        res.Free();
    }


    public static IEnumerator ReversePath(Node Start, Node Path)
    {
        if (Path.Position == new Vector2(Mathf.Infinity, Mathf.Infinity))
            yield return new Node[0];

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
        yield return paths_nodes[0];
    }
}

