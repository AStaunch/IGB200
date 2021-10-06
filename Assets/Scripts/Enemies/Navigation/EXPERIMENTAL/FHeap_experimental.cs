using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.Serialization;

public static class FHeap_experimental
{
    public class FibonnaciHeap
    {
        private FibHeapNode MinNode_;//This is main root
        private Dictionary<float,FibHeapNode> TestLazy = new Dictionary<float,FibHeapNode>();
        public int count { get { return TestLazy.Count; } }


        public void insert(FibHeapNode n)
        {
            if (TestLazy.ContainsKey(n.index))
                return;

            if(MinNode_ == null)
            {
                MinNode_ = n;
                TestLazy.Add(n.index, n);
            }
            else
            {
                if(MinNode_ < n)
                    MinNode_ = n;
                TestLazy.Add(n.index, n);
            }
            //if (count >= 4 && count % 4 == 0)
            //    CleanRoot();
        }

        private void CleanRoot()
        {
            int iterative = count - 1;

            while (iterative > 0)
            {
                KeyValuePair<float,FibHeapNode> n = TestLazy.ElementAt(iterative);
                List<KeyValuePair<float, FibHeapNode>> ns = TestLazy.Where((node) => { return node.Value.Degree == n.Value.Degree; }).ToList();
                if (ns != null && ns.Count > 1)
                {
                    if(ns.Count % 2 != 0)
                        ns.RemoveAt(ns.Count - 1);
                    
                    for(int i = 0; i < ns.Count; i++)
                    {
                        Debug.Log(ns.Count);
                        ns[0].Value.Children.Add(ns[1].Value);
                        ns.RemoveRange(0, 1);
                    }
                }
                else
                    continue;
            }
            if (count > 0)
            {
                FibHeapNode CurrentVal = TestLazy.Values.First();
                foreach(FibHeapNode n in TestLazy.Values)
                {
                    if (CurrentVal > n)
                        CurrentVal = n;
                }
                MinNode_ = CurrentVal;
            }
        }

        public FibHeapNode popmin()
        {
            if (MinNode_ != null)
            {
                //we move the children to the lazy list
                foreach (FibHeapNode n in MinNode_.Children)
                {
                    TestLazy.Add(n.index,n);
                }

                //remove the children
                MinNode_.Children.Clear();

                TestLazy.Remove(MinNode_.index);
                FibHeapNode retValue = MinNode_;

                //CleanRoot();

                if (count > 0)
                {
                    FibHeapNode CurrentVal = TestLazy.Values.First();
                    foreach (FibHeapNode n in TestLazy.Values)
                    {
                        if (CurrentVal > n)
                            CurrentVal = n;
                    }
                    MinNode_ = CurrentVal;
                }

                return retValue;
            }
            return null;
        }

        public void WipeHeap()
        {
            TestLazy.Clear();
        }

        //swapping to this since index = pos.x + pos.y * 5
        public bool Contains(AStar_experimental.Node n)
        {
            foreach(FibHeapNode fib in TestLazy.Values.ToArray())
            {
                if (fib.Value.Position == n.Position)
                {
                    return true;
                }
            }
            return false;

           // if(TestLazy.Values.Where((node) => { return node.Value_.Position == n.Position; }) != null)
           //     Debug.Log($"{n.Position} : true");
           // return TestLazy.Values.Where((node) => { return node.Value_.Position == n.Position; }) != null ;
        }

        #region original
        //private FibHeapNode MinimumNode_ { get; set; }                  //AKA. Root
        //private FibHeapNode[] LazyList_ = new FibHeapNode[0];  //Lazy list that stores more tree's
        //private List<AStar_experimental.Node> ContentList = new List<AStar_experimental.Node>();  //content list for contain checking

        //private int nodecount_ { get { return LazyList_.Length; } }
        //public int Count { get { return nodecount_; } }
        #region
        //public void insert(FibHeapNode n)
        //{
        //    if (MinimumNode_ != null)
        //    {
        //        LazyList_.SetValue(n,LazyList_.Length);
        //        MinimumNode_ = n.Value.F < MinimumNode_.Value.F ? n : MinimumNode_;
        //    }
        //    else
        //    {
        //        MinimumNode_ = n;
        //        LazyList_.SetValue(n, Count);
        //    }
        //    ContentList.Add(n.Value);
        //    CleanRoot();
        //}
        #endregion insert
        #region popmin
        //public FibHeapNode PopMin()
        //{
        //    if (MinimumNode_ != null)
        //    {
        //        //we move the children to the lazy list
        //        foreach (FibHeapNode n in MinimumNode_.Children) {
        //            LazyList_.SetValue(n, Count);
        //        }

        //        //remove the children
        //        MinimumNode_.Children.Clear();

        //        LazyList_.SetValue(null, Array.IndexOf(LazyList_, MinimumNode_));
        //        FibHeapNode retValue = MinimumNode_;
        //        ContentList.Remove(retValue.Value);

        //        CleanRoot();

        //        //nodecount_--;
        //        return retValue;
        //    }
        //    return null;
        //}
        #endregion

        #region thing
        ////Use this rarely
        //public void ChangeItem(ref FibHeapNode refNode, FibHeapNode newValues)
        //{
        //    //refNode = newNode;
        //    refNode.Value = newValues.Value;


        //    //-----------------
        //    //violation check
        //    //-----------------
        //    if (refNode.Parent != null) //Value isn't a root node
        //    {
        //        if (refNode.Value.F < refNode.Parent.Value.F)
        //        {
        //            //-----------------
        //            //heap violated
        //            //-----------------

        //            refNode.Parent.LostChildren++;
        //            refNode.Parent.Children.Remove(refNode);

        //            if (refNode.Parent.LostChildren == 2)
        //            {
        //                //Parent node gets disowned
        //                refNode.Parent.Parent.Children.Remove(refNode.Parent);
        //                refNode.Parent.Parent = null;
        //                refNode.Parent.LostChildren = 0;
        //                LazyList_.SetValue(refNode.Parent, Count);
        //            }

        //            refNode.Parent = null;
        //            refNode.LostChildren = 0;
        //            LazyList_.SetValue(refNode, Count);//turn refNode into a new root

        //        }
        //    }

        //}


        ////DO NOT USE THESE IN GENERAL CONDITIONS
        ////-------------------------------
        //public FibHeapNode MatchNode(Predicate<FibHeapNode> predicate)
        //{
        //    //gonna check if the node is in the root. this is fine, just means MatchNode best case is O(n)
        //    foreach (FibHeapNode Node_ in LazyList_)
        //    {
        //        if (predicate.Invoke(Node_))
        //        {
        //            return Node_;
        //        }
        //    }

        //    //this is the problem child. now we're at foreach node, recurse their children and their children till something comes up
        //    FibHeapNode tmp = null;
        //    foreach (FibHeapNode cnode in LazyList_)
        //    {
        //        //Console.WriteLine(cnode.Value.F);
        //        tmp = RecursiveSearch(cnode, predicate);
        //        if (tmp != null)
        //            return tmp;
        //    }

        //    return null;
        //}

        //private FibHeapNode RecursiveSearch(FibHeapNode croot, Predicate<FibHeapNode> predicate)
        //{
        //    if (predicate.Invoke(croot))
        //        return croot;

        //    FibHeapNode tmp = null;
        //    if (croot.Children.Count > 0)
        //    {
        //        foreach (FibHeapNode heapNode in croot.Children)
        //        {
        //            //Console.WriteLine(heapNode.Value.F);
        //            tmp = RecursiveSearch(heapNode, predicate);
        //            if (tmp != null)
        //                return tmp;
        //        }
        //    }
        //    return null;
        //}
        ////-------------------------------
        #endregion

        #region clean shit
        #region cleanroot
        //private void CleanRoot()
        //{

        //    int iterative = nodecount_ - 1;

        //    while (iterative > 0)
        //    {
        //        int iter2 = iterative;
        //        //from the lazy, find a root of the same degree
        //        FibHeapNode heapNode = Array.Find(LazyList_, (nde) =>
        //        {
        //            return LazyList_[iter2].Degree == nde.Degree && LazyList_[iter2] != nde;
        //        });

        //        //heap nodes been found
        //        if (heapNode != null)
        //        {
        //            if (LazyList_[iter2].Value.F < heapNode.Value.F)
        //            {
        //                //The found node is larger then the current node, so add it as a child to current node
        //                heapNode.Parent = LazyList_[iter2];
        //                LazyList_[iter2].Children.Add(heapNode);
        //                LazyList_.SetValue(null, Array.IndexOf(LazyList_,heapNode));
        //            }
        //            else if (LazyList_[iter2].Value.F > heapNode.Value.F)//we want to ignore node tree's whos root value is = to the value of the comparing tree
        //            {
        //                //else the found is smaller then current node, so do the opposite

        //                FibHeapNode fibHeapNode = Array.Find(LazyList_, (nde) => { return nde == heapNode; });
        //                LazyList_[iter2].Parent = fibHeapNode;
        //                fibHeapNode.Children.Add(LazyList_[iter2]);

        //                LazyList_.SetValue(null, iter2);
        //            }
        //        }
        //        iterative--;
        //    }
        //    if (nodecount_ > 0)
        //    {
        //        float compare = float.PositiveInfinity;
        //        FibHeapNode compareNode = LazyList_[0];
        //        foreach (FibHeapNode fibnode in LazyList_)
        //        {
        //            if (fibnode.Value.F < compare)
        //            {
        //                compareNode = fibnode;
        //                compare = fibnode.Value.F;
        //            }
        //        }
        //        MinimumNode_ = compareNode;
        //    }
        //}
        #endregion
        //public void WipeHeap()
        //{
        //    Array.Clear(LazyList_, 0, Count);
        //    ContentList.Clear();
        //}

        //public bool Contains(AStar_experimental.Node NavNode)
        //{
        //    return ContentList.Contains(NavNode);
        //}
        #endregion
        #endregion

    }

    public class FibHeapNode
    {
        public static bool operator <(FibHeapNode og, FibHeapNode comp) => og.Value_.F < comp.Value.F;
        public static bool operator >(FibHeapNode og, FibHeapNode comp) => og.Value_.F > comp.Value.F;
        public static ObjectIDGenerator objectID = new ObjectIDGenerator();

        private float index_ = Mathf.Infinity;
        public float index { get { return index_ != Mathf.Infinity ? index_ : CalcI(); } }

        private float CalcI()
        {
            index_ = objectID.GetId(this, out _);
            return index_;
        }

        public int LostChildren = 0;
        public List<FibHeapNode> Children = new List<FibHeapNode>();
        public FibHeapNode Parent = null;

        public FibHeapNode(AStar_experimental.Node node)
        {
            Value_ = node;
        }
        public AStar_experimental.Node Value { get { return Value_; } set { Value_ = value; } }
        public AStar_experimental.Node Value_;
        public int Degree { get { return Children.Count; } }
    }
}
