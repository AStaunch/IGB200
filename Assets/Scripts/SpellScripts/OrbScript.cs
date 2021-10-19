using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumsAndDictionaries;
using static SpellFunctionLibrary;
using static SoundManager;

public class OrbScript : MonoBehaviour, iPhysicsInterface
{
    public float baseDamage = 1f;
    public Elements element;

    private Animator Anim_ { get => GetComponent<Animator>(); }
    public Rigidbody2D RB_ => GetComponent<Rigidbody2D>();
    public float Deceleration_ { get => 1f; set => _ = value; }
    public Properties[] EntityProperties_ { get => new Properties[] { Properties.Light }; set => _ = value; }
    public EntityTypes EntityType_ => EntityTypes.Object;
    private float radius = 5f;
    private float duration = 5f;
    // Start is called before the first frame update
    void Start() {
        Anim_.speed = 1 / duration;
        
    }
    public void AlertObservers(AnimationEvents message) {
        if (message == AnimationEvents.Explode) {
            Debug.Log("Boom" + Time.timeSinceLevelLoad);
            Anim_.SetTrigger("explode");
            if (element == Elements.Push || element == Elements.Pull) {
                ExplodePhysics();
            } else {
                ExplodeDamage();
            }
        } else if (message == AnimationEvents.Finish) {
            Destroy(this.gameObject);
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
            }
        }
    }
    public void UpdateForce(float magnitude, Vector3 direction, Elements elementType) {
        if (EntityProperties_.Contains(Properties.Immovable)) {
            return;
        }
        gameObject.layer = 6;
        Instantiate(SoundDict[elementType.ToString() + "Sound"]);
        RB_.AddForce(magnitude * direction * RB_.mass, ForceMode2D.Impulse);
        Debug.Log(magnitude * direction * RB_.mass);
    }

    public void UpdateVelocity(float magnitude, Vector3 direction) {
        throw new System.NotImplementedException();
    }
    public void Decelerate() {
        throw new System.NotImplementedException();
    }
    public void UpdateSorting() {
        throw new System.NotImplementedException();
    }
}