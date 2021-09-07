using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed;
    private Vector3 change;
    private EntityManager em;
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
    }
}
