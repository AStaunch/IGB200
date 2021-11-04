using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;

public class MeleeEnemy : AbstractEnemy
{
    private Vector3 CurrentPosition = Vector3.zero;
    private Vector3 LastPosition = Vector3.zero;

    public override Vector3 CalculateFacing() {
        if (CurrentPosition == Vector3.zero) {
            LastPosition = Vector3.zero;
        } else {
            LastPosition = CurrentPosition;
        }
        CurrentPosition = transform.position;
        return (CurrentPosition - LastPosition).normalized;
    }
    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.transform.TryGetComponent(out PlayerEntity playerEntity) && AttackTime_ < Time.timeSinceLevelLoad) {
            AttackTime_ = AttackDelay;
            playerEntity.TakeDamage(EntityDamage_, DamageType);
            Anim_.SetTrigger("attack");            
            StartCoroutine(MovePause(AttackDelay));
        }
    }


}
