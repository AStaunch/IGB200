using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;

public class RangedEnemy : AbstractEnemy
{
    private Vector3 lastAttackDir = Vector3.zero;
    public override Vector3 CalculateFacing() {
        return lastAttackDir;
    }

    public GameObject Projectile;
    public float ProjectileSpeed = 3f;
    public void Attack(Vector2 Direction) {
        Anim_.SetTrigger("attack");
        Vector3 Dir3 = Direction.normalized;
        GameObject newProjectile = Instantiate(Projectile, transform.position + Dir3, transform.rotation);
        ProjectileScript projectile = newProjectile.AddComponent<ProjectileScript>();
        projectile.Shooter = this.gameObject;
        projectile.Velocity = ProjectileSpeed;
        projectile.Direction = Direction;
        projectile.Damage = EntityDamage_;
        projectile.element = DamageType;
        projectile.StartProj();
        lastAttackDir = Direction;
    }
    public float Range = 20f;
    public new void  Update() {
        if (Vector3.Distance(transform.position, PlayerEntity.Instance.transform.position) < Range && AttackTime_ < Time.timeSinceLevelLoad && !isFrozen_) {
            Attack(PlayerEntity.Instance.transform.position - transform.position);
            AttackTime_ = AttackDelay;
            UpdateAnimation(PlayerEntity.Instance.transform.position - transform.position);
        }

    }
}
