using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat_FSM : MonoBehaviour
{
    private GameObject this_gm_obj { get => gameObject; }
    private GameObject PlayerRef { get => PlayerEntity.Instance.gameObject; }

    public float DetectionRange = 5;



    private FSM_Struct MyMachine = new FSM_Struct();
    private void Define_states()
    {
        MyMachine.States = new Dictionary<int, FSM_States>()
        {
            {
                0, new FSM_States()
                {
                    Description = "Chase condition",
                    StateLogic = Chase(),
                    StatePredicate = (fsmd) => { return fsmd.Distance < DetectionRange && fsmd.Distance > 0.5f; }
                }
            },
            {
                1, new FSM_States()
                {
                    Description = "Idle state (should be last as a final condition)",
                    StateLogic = Idle(),
                    StatePredicate = (fsmd) => { return true; }
                }
            }
        };
    }


    Rigidbody2D rb2d { get => gameObject.GetComponent<Rigidbody2D>(); }
    private float MoveSpeed { get => GetComponent<iCreatureInterface>().EntitySpeed_; }
    private FSM_Datapass fsmdp = new FSM_Datapass();


    bool InView { get => HitPlayerFirst(); }
    bool wasInView = false;

    IEnumerator Idle() 
    {
        Debug.Log($"{transform.name} is Idle");
        while (true) 
        {
            if (InView)
            {
                fsmdp = new FSM_Datapass()
                {
                    Caller = this_gm_obj,
                    Target = PlayerRef,
                    UsePos = false,
                    TargetPos = fsmdp.TargetPos
                };
                wasInView = true;
            }
            else if (wasInView)
            {
                fsmdp = new FSM_Datapass()
                {
                    Caller = this_gm_obj,
                    Target = null,
                    UsePos = true,
                    TargetPos = fsmdp.TargetPos
                };
                wasInView = false;
            }
            yield return new WaitForEndOfFrame(); 
        } 
    }

    bool HitPlayerFirst()
    {
        
        RaycastHit2D[] hit2d_a = Physics2D.RaycastAll(gameObject.transform.position, PlayerRef.transform.position - gameObject.transform.position, DetectionRange);
        hit2d_a = Array.FindAll(hit2d_a, (h) => { return h.collider.gameObject.layer == LayerMask.NameToLayer("WALL") || h.collider.gameObject == PlayerRef; });
        Array.Sort<RaycastHit2D>(hit2d_a, (ray1, ray2) => { return ray1.distance.CompareTo(ray2.distance); });

        RaycastHit2D hit2d = new RaycastHit2D();
        for (int i = 0; i < hit2d_a.Length; i++)
        {
            if (hit2d_a[i].collider.gameObject.layer == LayerMask.NameToLayer("WALL"))
                return false;
            else if (hit2d_a[i].collider.gameObject == PlayerRef)
            {
                //Debug.Log($"{transform.name} sees Player");
                hit2d = hit2d_a[i];
                break;
            }
            else if (hit2d_a[i].collider.gameObject == gameObject){

            }
        }

        if (hit2d.collider != null)
            return true;
        return false;
    }

    IEnumerator Chase()
    {
        Debug.Log($"{transform.name} is Chasing");
        while (true)
        {
            
            if (Vector2.Distance(PlayerRef.transform.position, this_gm_obj.transform.position) <= DetectionRange && fsmdp.UsePos == false)
            {
                fsmdp.TargetPos = PlayerRef.transform.position;
            }


            if (fsmdp.UsePos)
            {
                
                Vector2 norm_1 = (fsmdp.TargetPos - this_gm_obj.transform.position).normalized;
                Vector2 tmp = norm_1 * Time.deltaTime * MoveSpeed * 1000;

                if(Vector2.Distance(this_gm_obj.transform.position, fsmdp.TargetPos) < 0.5f)
                {
                    rb2d.velocity = Vector2.zero;
                    this_gm_obj.transform.position = fsmdp.TargetPos;
                } else {
                    rb2d.velocity = (tmp);
                    Debug.Log($"{transform.name} sees Player : Velocity: {tmp.magnitude}");
                }
                    

            }
            else
            {
                if(fsmdp.Target == null)
                {
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    Vector2 norm_1 = (fsmdp.Target.transform.position - this_gm_obj.transform.position).normalized;
                    Vector2 tmp = norm_1 * Time.deltaTime * MoveSpeed * 1000;
                    rb2d.velocity = (tmp);
                    Debug.Log($"{transform.name} sees Player : Velocity: {tmp.magnitude}");
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    void Start()
    {
        Define_states();
        fsmdp = new FSM_Datapass()
        {
            Caller = this_gm_obj,
            Target = null,
            UsePos = false
        };
    }

    void Update()
    {
        this_gm_obj.layer = 6;
        MyMachine.UpdateState(fsmdp);
        MyMachine.RunState(this);
    }
}
