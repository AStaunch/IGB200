using System.Collections;
using System.Collections.Generic;
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
    public override void Decelerate() {
        throw new System.NotImplementedException();
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
}
