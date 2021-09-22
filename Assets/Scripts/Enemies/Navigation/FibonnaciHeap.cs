using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AStar;

public class FibonnaciHeap
{
    private FibHeapNode MinimumNode_ { get; set; }                  //AKA. Root
    private List<FibHeapNode> LazyList_ = new List<FibHeapNode>();  //Lazy list that stores more tree's
    private List<Node> ContentList = new List<Node>();  //content list for contain checking

    private int nodecount_ { get { return LazyList_.Count; } }
    public int Count { get { return nodecount_; } }

    public void insert(FibHeapNode n)
    {
        if (MinimumNode_ != null)
        {
            LazyList_.Add(n);
            MinimumNode_ = n.Value.F < MinimumNode_.Value.F ? n : MinimumNode_;
        }
        else
        {
            MinimumNode_ = n;
            LazyList_.Add(n);
        }
        ContentList.Add(n.Value);
        CleanRoot();
    }

    public FibHeapNode PopMin()
    {
        if (MinimumNode_ != null)
        {
            //we move the children to the lazy list
            LazyList_.AddRange(MinimumNode_.Children);
            //remove the children
            MinimumNode_.Children.Clear();

            LazyList_.Remove(MinimumNode_);
            FibHeapNode retValue = MinimumNode_;
            ContentList.Remove(retValue.Value);

            CleanRoot();
            
            //nodecount_--;
            return retValue;
        }
        return null;
    }



    //Use this rarely
    public void ChangeItem(ref FibHeapNode refNode, FibHeapNode newValues)
    {
        //refNode = newNode;
        refNode.Value = newValues.Value;


        //-----------------
        //violation check
        //-----------------
        if (refNode.Parent != null) //Value isn't a root node
        {
            if (refNode.Value.F < refNode.Parent.Value.F)
            {
                //-----------------
                //heap violated
                //-----------------

                refNode.Parent.LostChildren++;
                refNode.Parent.Children.Remove(refNode);

                if (refNode.Parent.LostChildren == 2)
                {
                    //Parent node gets disowned
                    refNode.Parent.Parent.Children.Remove(refNode.Parent);
                    refNode.Parent.Parent = null;
                    refNode.Parent.LostChildren = 0;
                    LazyList_.Add(refNode.Parent);
                }

                refNode.Parent = null;
                refNode.LostChildren = 0;
                LazyList_.Add(refNode);//turn refNode into a new root

            }
        }

    }


    //DO NOT USE THESE IN GENERAL CONDITIONS
    //-------------------------------
    public FibHeapNode MatchNode(Predicate<FibHeapNode> predicate)
    {
        //gonna check if the node is in the root. this is fine, just means MatchNode best case is O(n)
        foreach (FibHeapNode Node_ in LazyList_)
        {
            if (predicate.Invoke(Node_))
            {
                return Node_;
            }
        }

        //this is the problem child. now we're at foreach node, recurse their children and their children till something comes up
        FibHeapNode tmp = null;
        foreach (FibHeapNode cnode in LazyList_)
        {
            //Console.WriteLine(cnode.Value.F);
            tmp = RecursiveSearch(cnode, predicate);
            if (tmp != null)
                return tmp;
        }

        return null;
    }

    private FibHeapNode RecursiveSearch(FibHeapNode croot, Predicate<FibHeapNode> predicate)
    {
        if (predicate.Invoke(croot))
            return croot;

        FibHeapNode tmp = null;
        if (croot.Children.Count > 0)
        {
            foreach (FibHeapNode heapNode in croot.Children)
            {
                //Console.WriteLine(heapNode.Value.F);
                tmp = RecursiveSearch(heapNode, predicate);
                if (tmp != null)
                    return tmp;
            }
        }
        return null;
    }
    //-------------------------------


    private void CleanRoot()
    {
        #region
        //for (int i = 0; i < nodecount_; i++)
        //{
        //    //i is out current comparing node
        //    for (int j = 0; j < nodecount_; j++)
        //    {
        //        //j is our node to compare with

        //        //skip the matching node
        //        if (i==j)
        //            continue;

        //        if(LazyList_[i].Degree == LazyList_[j].Degree)
        //        {
        //            //compare and add i/j appropriately to the child list
        //            if(LazyList_[i].Value.F < LazyList_[j].Value.F)
        //            {

        //            }
        //            else if (LazyList_[i].Value.F > LazyList_[j].Value.F)//we want to ignore node tree's whos root value is = to the value of the comparing tree
        //            {

        //            }


        //            //if the degree's are the same, reset i to 0 so we can reiterate
        //            i = 0;
        //            j = 0;
        //        }

        //    }
        //}
        #endregion

        //List<FibHeapNode> fibHeapNodes = new List<FibHeapNode>(nodecount_);
        int iterative = nodecount_ - 1;

        while (iterative > 0)
        {
            int iter2 = iterative;
            //from the lazy, find a root of the same degree
            FibHeapNode heapNode = LazyList_.Find((nde) =>
            {
                return LazyList_[iter2].Degree == nde.Degree && LazyList_[iter2] != nde;
            });

            //heap nodes been found
            if (heapNode != null)
            {
                if (LazyList_[iter2].Value.F < heapNode.Value.F)
                {
                    //The found node is larger then the current node, so add it as a child to current node
                    heapNode.Parent = LazyList_[iter2];
                    LazyList_[iter2].Children.Add(heapNode);
                    LazyList_.Remove(heapNode);
                }
                else if (LazyList_[iter2].Value.F > heapNode.Value.F)//we want to ignore node tree's whos root value is = to the value of the comparing tree
                {
                    //else the found is smaller then current node, so do the opposite

                    FibHeapNode fibHeapNode = LazyList_.Find((nde) => { return nde == heapNode; });
                    LazyList_[iter2].Parent = fibHeapNode;
                    fibHeapNode.Children.Add(LazyList_[iter2]);

                    LazyList_.Remove(LazyList_[iter2]);
                }
            }
            iterative--;
        }
        if (nodecount_ > 0)
        {
            float compare = float.PositiveInfinity;
            FibHeapNode compareNode = LazyList_[0];
            foreach (FibHeapNode fibnode in LazyList_)
            {
                if (fibnode.Value.F < compare)
                {
                    compareNode = fibnode;
                    compare = fibnode.Value.F;
                }
            }
            MinimumNode_ = compareNode;
        }
    }

    public void WipeHeap()
    {
        LazyList_.Clear();
        ContentList.Clear();
    }

    public bool Contains(Node NavNode)
    {
        return ContentList.Contains(NavNode);
        ////gonna check if the node is in the root. this is fine, just means MatchNode best case is O(n)
        //foreach (FibHeapNode Node_ in LazyList_)
        //{
        //    if (Node_.Value == NavNode)
        //    {
        //        return true;
        //    }
        //}

        ////this is the problem child. now we're at foreach node, recurse their children and their children till something comes up
        //FibHeapNode tmp = null;
        //foreach (FibHeapNode cnode in LazyList_)
        //{
        //    //Console.WriteLine(cnode.Value.F);
        //    tmp = RecursiveSearch(cnode, new Predicate<FibHeapNode>((f_node) => { return f_node.Value == NavNode; }));
        //    if (tmp != null)
        //        return true;
        //}

        //return false;
    }
}


public class FibHeapNode
{
    public int LostChildren = 0;
    public List<FibHeapNode> Children = new List<FibHeapNode>();
    public FibHeapNode Parent = null;

    public FibHeapNode(Node node)
    {
        Value = node;
    }
    public Node Value;
    public int Degree { get { return Children.Count; } }
}
