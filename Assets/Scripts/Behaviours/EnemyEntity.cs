using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : AbstractCreature, iEnemyInterface
{
    //TODO?
    public EnumsAndDictionaries.Elements[] DamageImmunities;
    public override EnumsAndDictionaries.Elements[] DamageImmunities_ { get => DamageImmunities; set => DamageImmunities = value; }
    public float EntityDamage;
    public float EntityDamage_ { get => EntityDamage; set => EntityDamage = value; }

    public override void Decelerate() {
        throw new System.NotImplementedException();
    }

    public override void EntityDeath() {
        Anim_.SetBool("isDead", true);
        Destroy(this.gameObject);
    }

    public override void UpdateAnimation(Vector3 change) {
        throw new System.NotImplementedException();
    }

    public override void UpdateVelocity(float magnitude, Vector3 direction) {
        throw new System.NotImplementedException();
    }
}
