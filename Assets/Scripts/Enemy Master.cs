using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMaster : MonoBehaviour
{
    private float MoveDelay;
    private float MoveTime;
    public float MoveSpeed;
    private Transform target;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(0 >= MoveTime) {
            transform.position = Vector2.MoveTowards(transform.position, target.position, MoveSpeed * Time.deltaTime);

        }else if(Time.timeSinceLevelLoad > MoveTime){
            transform.position = Vector2.MoveTowards(transform.position, target.position, MoveSpeed);
        }
    }
}
