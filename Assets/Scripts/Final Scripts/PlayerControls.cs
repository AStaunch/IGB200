using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField]
    private float playerSpeed;
    private Vector3 change;
    private Animator anim;
    // Start is called before the first frame update
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        _ = TryGetComponent<Animator>(out anim);
    }

    // Update is called once per frame
    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        if (change != Vector3.zero)
        {
            MoveCharacter(transform.position);
            //anim.SetFloat("moveX", change.x);
            //anim.SetFloat("moveY", change.y);
            //anim.SetBool("moving", true);
        }
        else
        {
           //anim.SetBool("moving", false);
        }
    }

    void MoveCharacter(Vector3 old)
    {
        Vector3 Movement = change.normalized * playerSpeed * Time.deltaTime;
        GetComponent<EntityManager>().MoveEntity(Movement);
    }

}
