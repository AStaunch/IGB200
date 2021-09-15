using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar_test : MonoBehaviour
{
    public bool Test = false;
    public GameObject host;
    public GameObject PathTo;
    public LineRenderer LineRenderer;
    public AStar.EntityState EnemyMovement;

    public void Update()
    {
        if (Test)
        {
            AStar.Node Me = AStar.ClosestNode(gameObject.transform.position);
            AStar.Node Target = AStar.ClosestNode(PathTo.transform.position);

            AStar.Node[] NodePath = AStar.ReversePath(Me, AStar.RequestPath(Me, Target, EnemyMovement));


            List<Vector3> path = new List<Vector3>();
            foreach(AStar.Node n in NodePath)
            {
                path.Add(n.Position);
            }
            LineRenderer.positionCount = path.Count;
            LineRenderer.SetPositions(path.ToArray());
            //host.GetComponent<AStar>().RenderPath = true;
            //Debug.Log($"{Me.Position.x} - {n.Position.y}");
        }
    }
}
