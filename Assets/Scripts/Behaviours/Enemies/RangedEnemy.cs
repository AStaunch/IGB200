using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;

public class RangedEnemy : AbstractEnemy
{
    private Vector3 CurrentPosition = Vector3.zero;
    private Vector3 LastPosition = Vector3.zero;
    public override Vector3 CalculateFacing() {
        return Vector3.zero;
    }
    private float AttackTime_ { get { return AttackTime; } set { AttackTime = Time.timeSinceLevelLoad + value; } }//Automatically update the cast time to the new time
    private float AttackTime;
    public GameObject Projectile;
    public float ProjectileSpeed = 1f;
    public void Attack(Vector2 Direction) {
        Anim_.SetTrigger("attack");
        GameObject newProjectile = Instantiate(Projectile);
        ProjectileScript projectile = newProjectile.AddComponent<ProjectileScript>();
        projectile.Velocity = ProjectileSpeed;
        projectile.Direction = Direction;
        projectile.Damage = EntityDamage_;
        projectile.element = DamageType;
    }
}
