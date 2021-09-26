using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;

public class PlayerEntity : EntityManager
{    
   public bool isCasting;
    // Update is called once per frame
    void Update()
    {
        Vector3 change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        if (change != Vector3.zero) {
            change = entitySpeed * change.normalized;
            if(VectorToDirection(change) == GetEntityDirectionEnum()) {
                UpdateVelocity(change);
            }
            UpdateDirection(change);
            UpdateAnimation(rb.velocity);
        } else {

            UpdateAnimation(rb.velocity);
        }

        if (Input.GetKeyDown(KeyCode.Space)){
            CastSpell();
        }


        if (Input.GetKeyDown(KeyCode.R)) {
            transform.position = lastCheckpoint.transform.position;
        }
    }

    public void CastSpell() {
        rb.velocity = Vector2.zero;
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("attack");
    }

    public override void EntityDeath() {
        anim.SetBool("isDead", true);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    GameObject lastCheckpoint;
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

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.TryGetComponent(out CheckPoint cp)) {
            lastCheckpoint = cp.gameObject;
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