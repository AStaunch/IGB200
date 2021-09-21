using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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


    AStar.EntityState BatState = AStar.EntityState.FLY;

    IEnumerator Active_State;
    IEnumerator LastKnown_State;

    public float DetectionRange;
    public float Min_distance_from_target;
    public float Min_distance_from_target_variation;

    AStar.Node BatNode;

    private GameObject PlayerRef;

    public float MoveSpeed;

    void Start()
    {
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        Active_State = Idle();
        StartCoroutine("Idle");
        LastKnown_State = Active_State;
    }

    void FixedUpdate()
    {
        if (Vector2.Distance(PlayerRef.transform.position, gameObject.transform.position) <= DetectionRange)
        {
            RaycastHit2D hit2d = Physics2D.Raycast(gameObject.transform.position, PlayerRef.transform.position - gameObject.transform.position);
            Debug.DrawRay(gameObject.transform.position, PlayerRef.transform.position - gameObject.transform.position);
            if(hit2d.collider != null && hit2d.collider.gameObject == PlayerRef)
            {
                Active_State = Tracking_Chasing(hit2d.point, Random.Range(0, Min_distance_from_target_variation));
            }
        }




        if(LastKnown_State != Active_State)
        {
            StopCoroutine(LastKnown_State);
            LastKnown_State = Active_State;
            StartCoroutine(LastKnown_State);
        }
    }

    bool tmp = true;
    public IEnumerator Tracking_Chasing(Vector2 SeenPos, float variation)
    {
        while (true)
        {
            if (Vector2.Distance(gameObject.transform.position, SeenPos) <= Min_distance_from_target - variation)
            {
                Active_State = Idle();
            }
            else
            {
                
                if (tmp)
                {
                    BatNode = AStar.ClosestNode(gameObject.transform.position);
                    AStar.Node Path = AStar.RequestPath(BatNode, AStar.ClosestNode(SeenPos), BatState);
                    tmp = false;
                }


                //AStar.Node[] nodes = AStar.ReversePath(BatNode, AStar.RequestPath(BatNode, AStar.ClosestNode(SeenPos), BatState));
                //Debug.Log($"Bat World Pos: {BatNode.Position} - Next Node: {nodes[0]}");
                //foreach(AStar.Node node_ in nodes)
                //{
                //    Handles.DrawWireDisc(node_.Position, Vector3.forward, 0.05f);
                //}

                //gameObject.GetComponent<Rigidbody2D>().MovePosition(nodes[0].Position);
                //gameObject.transform.position += (((Vector3)nodes[0].Position - gameObject.transform.position) * Time.deltaTime * MoveSpeed);

            }


            yield return new WaitForSecondsRealtime(5);
        }
    }

    public IEnumerator Idle()
    {
        

        yield return new WaitForEndOfFrame();
    }

    private void OnDrawGizmos()
    {
        if(Selection.activeObject == transform.gameObject)
        {
            Handles.DrawWireDisc(gameObject.transform.position, Vector3.forward, DetectionRange);
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(gameObject.transform.position, Vector3.forward, Min_distance_from_target);
            Handles.color = Color.white;

            if(Active_State != null)
                Handles.Label(gameObject.transform.position + new Vector3(-1.5f, 1, 0), Active_State.ToString());
        }
    }

    

}
