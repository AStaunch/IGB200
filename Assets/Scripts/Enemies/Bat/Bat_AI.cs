using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEditor;
using UnityEngine;

using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Bat_AI : MonoBehaviour
{
    #region Notes

    //States are as follows
    //---------------------
    // ‣ Stationary
    // ‣ Tracking
    // ‣ Chasing

    //Tracking
    //<===================>
    //Tracking is where the bat has no line of sight on the player, or the player has dropped out of visual range
    //if this happens, the bat will go to the last known point
    //pretty basic shit

    //Chasing
    //<===================>
    //litterally tracking but the player is in view, so the bat just goes for it
    #endregion


    //AStar.EntityState BatState = AStar.EntityState.FLY;

    Rigidbody2D rb2d;

    enum ValidStates
    {
        Idle,
        Targeting
    }

    ValidStates NextState;
    ValidStates CurrentState;

    Coroutine ActiveFunction;


    public float DetectionRange;
    public float Min_distance_from_target;
    public float Min_distance_from_target_variation;

    //AStar.Node BatNode;

    private GameObject PlayerRef;

    public float MoveSpeed;


    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        NextState = ValidStates.Idle;
        MoveSpeed = GetComponent<EntityManager>().entitySpeed;
    }



    Vector2 LastViewedPos;
    void Update()
    {
        if (Vector2.Distance(PlayerRef.transform.position, gameObject.transform.position) <= DetectionRange)
        {
            RaycastHit2D hit2d = Physics2D.Raycast(gameObject.transform.position, PlayerRef.transform.position - gameObject.transform.position);
            Debug.DrawRay(gameObject.transform.position, PlayerRef.transform.position - gameObject.transform.position);
            if(hit2d.collider != null && hit2d.collider.gameObject == PlayerRef)
            {
                if (CurrentState != ValidStates.Targeting)
                {
                    NextState = ValidStates.Targeting;
                }
                LastViewedPos = hit2d.collider.gameObject.transform.position;
            }
        }
        else
        {
            if (CurrentState != ValidStates.Idle && Vector2.Distance(LastViewedPos, gameObject.transform.position) <= Min_distance_from_target)
            {
                NextState = ValidStates.Idle;
            }
        }




        if(CurrentState != NextState)
        {
            CurrentState = NextState;
            if (ActiveFunction != null)//null by defualt so
                StopCoroutine(ActiveFunction);

            if (CurrentState == ValidStates.Idle)
                ActiveFunction = StartCoroutine(Idle());
            if (CurrentState == ValidStates.Targeting)
                ActiveFunction = StartCoroutine(Tracking_Chasing(Random.Range(0, Min_distance_from_target_variation)));
        }

    }

    //Stopwatch stopwatch= new Stopwatch();

    public IEnumerator Tracking_Chasing(float variation)
    {
        while (true)
        {
            if (Vector2.Distance(gameObject.transform.position, LastViewedPos) <= Min_distance_from_target - variation || Vector2.Distance(gameObject.transform.position, LastViewedPos) > DetectionRange)
            {
                NextState = ValidStates.Idle;
            }
            else
            {
                #region Had pathfinding
                //BatNode = AStar.ClosestNode(gameObject.transform.position);
                //stopwatch.Reset();
                //stopwatch.Start();

                //AStar.Node Path = AStar.RequestPath(BatNode, AStar.ClosestNode(LastViewedPos), BatState);
                //AStar.Node[] nodes = AStar.ReversePath(BatNode, Path);
                //gameObject.transform.position += ((Vector3)nodes[1].Position - gameObject.transform.position) * MoveSpeed * Time.deltaTime;

                //pos.transform.position = nodes[1].Position;
                //Debug.Log($"{stopwatch.ElapsedMilliseconds}");
                //stopwatch.Stop();
                //Handles.DrawLine(Path.Position, Path.Parent.Position);
                //Handles.DrawWireDisc(Path.Position, Vector3.forward, 0.02f);
                //AStar.Node[] nodes = AStar.ReversePath(BatNode, AStar.RequestPath(BatNode, AStar.ClosestNode(SeenPos), BatState));

                //Debug.Log($"Bat World Pos: {BatNode.Position} - Next Node: {nodes[0]}");
                //foreach(AStar.Node node_ in nodes)
                //{
                //    Handles.DrawWireDisc(node_.Position, Vector3.forward, 0.05f);
                //}

                //gameObject.GetComponent<Rigidbody2D>().MovePosition(nodes[0].Position);
                //gameObject.transform.position += (((Vector3)nodes[0].Position - gameObject.transform.position) * Time.deltaTime * MoveSpeed);
                #endregion
                #region no pathfinding - just chase
                gameObject.transform.position += ((Vector3)LastViewedPos - gameObject.transform.position) * MoveSpeed * Time.deltaTime;
                #endregion
            }


            yield return new WaitForFixedUpdate();
        }
        //Tracking_Chasing(SeenPos, variation);
    }

    

    public IEnumerator Idle()
    {
        

        yield return new WaitForEndOfFrame();
    }

    private void OnDrawGizmos()
    {
        //if (Selection.activeObject == transform.gameObject) {
        //    Handles.DrawWireDisc(gameObject.transform.position, Vector3.forward, DetectionRange);
        //    Handles.color = Color.yellow;
        //    Handles.DrawWireDisc(gameObject.transform.position, Vector3.forward, Min_distance_from_target);
        //    Handles.color = Color.white;

        //    Handles.Label(gameObject.transform.position + new Vector3(-0.25f, 1, 0), CurrentState.ToString());
        //}
    }

    

}
