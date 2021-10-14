using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;
public abstract class AbstractEnemy : AbstractCreature, iEnemyInterface
{
    public float AttackDelay;
    public float EntityDamage;
    public Elements DamageType = Elements.NULL;
    public float EntityDamage_ { get => EntityDamage; set => EntityDamage = value; }
    public GameObject enemDeathSound;
    public GameObject attackWhiffSound;
    public GameObject attackDamageSound;
    public GameObject enemPushSound;
    public void Start() {
        Health_ = MaxHealth_;
    }
    public void Update() {
        Vector3 change = CalculateFacing();
        UpdateAnimation(change);
    }
    public abstract Vector3 CalculateFacing();
    public override void Decelerate() {
        if (RB_.velocity != Vector2.zero && gameObject.layer == 7) {
            RB_.velocity *= 0.1f;
        }
    }

    public override void EntityDeath() {
        Anim_.SetTrigger("death");
        Instantiate(enemDeathSound);
        Destroy(this.gameObject);
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

    public override void UpdateForce(float magnitude, Vector3 direction) {
        if (EntityProperties_.Contains(Properties.Immovable)) {
            return;
        }
        gameObject.layer = 6;
        Instantiate(enemPushSound);
        RB_.AddForce(magnitude * direction * RB_.mass, ForceMode2D.Impulse);
    }
}
