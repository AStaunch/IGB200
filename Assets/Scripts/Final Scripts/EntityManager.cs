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
    public float entitySpeed = 1f;
    protected Animator anim;
    public float Deceleration = 5f;

    // Start is called before the first frame update
    void Awake() {
        if (!gameObject.TryGetComponent(out rb) && entityType.Equals(EntityType.Creature)) {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        if(rb != null){
            rb.gravityScale = 0;
            rb.freezeRotation = true;
            
        }
        maxHealth = health;
        gameObject.TryGetComponent(out anim);
    }

    Vector2 previousVelocity;
    private void FixedUpdate() {
        if (rb) {
            if(rb.velocity.magnitude <= previousVelocity.magnitude) {
                rb.velocity = Decelerate(rb.velocity);
            }
            previousVelocity = rb.velocity;
        }
        //Update 
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(-transform.position.y);
    }

    private Vector2 Decelerate(Vector2 velocity) {
        //Debug.Log(vector2);
        if(velocity == Vector2.zero) {
            return velocity;
        }

        velocity -= Deceleration * Time.deltaTime * velocity;

        if (velocity.magnitude < 0.25f) {
            velocity *= 0f;
        }
        if (anim && velocity == Vector2.zero) {
            UpdateAnimation(velocity);
        }
        return velocity;
    }

    #region Update Variables Externally
    public void UpdateVelocity(Vector3 change) {
        if(rb){
            rb.velocity = change;
        }
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
#endregion

    #region Entity Health and Death
    public void TakeDamage(float damage) {
        if (entityProperties.Contains(Properties.Indestructable)) {
            damage = 0f;
        }
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);
        if (0 >= health) {
            EntityDeath();
        }
    }

    private void EntityDeath() {

        if (entityType == EntityType.Object) {
            if (entityProperties.Contains(Properties.Door) && TryGetComponent(out DoorEntity portal)) {
                portal.SetDoor(true);
            }
        } else {
            if (transform.CompareTag("Player")) {
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            }
            Destroy(this.gameObject, Time.deltaTime);
        }
        // TODO: Impliment Colour change

    }
    #endregion

    #region Property Management
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

    IEnumerator Wait(float time) {
        yield return new WaitForSeconds(time);
    }
    #endregion

    #region Facing Getters
    public Directions GetEntityDirectionEnum() {
        return CurrentDirection;
    }
    public int GetEntityFacing() {
        return IntDict[CurrentDirection];
    }
    public Vector2 GetEntityDirection() {
        return VectorDict[CurrentDirection];
    }
    #endregion
}
