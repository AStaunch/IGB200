using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsAndDictionaries;
using static SpellFunctionLibrary;

public class OrbScript : MonoBehaviour
{
    public float baseDamage = 1f;
    public Elements element;

    private Animator Anim_ { get => GetComponent<Animator>(); }
    private float radius = 5f;
    private float duration = 5f;
    // Start is called before the first frame update
    void Start() {
        Anim_.speed = 1 / duration;
    }

    public void WaitForAnimEnd() {
        Debug.Log("Boom" + Time.timeSinceLevelLoad);
        if (element == Elements.Push || element == Elements.Pull) {
            ExplodePhysics();
        } else {
            ExplodeDamage();
        }
    }

    private void ExplodePhysics() {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, radius)) {
            if (collider.TryGetComponent(out iPhysicsInterface iPhysics_)) {
                float force = ComputeOutPutValue(element, iPhysics_.EntityProperties_, baseDamage);
                Vector2 direction = transform.position - collider.transform.position;
                iPhysics_.UpdateForce(force, direction, element);
            }
        }
    }

    private void ExplodeDamage() {
        int layerMask = 1 << 5;
        layerMask = ~layerMask;
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, radius, layerMask)) {
            if (collider.TryGetComponent(out iHealthInterface health_)) {
                float damage = ComputeOutPutValue(element, health_.EntityProperties_, baseDamage);
                health_.TakeDamage(damage, element);
                //Debug.Log(collider.transform.name);
            }
        }
        Destroy(this.gameObject);
    }
}