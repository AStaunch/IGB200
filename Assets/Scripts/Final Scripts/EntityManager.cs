using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using static EnumsAndDictionaries;

public class EntityManager : MonoBehaviour
{
    public Properties[] entityProperties;

    public EntityType entityType;

    Directions CurrentDirection = Directions.Down;

    protected Rigidbody2D rb;
    [SerializeField]
    protected float health = 10;
    protected float maxHealth;

    //[SerializeField]
    public float entitySpeed;
    [SerializeField]
    private Sprite[] EntitySpriteSheet;
    protected Animator anim;
    private Vector2 previousVelocity;

    // Start is called before the first frame update
    void Awake() {
        if (!gameObject.TryGetComponent(out rb) && entityType.Equals(EntityType.Creature)) {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        if(rb != null){
            rb.gravityScale = 0;
            rb.freezeRotation = true;
            if(rb.drag == 0)
                rb.drag = 1f;
            //rb.bodyType = RigidbodyType2D.Kinematic;
            rb.mass = 1f;
        }
        maxHealth = health;
        gameObject.TryGetComponent(out anim);
    }

    private void Update() {
        if (rb) {
            Decelerate();
        }

    }
    private void Decelerate() {
        if (entityType == EntityType.Creature && previousVelocity.magnitude >= rb.velocity.magnitude) {
            rb.velocity *= 0.5f;
        }
        if (rb.velocity.magnitude < 0.01f) {
            rb.velocity *= 0f;
        }
        previousVelocity = rb.velocity;
        if (anim && rb.velocity == Vector2.zero) {
            UpdateAnimation(rb.velocity);
        }
    }

    public void TakeDamage(float damage) {
        if (entityProperties.Contains(Properties.Indestructable)){
            damage = 0f; 
        }
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);
        if (0 >= health) {
            EntityDeath();
        }
    }
    public void UpdatePosition(Vector3 change) {
        if (change != Vector3.zero) {
            UpdateDirection(change);
        }
        change = entitySpeed * change.normalized;
        rb.velocity = (change);
    }

    public void UpdateAnimation(Vector3 change) {
        if (change != Vector3.zero) {
            anim.SetFloat("moveX", change.x);
            anim.SetFloat("moveY", change.y);
            anim.SetBool("moving", true);
        } else {
            anim.SetBool("moving", false);
        }
    }

    public void UpdateDirection(Vector3 change) {   //Up ; Right; Down; Left
        if (change != Vector3.zero) {

            if (Mathf.Abs(change.x) < Mathf.Abs(change.y)) {
                if (change.y > 0) {
                    CurrentDirection = Directions.Up;
                } else {
                    CurrentDirection = Directions.Down;
                }
            } else {
                if (change.x > 0) {
                    CurrentDirection = Directions.Right;
                } else {
                    CurrentDirection = Directions.Left;
                }
            }
        }
    }

    private void EntityDeath() {

        if (entityType == EntityType.Object) {
            if (entityProperties.Contains(Properties.Door) && TryGetComponent(out DoorScript portal)) {
                portal.SetDoor(true);
            }
        } else {
            if (transform.CompareTag("Player")) {
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            }
            Destroy(this.gameObject, Time.deltaTime);
        }
        // TODO: Impliment Colour change
        // SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public void AddProperty(Properties property) {
        if (!entityProperties.Contains(property)) {
            //Add Property
            Array.Resize(ref entityProperties, entityProperties.Length + 1);
            entityProperties[entityProperties.Length - 1] = property;
        }
    }

    public void AddProperty(Properties property, float duration) {
        if (!entityProperties.Contains(property)) {
            //Add Property
            Array.Resize(ref entityProperties, entityProperties.Length + 1);
            entityProperties[entityProperties.Length - 1] = property;

            //WaitForSeconds
            IEnumerator coroutine = Wait(duration);
            StartCoroutine(coroutine);

            //Remove Property
            Array.Resize(ref entityProperties, entityProperties.Length - 1);
        }
    }

    float timer = 0;
    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.transform.TryGetComponent(out BlockScript _) || collision.transform.CompareTag("Ground")) {
            timer = 0;
        } else if (collision.transform.CompareTag("Void")) {
            timer += Time.deltaTime;
        }
        if (timer > 1f) {
            EntityDeath();
        }
    }
IEnumerator Wait(float time) {
        yield return new WaitForSeconds(time);
    }

    public Directions GetEntityDirectionEnum() {
        return CurrentDirection;
    }
    public int GetEntityFacing() {
        return IntDict[CurrentDirection];
    }
    public Vector2 GetEntityDirection() {
        return VectorDict[CurrentDirection];
    }
}
