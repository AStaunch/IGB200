using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed;
    private Vector3 change;
    private EntityManager em;
    
   public bool isCasting;
    // Start is called before the first frame update
    void Awake()
    {
        em = GetComponent<EntityManager>();
        em.entitySpeed = playerSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        if(change != Vector3.zero) {
            em.UpdatePosition(change);
            em.UpdateAnimation(change);
        }


        if (Input.GetKeyDown(KeyCode.Space)){
            CastSpell();
        }
    }

    public void CastSpell() {
        Animator anim = GetComponent<Animator>();
        Vector2 facing = em.GetEntityDirection();
        anim.SetFloat("moveX", facing.x);
        anim.SetFloat("moveY", facing.y);
        anim.SetBool("casting", true);
        while (anim.GetBool("casting")) {
            anim.SetBool("casting", isCasting);
        }
    }
}
