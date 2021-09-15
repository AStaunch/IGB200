using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : EntityManager
{    
   public bool isCasting;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        Vector3 change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        if (change != Vector3.zero) {
            change = entitySpeed * change.normalized;

            UpdateVelocity(change);
            UpdateAnimation(change);
            UpdateDirection(change);
        }
        if (Input.GetKeyDown(KeyCode.Space)){
            CastSpell();
        }
    }

    public void CastSpell() {
        Animator anim = GetComponent<Animator>();
        Vector2 facing = GetEntityDirection();
        anim.SetFloat("moveX", facing.x);
        anim.SetFloat("moveY", facing.y);
        anim.SetBool("casting", true);
        while (anim.GetBool("casting")) {
            anim.SetBool("casting", isCasting);
        }
    }

    private Vector2 Decelerate(Vector2 velocity) {
        //Debug.Log(vector2);
        if (velocity == Vector2.zero) {
            return velocity;
        }
        velocity -= Deceleration * Time.deltaTime * velocity;
        if (velocity.magnitude < 0.01f) {
            velocity *= 0f;
        }
        if (anim && velocity == Vector2.zero) {
            UpdateAnimation(velocity);
        }
        return velocity;
    }

    List<Rigidbody2D> collidedObjects = new List<Rigidbody2D>();
    private void OnCollisionEnter2D(Collision2D collision) {
        bool b1 = collision.transform.TryGetComponent(out EntityManager _); 
        bool b2 = collision.transform.TryGetComponent(out Rigidbody2D rb);
        if (b1 && b2) {
            collidedObjects.Add(rb);
            rb.isKinematic = true;
            rb.useFullKinematicContacts = true;
            rb.velocity = Vector2.zero;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.transform.TryGetComponent(out Rigidbody2D rb)){
            if(collidedObjects.Contains(rb)) {
                collidedObjects.Remove(rb);
                rb.isKinematic = false;
            }
        }
        
    }
}
