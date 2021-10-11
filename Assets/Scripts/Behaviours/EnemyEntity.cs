using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;
public class EnemyEntity : AbstractCreature, iEnemyInterface
{
    public GameObject enemDeathSound;
    //TODO?

    public float EntityDamage;
    public float EntityDamage_ { get => EntityDamage; set => EntityDamage = value; }

    public void Start() {
        Health_ = MaxHealth_;
        if (DamageImmunities_ == null) {
            DamageImmunities_ = new Elements[0];
        }
    }

    private void FixedUpdate() {
        if (RB_.velocity != Vector2.zero && gameObject.layer == 7) {
            Decelerate();
        }
    }
    public override void Decelerate() {
        RB_.velocity *= 0.1f;
    }

    public override void EntityDeath() {
        Anim_.SetBool("isDead", true);
        Instantiate(enemDeathSound);
        Destroy(this.gameObject);
    }

    public override void UpdateAnimation(Vector3 change) {
        throw new System.NotImplementedException();
    }

    public override void UpdateVelocity(float magnitude, Vector3 direction) {
        RB_.velocity = magnitude * direction;
    }

    public override void UpdateForce(float magnitude, Vector3 direction) {
        if (EntityProperties_.Contains(Properties.Immovable)) {
            return;
        }
        gameObject.layer = 6;
        RB_.AddForce(magnitude * direction * RB_.mass, ForceMode2D.Impulse);
    }
}
