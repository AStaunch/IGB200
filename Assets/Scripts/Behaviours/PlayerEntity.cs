using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;

public class PlayerEntity : AbstractCreature
{    

    private void Start() {
        Deceleration_ = 5;
        Health_ = MaxHealth_;
        EntitySpeed_ = 5;
        gameObject.layer = 7;
        if (DamageImmunities_ == null) {
            DamageImmunities_ = new Elements[0];
        }
    }
    private Vector3 change;
    // Update is called once per frame
    void Update()
    {
        if(this.gameObject.layer == 7) {
            change = Vector3.zero;
            change.x = Input.GetAxisRaw("Horizontal");
            change.y = Input.GetAxisRaw("Vertical");
            if (change != Vector3.zero) {
                if (VectorToDirection(change) == GetEntityDirectionEnum()) {
                    UpdateVelocity(EntitySpeed_, change.normalized);
                }
                UpdateDirection(change);
                UpdateAnimation(RB_.velocity);
            } else {
                UpdateAnimation(RB_.velocity);
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            EntityDeath();
        }
    }

    public void CastSpell() {
        RB_.velocity = Vector2.zero;
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("attack");
        Instantiate(castingSound);
    }

    public override void EntityDeath() {
        if(LastDoor != null) {
            Health_ = MaxHealth_;
            transform.position = LastDoor_.RespawnPoint;
            LastDoor.ExitDoor.ReloadRoom();
        }
    }
    protected override void EntityFall() {
        if (LastDoor != null) {
            TakeDamage(1f, Elements.NULL);
            transform.position = LastDoor_.RespawnPoint;
            LastDoor.ExitDoor.ReloadRoom();
        }
    }

    public override void UpdateAnimation(Vector3 change) {
        if (change != Vector3.zero) {
            Anim_.SetFloat("moveX", change.x);
            Anim_.SetFloat("moveY", change.y);
            Anim_.SetBool("moving", true);
        } else {
            Anim_.SetBool("moving", false);
        }
    }

    public override void UpdateVelocity(float magnitude, Vector3 direction) {
            RB_.velocity = magnitude * direction;
    }
    public override void Decelerate() {
        if (RB_.velocity != Vector2.zero && change == Vector3.zero && gameObject.layer == 7) {
            RB_.velocity *= 0.1f;
        }
    }


    //Hacky Checkpoint Management
    public AbstractDoor LastDoor_ { get => LastDoor; set => LastDoor = value; }
    private AbstractDoor LastDoor;
    List<Rigidbody2D> collidedObjects = new List<Rigidbody2D>();
    public GameObject castingSound;

    //Very Hacky Kino Management
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
    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.transform.TryGetComponent(out Rigidbody2D rb)){
            if(collidedObjects.Contains(rb)) {
                collidedObjects.Remove(rb);
                rb.isKinematic = false;
            }
        }        
    }
    private void OnCollisionStay2D(Collision2D collision) {
        if(gameObject.layer != 6) {
            CheckFalling(collision);
        }
    }

}
