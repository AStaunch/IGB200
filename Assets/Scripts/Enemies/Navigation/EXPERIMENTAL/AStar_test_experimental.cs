using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar_test_experimental : MonoBehaviour
{
    public bool Test = false;
    public GameObject host;
    public GameObject PathTo;
    public LineRenderer LineRenderer;
    public AStar_experimental.EntityState EnemyMovement;

    public void Update()
    {
        if (Test)
        {
            //AStar_experimental.Node Me = AStar_experimental.ClosestNode(gameObject.transform.position);
            //AStar_experimental.Node Target = AStar_experimental.ClosestNode(PathTo.transform.position);

            //AStar_experimental.Node[] NodePath = AStar_experimental.ReversePath(Me, AStar_experimental.RequestPath(Me, Target, EnemyMovement));


            //List<Vector3> path = new List<Vector3>();
            //foreach(AStar_experimental.Node n in NodePath)
            //{
            //    path.Add(n.Position);
            //}
            //LineRenderer.positionCount = path.Count;
            //LineRenderer.SetPositions(path.ToArray());
            ////host.GetComponent<AStar>().RenderPath = true;
            ////Debug.Log($"{Me.Position.x} - {n.Position.y}");

            StartCoroutine("Req");
            Test = false;
        }
    }

    public IEnumerator Req()
    {
        while (true)
        {
            AStar_experimental.Node Me = AStar_experimental.ClosestNode(gameObject.transform.position);
            AStar_experimental.Node Target = AStar_experimental.ClosestNode(PathTo.transform.position);

            StartCoroutine(AStar_experimental.RequestPath(Me, Target, EnemyMovement));
            yield return new WaitUntil(() => AStar_experimental.Result_ != null);
            AStar_experimental.Node[] NodePath = AStar_experimental.ReversePath(Me, AStar_experimental.Result_);


            List<Vector3> path = new List<Vector3>();
            foreach (AStar_experimental.Node n in NodePath)
            {
                path.Add(n.Position);
            }
            LineRenderer.positionCount = path.Count;
            LineRenderer.SetPositions(path.ToArray());
            yield return new WaitForEndOfFrame();
        }
    }
}
