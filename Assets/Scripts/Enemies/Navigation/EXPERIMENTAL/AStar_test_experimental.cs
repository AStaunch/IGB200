using System;
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
            StartCoroutine(MakeRequest());
            Test = false;
        }
    }


    public IEnumerator MakeRequest()//(AStar_experimental.Node start, AStar_experimental.Node targ)
    {
        //would use a try catch here, but its not allowed to have a yield in it, so going to wrap the casts of the result in one since corontine_wrap doesn't give a fuck about response type
        Corontine_wrap wrapped_Path;
        while (true)
        {
            AStar_experimental.Node Me = AStar_experimental.ClosestNode(gameObject.transform.position);
            AStar_experimental.Node Target = AStar_experimental.ClosestNode(PathTo.transform.position);
            wrapped_Path = new Corontine_wrap(this, AStar_experimental.RequestPath(Target, Me, EnemyMovement));//this is reversed so we dont need ReversePath. This is also called a Reverse Astar
            yield return new WaitUntil(() => wrapped_Path.IsDone);
            CoroutineResult resu = wrapped_Path.Results;
            AStar_experimental.Node PathNode = ((AStar_experimental.Node)resu.Result);
            if (PathNode != Target)
            {
                transform.position += (((Vector3)PathNode.Parent.Position) - transform.position) * Time.deltaTime * 5;
            }
                
            yield return new WaitForSecondsRealtime(2.5f);
        }
        
    }
}
