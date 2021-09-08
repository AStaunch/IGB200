using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar_test : MonoBehaviour
{
    public bool Test = false;
    public GameObject host;
    public GameObject PathTo;
    public LineRenderer LineRenderer;

    public void Update()
    {
        if (Test)
        {
            
            AStar.Node Me = host.GetComponent<AStar>().ClosestNode(gameObject.transform.position);
            AStar.Node Target = host.GetComponent<AStar>().ClosestNode(PathTo.transform.position);

            //host.GetComponent<AStar>().RequestPath(Me, Target);

            AStar.Node CurrentNode = host.GetComponent<AStar>().RequestPath(Me, Target);
            List<Vector3> path = new List<Vector3>();
            while (CurrentNode != Me)
            {
                path.Add(CurrentNode.Position);
                //Debug.Log($"{CurrentNode.Position.x} | {CurrentNode.Position.y}");
                CurrentNode = CurrentNode.Parent;
            }
            if (CurrentNode == Me)
            {
                path.Add(CurrentNode.Position);
                //Debug.Log($"{CurrentNode.Position.x} | {CurrentNode.Position.y} - Start");
            }
            LineRenderer.positionCount = path.Count;
            LineRenderer.SetPositions(path.ToArray());
            //host.GetComponent<AStar>().RenderPath = true;
            //Debug.Log($"{Me.Position.x} - {n.Position.y}");
        }
    }
}
