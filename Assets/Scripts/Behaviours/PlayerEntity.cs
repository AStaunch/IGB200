using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;

public class PlayerEntity : AbstractCreature
{    
   public bool isCasting;
    // Update is called once per frame
    void Update()
    {
        Vector3 change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        if (change != Vector3.zero) {
            change = EntitySpeed_ * change.normalized;
            if(VectorToDirection(change) == GetEntityDirectionEnum()) {
                UpdateVelocity(change.magnitude, change.normalized);
            }
            UpdateDirection(change);
            UpdateAnimation(RB_.velocity);
        } else {

            UpdateAnimation(RB_.velocity);
        }

        if (Input.GetKeyDown(KeyCode.Space)){
            CastSpell();
        }


        if (Input.GetKeyDown(KeyCode.R)) {
            transform.position = lastCheckpoint.transform.position;
        }
    }

    public void CastSpell() {
        RB_.velocity = Vector2.zero;
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("attack");
    }

    public override void EntityDeath() {
        Anim_.SetBool("isDead", true);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public override void UpdateAnimation(Vector3 change) {
        throw new System.NotImplementedException();
    }

    public override void UpdateVelocity(float magnitude, Vector3 direction) {
        throw new System.NotImplementedException();
    }
    public override void Decelerate() {
        throw new System.NotImplementedException();
    }


    //Hacky Checkpoint Management
    GameObject lastCheckpoint;
    List<Rigidbody2D> collidedObjects = new List<Rigidbody2D>();

    public override Elements[] DamageImmunities_ { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private void OnCollisionEnter2D(Collision2D collision) {
        bool b1 = collision.transform.TryGetComponent(out iPropertyInterface _); 
        bool b2 = collision.transform.TryGetComponent(out Rigidbody2D rb);
        if (b1 && b2) {
            collidedObjects.Add(rb);
            rb.isKinematic = true;
            rb.useFullKinematicContacts = true;
            rb.velocity = Vector2.zero;
        }        
    }

    //Very Hacky Kino Management
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
