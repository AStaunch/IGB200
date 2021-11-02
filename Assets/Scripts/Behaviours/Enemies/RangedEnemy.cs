using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        AttackTime_ = AttackDelay;
        UpdateAnimation(PlayerEntity.Instance.transform.position - transform.position);
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
            if (checkLOS()) {
                Attack(PlayerEntity.Instance.transform.position - transform.position);
            }
        }
    }

    private void LastSeenFSM() {

    }

    private bool checkLOS() {
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(transform.position, PlayerEntity.Instance.transform.position, Range)) {
            if (hit.transform.gameObject.layer == 8) {
                return false;
            } else if (hit.transform.gameObject != PlayerEntity.Instance.gameObject) {
                return true;
            }
        }
        return false;
    }
}
